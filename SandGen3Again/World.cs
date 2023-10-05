using SandGen3Again.Elements;
using SFML.Graphics;
using SFML_Game_Engine;

namespace SandGen3Again
{
    public class World : Component
    {
        float tickTimer = 0;
        float tickTime = 1f / 60f;
        public int sizeX;
        public int sizeY;
        Chunk[,] chunks;

        /// <summary>
        /// If true, and GetElement() calls that go out of bounds will loop back around.
        /// </summary>
        public bool loopEdges = false;

        public World(int sizeX, int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            chunks = new Chunk[sizeX, sizeY];

            for(int x = 0; x < sizeX; x++)
            {
                for(int y = 0; y < sizeY; y++)
                {
                    chunks[x, y] = new Chunk();
                }
            }

            for(int x = 0; x < sizeX * Chunk.Size; x++)
            {
                SetElementImmediate(x, sizeY * Chunk.Size - 1, new Stone());
            }
        }

        public override void Update()
        {
            tickTimer += deltaTime;
            if(tickTimer >= tickTime)
            {
                tickTimer = 0;
                WorldTick();
            }
        }

        public bool WithinLocalBounds(int x, int y)
        {
            if (x >= Chunk.Size || y >= Chunk.Size) { return false; }
            if (x < 0 || y < 0) { return false; }
            return true;
        }

        public void PhysicsTick(Chunk chunk, int cx, int cy)
        {
            for(int x = 0; x < Chunk.Size; x++)
            {
                for(int y = 0; y < Chunk.Size; y++)
                {
                    int wX = x + (Chunk.Size * cx);
                    int wY = y + (Chunk.Size * cy);
                    Element atPos = GetElement(wX, wY);

                    if (atPos.phsType == physicsType.None) { continue; }

                    if (atPos.phsType == physicsType.Sand)
                    {
                        if (GetElement(wX, wY + 1) is Air)
                        {
                            RemoveElement(wX, wY);
                            SetElement(wX, wY + 1, atPos);
                            continue;
                        }

                        bool leftFree = GetElement(wX - 1, wY + 1) is Air;
                        bool rightFree = GetElement(wX + 1, wY + 1) is Air;

                        if (leftFree && rightFree)
                        {
                            int coinFlip = RandomGen.Next(0, 10);
                            RemoveElement(wX, wY);
                            if (coinFlip > 5)
                            {
                                SetElement(wX - 1, wY + 1, atPos);
                                continue;
                            }
                            SetElement(wX + 1, wY + 1, atPos);
                            continue;
                        }

                        if(!leftFree && !rightFree) { continue; }

                        RemoveElement(wX, wY);
                        if (leftFree)
                        {
                            SetElement(wX - 1, wY + 1, atPos);
                            continue;
                        }
                        if (rightFree)
                        {
                            SetElement(wX + 1, wY + 1, atPos);
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

        // Requests to set an element at a position, if there is already a request to move there it will flip a coin to decide if it should replace it.
        public void SetElement(int worldX, int worldY, Element elm)
        {
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            atPos.SetElement(Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size), elm);

            //Console.WriteLine($"{worldX}, {worldY} =>  {worldX % Chunk.Size}, {worldY % Chunk.Size}");
        }

        public void SetElementImmediate(int worldX, int worldY, Element elm)
        {
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            atPos.elms[Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size)] = elm;
        }

        public void RemoveElement(int worldX, int worldY)
        {
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            atPos.RemoveElement(Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size));
        }

        public void WorldTick()
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (chunks[x, y].sleeping == false)
                    {
                        PhysicsTick(chunks[x, y], x, y);
                    }
                }
            }

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (chunks[x, y].sleeping == false)
                    {
                        chunks[x, y].ProcessChanges();
                    }
                }
            }
        }

        public void TestDraw(RenderWindow app, float scale)
        {
            for(int x = 0;x < sizeX; x++)
            {
                for(int y= 0; y < sizeY; y++)
                {
                    chunks[x, y].Render(app, new Vector2(x * Chunk.Size, y * Chunk.Size) * scale, new Vector2(scale, scale));
                }
            }
        }
    }
}
