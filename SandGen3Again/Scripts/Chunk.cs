using SandGen3Again.Elements;
using SFML.Graphics;
using SFML_Game_Engine;

namespace SandGen3Again.Scripts
{
    public enum ChangeType
    {
        None,
        Set,
        Remove,
        Swap
    }

    public struct Change // used to record changes, then apply them.
    {
        // To be honest i should not be using a struct here
        // this is not fixed size, you just know its all on the heap
        // but if it aint broke, dont fix it!
        public ChangeType type;
        public int lx;
        public int ly;
        public Element elm;

        public int authorHash; // used to make sure what we are swapping with is still there!

        // for use with "Swap"
        public Chunk fromChunk;

        public void Recycle() // Making new ones increased memory usage by ~100mb, this is much better.
        {
            fromChunk = null;
            type = ChangeType.None;
            elm = null;
            authorHash = 0;
            lx = 0;
            ly = 0;
        }
    }

    public class Chunk
    {
        public const int Size = 40; // size of chunk ^2

        public Element[,] elms = new Element[Size, Size];
        public Change[,] changes = new Change[Size, Size];
        public Texture chunkTexture = new Texture(Size, Size);

        public bool sleeping { get; private set; } = false;

        bool chunkChanged = false;

        /// <summary>
        /// If true, chunks will have a border around them showing their sleeping state.
        /// </summary>
        public static bool DebugRender = false;

