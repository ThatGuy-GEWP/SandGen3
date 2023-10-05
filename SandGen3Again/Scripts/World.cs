using SandGen3Again.Elements;
using SFML.Graphics;
using SFML_Game_Engine;

namespace SandGen3Again.Scripts
{
    public class World : Component, IRenderable
    {
        float tickTimer = 0;
        float tickTime = 1f / 144f;
        public int sizeX;
        public int sizeY;
        Chunk[,] chunks;

        public sbyte zOrder { get; set; } = -1;

        /// <summary>
        /// If true, and GetElement() calls that go out of bounds will loop back around.
        /// </summary>
        public bool loopEdges = false;

        public World(int sizeX, int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            chunks = new Chunk[sizeX, sizeY];

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    chunks[x, y] = new Chunk();
                }
            }

            for (int x = 0; x < sizeX * Chunk.Size; x++)
            {
                //SetElementImmediate(x, sizeY * Chunk.Size - 1, new Stone());
            }
        }

        public void SwapElement(int fromWX, int fromWY, int toWX, int toWY)
        {
            AwakeIfNearEdge(fromWX, fromWY);
            AwakeIfNearEdge(toWX, toWY);
            if (fromWX < 0 || fromWY < 0) { return; }

            Chunk from = ChunkFromWorld(fromWX, fromWY);
            Chunk to = ChunkFromWorld(toWX, toWY);

            to.SwapElement(toWX % Chunk.Size, toWY % Chunk.Size, from, fromWX % Chunk.Size, fromWY % Chunk.Size);
        }

        public override void Update()
        {
            tickTimer += deltaTime;
            if (tickTimer >= tickTime)
            {
                tickTimer = 0;
                WorldTick();
            }
            gameObject.scene.renderManager.AddToRenderQueue(this);
        }

        public bool WithinWorldBounds(int worldX, int worldY)
        {
            if (worldX >= Chunk.Size * sizeX || worldY >= Chunk.Size * sizeY) { return false; }
            if (worldX < 0 || worldY < 0) { return false; }
            return true;
        }

        public bool WithinLocalBounds(int x, int y)
        {
            if (x >= Chunk.Size || y >= Chunk.Size) { return false; }
            if (x < 0 || y < 0) { return false; }
            return true;
        }

        // does the physics required for all chunks.
        public void PhysicsTick(Chunk chunk, int cx, int cy)
        {
            for (int x = 0; x < Chunk.Size; x++)
            {
                for (int y = 0; y < Chunk.Size; y++)
                {
                    int wX = x + Chunk.Size * cx;
                    int wY = y + Chunk.Size * cy;
                    Element atPos = GetElement(wX, wY);

                    if (atPos.phsType == physicsType.None) { continue; }

                    // LOTS of reusable code here, should prob pack into a function
                    // but the current solution works for now.

                    if (atPos.phsType == physicsType.Liquid)
                    {
                        if (GetElement(wX, wY + 1) is Air && (!loopEdges && WithinWorldBounds(wX, wY + 1)) | loopEdges == true)
                        {
                            SwapElement(wX, wY, wX, wY + 1);
                            continue;
                        }

                        bool leftFree = GetElement(wX - 1, wY) is Air;
                        bool rightFree = GetElement(wX + 1, wY) is Air;

                        leftFree = loopEdges == false ? leftFree && WithinWorldBounds(wX - 1, wY) : leftFree;
                        rightFree = loopEdges == false ? rightFree && WithinWorldBounds(wX + 1, wY) : rightFree;

                        if (leftFree && rightFree)
                        {
                            int coinFlip = RandomGen.Next(0, 2);
                            if (coinFlip == 1)
                            {
                                SwapElement(wX, wY, wX - 1, wY);
                                continue;
                            }
                            SwapElement(wX, wY, wX + 1, wY);
                            continue;
                        }

                        if (!leftFree && !rightFree) { continue; }

                        if (leftFree)
                        {
                            SwapElement(wX, wY, wX - 1, wY);
                            continue;
                        }
                        if (rightFree)
                        {
                            SwapElement(wX, wY, wX + 1, wY);
                            continue;
                        }
                    }

                    if (atPos.phsType == physicsType.Sand)
                    {
                        // hmm torn on just scrapping looped edges, but it would be wanted for some games so il keep it
                        // for now and keep doing this bool based hell.
                        if (GetElement(wX, wY + 1) is Air && (!loopEdges && WithinWorldBounds(wX, wY + 1)) | loopEdges == true)
                        {
                            SwapElement(wX, wY, wX, wY + 1);
                            continue;
                        }

                        bool leftFree = GetElement(wX - 1, wY + 1) is Air && GetElement(wX - 1, wY) is Air;
                        bool rightFree = GetElement(wX + 1, wY + 1) is Air && GetElement(wX + 1, wY) is Air;

                        leftFree = loopEdges == false ? leftFree && WithinWorldBounds(wX - 1, wY + 1) && WithinWorldBounds(wX - 1, wY) : leftFree;
                        rightFree = loopEdges == false ? rightFree && WithinWorldBounds(wX + 1, wY + 1) && WithinWorldBounds(wX + 1, wY) : rightFree;

                        if (leftFree && rightFree)
                        {
                            int coinFlip = RandomGen.Next(0, 10);
                            if (coinFlip > 5)
                            {
                                SwapElement(wX, wY, wX - 1, wY + 1);
                                continue;
                            }
                            SwapElement(wX, wY, wX + 1, wY + 1);
                            continue;
                        }

                        if (!leftFree && !rightFree) { continue; }

                        if (leftFree)
                        {
                            SwapElement(wX, wY, wX - 1, wY + 1);
                            continue;
                        }
                        if (rightFree)
                        {
                            SwapElement(wX, wY, wX + 1, wY + 1);
                            continue;
                        }

                    }
                }
            }
        }

        public Chunk ChunkFromWorld(int worldX, int worldY)
        {
            return chunks[Math.Abs((int)MathF.Floor(worldX / Chunk.Size) % sizeX), Math.Abs((int)MathF.Floor(worldY / Chunk.Size) % sizeY)];
        }

        public Element GetElement(int worldX, int worldY)
        {
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            Element elm = atPos.elms[Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size)];
            return elm;
        }

        public void AwakeIfNearEdge(int worldX, int worldY)
        {
            int lX = worldX % Chunk.Size;
            int lY = worldY % Chunk.Size;

            if (lX == Chunk.Size - 1)
            {
                ChunkFromWorld(worldX + 1, worldY).Awake();
            }
            if (lX == 0)
            {
                ChunkFromWorld(worldX - 1, worldY).Awake();
            }
            if (lY == Chunk.Size - 1)
            {
                ChunkFromWorld(worldX, worldY + 1).Awake();
            }
            if (lY == 0)
            {
                ChunkFromWorld(worldX, worldY - 1).Awake();
            }
        }

        // Requests to set an element at a position, if there is already a request to move there it will flip a coin to decide if it should replace it.
        public void SetElement(int worldX, int worldY, Element elm)
        {
            AwakeIfNearEdge(worldX, worldY);
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            atPos.SetElement(Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size), elm);

            // Stuff
        }

        public void SetElementImmediate(int worldX, int worldY, Element elm)
        {
            AwakeIfNearEdge(worldX, worldY);
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            atPos.Awake();
            atPos.elms[Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size)] = elm;
        }

        public void RemoveElement(int worldX, int worldY)
        {
            AwakeIfNearEdge(worldX, worldY);
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            atPos.RemoveElement(Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size));
        }

        public void WorldTick()
        {

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    chunks[x, y].ProcessChanges();
                }
            }

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (!chunks[x, y].sleeping)
                    {
                        PhysicsTick(chunks[x, y], x, y);
                    }
                }
            }
        }

        public void Draw(RenderTarget rt)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    chunks[x, y].Render(rt, new Vector2(x * Chunk.Size, y * Chunk.Size) * 4, new Vector2(4f, 4f));
                }
            }
        }
    }
}
