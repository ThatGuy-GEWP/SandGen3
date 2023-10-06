using SandGen3Again.Elements;
using SFML.Graphics;
using SFML_Game_Engine;

namespace SandGen3Again.Scripts
{
    public class World : Component, IRenderable
    {
        public float scalingFactor = 1f;
        public bool useReactions = true;
        float tickTimer = 0;
        float tickTime = 1f / 60f;
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

            for (int x = 0; x < sizeX; x++)
            {
                chunks[x, 0].FillChunk<Water>();
            }

            for (int x = 0; x < sizeX; x++)
            {
                chunks[x, sizeY-1].FillChunk<Sand>();
            }

            for (int x = 0; x < sizeX * Chunk.Size; x++)
            {
                for(int i = 0; i < 32; i++)
                {
                    SetElementImmediate(x, ((sizeY - 1) * Chunk.Size - 1) + RandomGen.Next(-32, 0), new Sand());
                }
            }

            for (int x = 0; x < sizeX * Chunk.Size; x++)
            {
                //SetElementImmediate(x, sizeY * Chunk.Size - 1, new Stone());
            }
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

                    if (atPos.phsType == physicsType.Liquid || atPos.phsType == physicsType.Sand) // Falling function for both liquid, sand, and rigid
                    {
                        if ((!loopEdges && WithinWorldBounds(wX, wY + 1)) | loopEdges == true)
                        {
                            Element below = GetElement(wX, wY + 1);
                            if (below is Air)
                            {
                                SwapElement(wX, wY, wX, wY + 1);
                                continue;
                            }
                            else if (below.phsType != physicsType.None)
                            {
                                if (below.weight < atPos.weight)
                                {
                                    float weightDiff = atPos.weight - below.weight;
                                    if ((float)RandomGen.Next() <= weightDiff)
                                    {
                                        SwapElement(wX, wY, wX, wY + 1);
                                        continue;
                                    }
                                    else
                                    {
                                        chunk.Awake(); // this element is not settled, stay awake!
                                    }
                                }
                            }
                        }
                    }


