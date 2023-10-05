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

    public struct Change
    {
        public ChangeType type;
        public int lx;
        public int ly;
        public Element elm;

        public int authorHash; // used to make sure what we are swapping with is still there!

        // for use with "Swap"
        public Chunk fromChunk;

        public void Recycle()
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

        public bool sleeping = false;

        bool chunkChanged = false;

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

        public void SwapElement(int thisX, int thisY, Chunk from, int fromX, int fromY)
        {
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

        public void RemoveElement(int x, int y)
        {
            if (sleeping) { Awake(); }
            changes[x, y] = new Change { lx = x, ly = y, type = ChangeType.Remove };
        }

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

        public void SetElementNow(int x, int y, Element elm)
        {
            changes[x, y].Recycle();
            elms[x, y] = elm;
        }

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
                    if (elms[x, y] is ITickable) { hastickable = true; }
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


        public static bool DebugRender = false;
        public void Render(RenderTarget app, Vector2 pos, Vector2 scale)
        {
            if (chunkChanged)
            {
                UpdateTexture();
            }

            spr.Texture = chunkTexture;
            spr.Position = pos;
            spr.Scale = scale;
            app.Draw(spr);

            if (DebugRender)
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
