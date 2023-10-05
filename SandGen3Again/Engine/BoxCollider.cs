using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    public class BoxCollider : Collider, IRenderable
    {
        /*
         * A smart man would not do any of this
         * A smart man would have just used a physics library
         * 
         * but i, am not a smart man. i am lazy, and all box2D.Net bindings suck.
         */


        public Vector2 size
        {
            get { return _size; }
            set
            {
                if (value != _size)
                {
                    CreateBoxOfSize(value);
                }
                _size = value;
            }
        }
        Vector2 _size;

        Vector2[] verts;

        Line[] colliderBox = new Line[4];

        public static bool DebugDraw = false;

        public sbyte zOrder { get; set; } = 0;

        public BoxCollider(Vector2 size)
        {
            _size = size;
        }

        public Line[] GetCollider()
        {
            return colliderBox;
        }

        void CreateBoxOfSize(Vector2 size)
        {
            verts = new Vector2[4];

            verts[0] = new Vector2(-size.x, -size.y);
            verts[1] = new Vector2(size.x, -size.y);
            verts[2] = new Vector2(size.x, size.y);
            verts[3] = new Vector2(-size.x, size.y);

            for (int i = 0; i < 4; i++)
            {
                verts[i] = Vector2.Rotate(verts[i], gameObject.rotation);
            }

            Rebuild();
        }

        public override void OnAdded()
        {
            gameObject.PositionChanged += (gm) =>
            {
                Rebuild();
            };

            gameObject.RotationChanged += (gm) =>
            {
                CreateBoxOfSize(size);
            };
        }

        //Final "Make sure everything is where it needs to be" checks.
        public override void Start()
        {
            CreateBoxOfSize(size);
            Rebuild();
        }

        public override void Update()
        {
            if(!DebugDraw) { return; }
            gameObject.scene.renderManager.AddToRenderQueue(this);
        }

        protected override void Rebuild()
        {
            if (verts == null) { CreateBoxOfSize(size); }
            colliderBox = new Line[4];
            for (int i = 0; i < verts.Length; i++)
            {
                Line thisVert;

                if (i != verts.Length - 1)
                {
                    thisVert = new Line
                    {
                        startPos = verts[i] + gameObject.position,
                        endPos = verts[i + 1] + gameObject.position,
                    };
                }
                else
                {
                    thisVert = new Line
                    {
                        startPos = verts[i] + gameObject.position,
                        endPos = verts[0] + gameObject.position,
                    };
                }
                colliderBox[i] = thisVert;
            }
        }

        /// <summary>
        /// Returns a list of <see cref="Contact"/>'s that this collider is touching, only checks colliders in the <paramref name="withColliders"/> array.
        /// </summary>
        /// <param name="withColliders"></param>
        /// <returns></returns>
        public List<Contact> GetContacts(Collider[] withColliders)
        {
            List<Contact> contactPoints = new List<Contact>();

            foreach (Collider collider in withColliders)
            {
                if (collider != this && collider != null)
                {
                    if (collider is BoxCollider)
                    {
                        BoxCollider boxCollider = collider as BoxCollider;
                        foreach (Line otherLine in boxCollider.GetCollider())
                        {
                            for (int i = 0; i < colliderBox.Length; i++)
                            {
                                Vector2? col = LineLine(colliderBox[i], otherLine);
                                if (!(col is null))
                                {
                                    contactPoints.Add(new Contact { point = col, colliderA = this, colliderB = boxCollider });
                                }
                            }
                        }
                    }
                }
            }
            return contactPoints;
        }

        public void Draw(RenderTarget rt)
        {
            Vertex[] vertices = new Vertex[]
            {
                new Vertex(colliderBox[0].startPos, Color.Green),
                new Vertex(colliderBox[0].endPos, Color.Green),
                new Vertex(colliderBox[1].startPos, Color.Green),
                new Vertex(colliderBox[1].endPos, Color.Green),
                new Vertex(colliderBox[2].startPos, Color.Green),
                new Vertex(colliderBox[2].endPos, Color.Green),
                new Vertex(colliderBox[3].startPos, Color.Green),
                new Vertex(colliderBox[3].endPos, Color.Green),
            };

            rt.Draw(vertices, PrimitiveType.Lines);
        }


    }
}