        public Chunk()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    elms[x, y] = new Air();
                }
            }
            chunkChanged = true;
            sleeping = false;
        }

        public Element GetElement(int x, int y)
        {
            return elms[x, y];
        }

        /// <summary>
        /// Fills a chunk with the type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void FillChunk<T>() where T : Element
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    elms[x, y] = (Element)Activator.CreateInstance(typeof(T));
                }
            }
        }

        /// <summary>
        /// Swaps an element with another element.
        /// </summary>
        public void SwapElement(int thisX, int thisY, Chunk from, int fromX, int fromY)
        {
            // This is a mess, but a working mess!
            if (sleeping) { Awake(); }
            if (changes[thisX, thisY].type == ChangeType.Swap)
            {
                if(RandomGen.Next(2) == 1) // something else is here, flip a coin!
                {
                    changes[thisX, thisY].type = ChangeType.Swap;
                    changes[thisX, thisY].fromChunk = from;
                    changes[thisX, thisY].authorHash = from.GetElement(fromX, fromY).GetHashCode();
                    changes[thisX, thisY].lx = fromX;
                    changes[thisX, thisY].ly = fromY;
                }
                return;
            }
            changes[thisX, thisY].type = ChangeType.Swap;
            changes[thisX, thisY].fromChunk = from;
            changes[thisX, thisY].authorHash = from.GetElement(fromX, fromY).GetHashCode();
            changes[thisX, thisY].lx = fromX;
            changes[thisX, thisY].ly = fromY;
        }

        /// <summary>
        /// Queues the removal of an element.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RemoveElement(int x, int y)
        {
            if (sleeping) { Awake(); }
            changes[x, y] = new Change { lx = x, ly = y, type = ChangeType.Remove };
        }

        /// <summary>
        /// Queues placing an element.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="elm"></param>
        public void SetElement(int x, int y, Element elm)
        {
            if (sleeping) { Awake(); }
            if (changes[x, y].type == ChangeType.Set) // something else is already moving here
            {
                if (RandomGen.Next(2) == 1)
                {
                    changes[x, y] = new Change { elm = elm, lx = x, ly = y, type = ChangeType.Set };
                }
                return;
            }
            changes[x, y] = new Change { elm = elm, lx = x, ly = y, type = ChangeType.Set };
        }

        /// <summary>
        /// Discards changes at a position, and just sets an element.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="elm"></param>
        public void SetElementNow(int x, int y, Element elm)
        {
            changes[x, y].Recycle();
            elms[x, y] = elm;
        }

        /// <summary>
        /// Awakes a chunk.
        /// </summary>
        public void Awake()
        {
            chunkChanged = true;
            sleeping = false;
            GoingToSleep = false;
        }

        // Used to delay sleeping by a tick.
        // Instatly sleeping is bad if nearby chunks
        // Are trying to keep this one awake.
        public bool GoingToSleep = false;

        public void ProcessChanges(int cX, int cY, World world)
        {
            int changesProcessed = 0;
            bool hastickable = false;
            if (sleeping) { return; }

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (elms[x, y] is ITickable) { hastickable = true; } // oh god we have a tickable, stay awake!
                    Change curChange = changes[x, y];
                    if (curChange.type == ChangeType.None) { continue; }

                    if (curChange.type == ChangeType.Set)
                    {
                        elms[x, y] = curChange.elm;
                    }

                    if (curChange.type == ChangeType.Swap)
                    {
                        Element from = curChange.fromChunk.GetElement(curChange.lx, curChange.ly);
                        if(from.GetHashCode() != curChange.authorHash) { changes[x, y].Recycle(); continue; }
                        Element to = elms[x, y];

                        elms[x, y] = from;
                        curChange.fromChunk.SetElementNow(curChange.lx, curChange.ly, to);
                    }

                    if (curChange.type == ChangeType.Remove)
                    {
                        elms[x, y] = new Air();
                    }

                    chunkChanged = true;
                    changesProcessed++;
                    changes[x, y].Recycle();
                }
            }

            if (!hastickable)
            {
                if (changesProcessed == 0 && GoingToSleep)
                {
                    sleeping = true;
                    GoingToSleep = false;
                }

                if (changesProcessed == 0)
                {
                    GoingToSleep = true;
                }
            } 
            else
            {
                for (int x = 0; x < Size; x++)
                {
                    for (int y = 0; y < Size; y++)
                    {
                        if(elms[x, y] is ITickable)
                        {
                            ITickable tickable = (ITickable)elms[x, y];
                            tickable.OnTick(x + (Size * cX), y + (Size * cY), world);
                        }
                    }
                }
            }
        }

        public void UpdateTexture()
        {
            // For some reason, instead of using a byte[,] array to update a texture,
            // SFML likes just a flat byte[] array.
            // Im guessing its because the texture is on the GPU and somthing somthing openGL,
            // but it would still be nice to use a 2d array instead.

            byte[] frameColor = new byte[Size * Size * 4];

            int offset = 0;
            for (int i = 0; i < Size * Size; i++)
            {
                int sX = i % Size;
                int sy = i / Size;


                frameColor[offset] = elms[sX, sy].Color.R;
                frameColor[offset + 1] = elms[sX, sy].Color.G;
                frameColor[offset + 2] = elms[sX, sy].Color.B;
                frameColor[offset + 3] = elms[sX, sy].Color.A;
                offset += 4;
            }

            chunkTexture.Update(frameColor);
        }

        Sprite spr = new Sprite();

        public void Render(RenderTarget app, Vector2 pos, Vector2 scale)
        {
            if (chunkChanged) // prevent updating the texture if nothing has changed.
            {
                chunkChanged = false;
                UpdateTexture();
            }

            spr.Texture = chunkTexture;
            spr.Position = pos;
            spr.Scale = scale;
            app.Draw(spr);

            if (DebugRender) // this mess just draws the border of the chunk when DebugRender is true.
            {
                RectangleShape awakeShape = new RectangleShape(new Vector2(Size, Size));
                awakeShape.FillColor = Color.Transparent;
                awakeShape.Position = pos;
                awakeShape.Scale = scale;
                awakeShape.OutlineColor = Color.Green;
                awakeShape.OutlineThickness = -0.5f;

                if (sleeping)
                {
                    awakeShape.OutlineColor = Color.Red;
                }
                awakeShape.OutlineColor -= new Color(0, 0, 0, 200);
                app.Draw(awakeShape);
            }
        }

    }
}
