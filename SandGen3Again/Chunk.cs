using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandGen3Again.Elements;
using SFML.Graphics;
using SFML_Game_Engine;

namespace SandGen3Again
{
    public enum ChangeType
    {
        None,
        Set,
        Remove
    }

    public struct Change
    {
        public ChangeType type;
        public int lx;
        public int ly;
        public Element elm;
    }

    public class Chunk
    {
        public const int Size = 64; // size of chunk ^2

        public Element[,] elms = new Element[Size, Size];
        public Change[,] changes = new Change[Size, Size];
        public Texture chunkTexture = new Texture(Size, Size);

        public bool sleeping = false;

        bool chunkChanged = false;

        public Chunk()
        {
            for(int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    //elms[x, y] = new Air();
                    int rng = RandomGen.Next(0, 2);
                    if (rng == 0)
                    {
                        elms[x, y] = new Air();
                    }
                    else
                    {
                        elms[x, y] = new Sand();
                    }
                }
            }
            chunkChanged = true;
            sleeping = false;
        }

        public Element GetElement(int x, int y)
        {
            return elms[x, y];
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
                if(RandomGen.Next(10) > 5)
                {
                    changes[x, y] = new Change { elm = elm, lx = x, ly = y, type = ChangeType.Set };
                    return;
                }
            }
            changes[x, y] = new Change { elm = elm, lx = x, ly = y, type = ChangeType.Set };
        }

        public void Awake()
        {
            chunkChanged = true;
            sleeping = false;
        }

        public void ProcessChanges()
        {
            if (sleeping) return;

            int changesProcessed = 0;

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Change curChange = changes[x, y];
                    if (curChange.type == ChangeType.None) { continue; }

                    if (curChange.type == ChangeType.Set)
                    {
                        if(curChange.elm is null)
                        {
                            Console.WriteLine("IDFK WHY ITS NULL");
                        }
                        lock (curChange.elm)
                        {
                            elms[x, y] = curChange.elm;
                        }
                    }

                    if (curChange.type == ChangeType.Remove)
                    {
                        elms[x, y] = new Air();
                    }

                    changesProcessed++;
                    changes[x, y] = new Change { type = ChangeType.None };
                }
            }

            if (changesProcessed == 0)
            {
                sleeping = true;
            }

        }

        public void UpdateTexture()
        {
            byte[] frameColor = new byte[Size * Size * 4];

            int offset = 0;
            for(int i = 0; i < Size*Size; i++)
            {
                int sX = i % Size;
                int sy = i / Size;


                frameColor[offset] = elms[sX, sy].Color.R;
                frameColor[offset+1] = elms[sX, sy].Color.G;
                frameColor[offset+2] = elms[sX, sy].Color.B;
                frameColor[offset + 3] = elms[sX, sy].Color.A;
                offset += 4;
            }

            chunkTexture.Update(frameColor);
        }

        Sprite spr = new Sprite();

        public void Render(RenderWindow app, Vector2 pos, Vector2 scale)
        {

            if (chunkChanged)
            {
                UpdateTexture();
            }

            spr.Texture = chunkTexture;
            spr.Position = pos;
            spr.Scale = scale;
            app.Draw(spr);
        }

    }
}