                    if (atPos.phsType == physicsType.Liquid)
                    {
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
                        bool leftFree = GetElement(wX - 1, wY + 1) is Air && GetElement(wX - 1, wY) is Air;
                        bool rightFree = GetElement(wX + 1, wY + 1) is Air && GetElement(wX + 1, wY) is Air;

                        leftFree = loopEdges == false ? leftFree && WithinWorldBounds(wX - 1, wY + 1) && WithinWorldBounds(wX - 1, wY) : leftFree;
                        rightFree = loopEdges == false ? rightFree && WithinWorldBounds(wX + 1, wY + 1) && WithinWorldBounds(wX + 1, wY) : rightFree;

                        if (leftFree && rightFree)
                        {
                            int coinFlip = RandomGen.Next(0, 2);
                            if (coinFlip == 1)
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
            if (useReactions == false) { return; }
            for (int x = 0; x < Chunk.Size; x++)
            {
                for (int y = 0; y < Chunk.Size; y++)
                {
                    int wX = x + Chunk.Size * cx;
                    int wY = y + Chunk.Size * cy;
                    Element atPos = GetElement(wX, wY);
                    if (!(atPos is IReaction)) { continue; }

                    IReaction reaction = (IReaction)atPos;

                    if (reaction.ReactionChance < 100.0f)
                    {
                        float rng = RandomGen.Next() * 100;
                        if(rng >= reaction.ReactionChance)
                        {
                            chunk.Awake(); // Reaction can happen here still, stay awake!
                            continue;
                        }
                    }

                    // Not really readable but performance > readability
                    Element Above = GetElement(wX, wY - 1);
                    if (Above.GetType() == reaction.With && !reaction.NeedsSurrouned)
                    {
                        if (!reaction.KeepReactive) { RemoveElement(wX, wY - 1); }
                        SetElement(wX, wY, (Element)Activator.CreateInstance(reaction.To));
                        continue;
                    }

                    Element Left = GetElement(wX - 1, wY);
                    if (Left.GetType() == reaction.With && !reaction.NeedsSurrouned)
                    {
                        if (!reaction.KeepReactive) { RemoveElement(wX - 1, wY); }
                        SetElement(wX, wY, (Element)Activator.CreateInstance(reaction.To));
                        continue;
                    }

                    Element Right = GetElement(wX + 1, wY);
                    if (Right.GetType() == reaction.With && !reaction.NeedsSurrouned)
                    {
                        if (!reaction.KeepReactive) { RemoveElement(wX, wY + 1); }
                        SetElement(wX, wY, (Element)Activator.CreateInstance(reaction.To));
                        continue;
                    }

                    Element Below = GetElement(wX, wY + 1);
                    if (Below.GetType() == reaction.With && !reaction.NeedsSurrouned)
                    {
                        if (!reaction.KeepReactive) { RemoveElement(wX, wY + 1); }
                        SetElement(wX, wY, (Element)Activator.CreateInstance(reaction.To));
                        continue;
                    }

                    if (reaction.NeedsSurrouned)
                    {
                        if(
                            Above.GetType() == reaction.With && 
                            Left.GetType() == reaction.With && 
                            Right.GetType() == reaction.With && 
                            Below.GetType() == reaction.With
                            )
                        {
                            if (!reaction.KeepReactive)
                            {
                                RemoveElementImmediate(wX, wY + 1);
                                RemoveElementImmediate(wX, wY - 1);
                                RemoveElementImmediate(wX + 1, wY);
                                RemoveElementImmediate(wX - 1, wY);
                            }
                            SetElement(wX, wY, (Element)Activator.CreateInstance(reaction.To));
                        }
                    }


                }
            }
        }

        public void WorldTick()
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    chunks[x, y].ProcessChanges(x, y, this);
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

        /// <summary>
        /// Requests to swap an element from a position, to a position
        /// <para/>
        /// if there is already a request to swap, it will flip a coin to decide if it should swap.
        /// </summary>
        public void SwapElement(int fromWX, int fromWY, int toWX, int toWY)
        {
            AwakeIfNearEdge(fromWX, fromWY);
            AwakeIfNearEdge(toWX, toWY);
            if (fromWX < 0 || fromWY < 0) { return; }

            Chunk from = ChunkFromWorld(fromWX, fromWY);
            Chunk to = ChunkFromWorld(toWX, toWY);

            to.SwapElement(toWX % Chunk.Size, toWY % Chunk.Size, from, fromWX % Chunk.Size, fromWY % Chunk.Size);
        }

        /// <summary>
        /// Requests to set an element at a position, if there is already a request to move there it will flip a coin to decide if it should replace it.
        /// </summary>
        public void SetElement(int worldX, int worldY, Element elm)
        {
            AwakeIfNearEdge(worldX, worldY);
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            atPos.SetElement(Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size), elm);
        }

        /// <summary>
        /// Skips requesting a change and forces an element to a position.
        /// <para></para>
        /// If you dont know what that means/does, dont use this and use <see cref="World.SetElement(int, int, Type)"/> instead
        /// </summary>
        public void SetElementImmediate(int worldX, int worldY, Element elm)
        {
            AwakeIfNearEdge(worldX, worldY);
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            atPos.Awake();
            atPos.elms[Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size)] = elm;
            atPos.changes[Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size)].Recycle();
        }

        public void RemoveElement(int worldX, int worldY)
        {
            AwakeIfNearEdge(worldX, worldY);
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            atPos.RemoveElement(Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size));
        }

        public void RemoveElementImmediate(int worldX, int worldY)
        {
            AwakeIfNearEdge(worldX, worldY);
            Chunk atPos = ChunkFromWorld(worldX, worldY);
            atPos.Awake();
            atPos.elms[Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size)] = new Air();
            atPos.changes[Math.Abs(worldX % Chunk.Size), Math.Abs(worldY % Chunk.Size)].Recycle();
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

        public void Draw(RenderTarget rt)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    chunks[x, y].Render(rt, (new Vector2(x * Chunk.Size, y * Chunk.Size) * scalingFactor) + gameObject.position, new Vector2(scalingFactor, scalingFactor));
                }
            }
        }
    }
}
