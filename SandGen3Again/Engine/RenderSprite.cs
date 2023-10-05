using SFML.Graphics;
using SFML_Game_Engine.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    /// <summary>
    /// Renders a sprite, if you would like to set the sprites specific size, use <see cref="RenderRect.texture"/> instead.
    /// </summary>
    public class RenderSprite : Component, IRenderable
    {
        public TextureResource renderTexture;
        public Vector2 offset = new Vector2(0, 0);
        public Vector2 origin = new Vector2(0, 0);
        public Vector2 scale = new Vector2(1, 1);
        Sprite sprite;

        public RenderSprite(TextureResource texture)
        {
            renderTexture = texture;
            sprite = new Sprite(renderTexture.resource);
        }

        public sbyte zOrder { get; set; } = 0;

        public override void Update()
        {
            gameObject.scene.renderManager.AddToRenderQueue(this);
        }

        public void Draw(RenderTarget rt)
        {
            sprite.Scale = scale;
            sprite.Position = gameObject.localPosition + offset;
            sprite.Texture = renderTexture.resource;
            sprite.Origin = ((Vector2)renderTexture.resource.Size * scale) * origin;
            sprite.Rotation = gameObject.rotation;
            rt.Draw(sprite);
        }

        public override void OnDestroy()
        {
            sprite.Dispose();
        }
    }
}
