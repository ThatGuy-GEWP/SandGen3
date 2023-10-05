using SFML.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A class for drawing rectangles.<para></para>
    /// </summary>
    internal class RenderRect : Component, IRenderable
    {
        public Vector2 size = new Vector2(50, 50);

        public Vector2 origin = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// The fill color of this rectangle.
        /// </summary>
        public Color fillColor = Color.White;

        /// <summary>
        /// The color of the outline for this rectangle.
        /// </summary>
        public Color outlineColor = Color.White;

        /// <summary>
        /// The outline thickness of this rectangle.
        /// </summary>
        public float outlineThickness = 0;

        /// <summary>
        /// The texture of this rectangle, when set the texture will be stretched to this rects <see cref="size"/>, defaults to a white texture.
        /// </summary>
        public TextureResource texture;

        RectangleShape shape = new RectangleShape();

        public sbyte zOrder { get; set; } = 5;

        public RenderRect(Vector2 size) { this.size = size; }

        public override void OnAdded()
        {
            shape.Position = gameObject.localPosition;
            shape.Size = size;
            shape.Origin = size * origin;
            if (texture == null)
            {
                texture = gameObject.scene.resources.GetResourceByName<TextureResource>("DefaultSprite");
            }
        }

        public override void Update()
        {
            gameObject.scene.renderManager.AddToRenderQueue(this);
        }

        public void Draw(RenderTarget rt)
        {
            shape.Texture = texture;
            shape.Position = gameObject.localPosition;
            shape.Size = size;
            shape.Origin = size * origin;
            shape.FillColor = fillColor;
            shape.OutlineColor = outlineColor;
            shape.OutlineThickness = outlineThickness;
            shape.Rotation = gameObject.rotation;
            rt.Draw(shape);
        }
    }
}
