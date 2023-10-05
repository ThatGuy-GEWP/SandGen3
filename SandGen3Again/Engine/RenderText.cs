using SFML.Graphics;
using SFML_Game_Engine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A simple class for rendering text
    /// </summary>
    internal class RenderText : Component, IRenderable
    {
        static Font defaultFont = new Font("Engine\\Font\\Roboto-Light.ttf");
        public Font font = defaultFont;

        public Color color = Color.White;

        Text rtext;

        public string text { get; set; } = "";
        public uint size = 40;
        public Vector2 offset = new Vector2(0, 0);

        public RenderText(string Text)
        {
            text = Text;
            rtext = new Text(text, font);
        }

        public RenderText(string Text, Font font)
        {
            text = Text;
            this.font = font;
            rtext = new Text(text, font);
        }

        public sbyte zOrder { get; set; } = 0;

        public override void Update()
        {
            gameObject.scene.renderManager.AddToRenderQueue(this);
        }

        public void Draw(RenderTarget rt)
        {
            rtext.CharacterSize = size;
            rtext.FillColor = color;
            rtext.Rotation = gameObject.rotation;
            rtext.Position = gameObject.localPosition + offset;
            rtext.DisplayedString = text;
            rt.Draw(rtext);
        }
    }
}
