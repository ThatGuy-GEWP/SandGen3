using SFML.Graphics;
using SFML.Window;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A trigger that can sense when the mouse interacts with it, great for UI or ingame buttons
    /// </summary>
    internal class MouseTrigger : Component, IRenderable
    {
        public Vector2 Offset; // relative to gameObject of course.
        public Vector2 Size; // not scale!, absolute size of this mouse trigger
        public Vector2 Origin; // origin is same as everying else, (0.0,0.0) would be the top left of this box, (1.0,1.0) would be bottom right

        /// <summary>
        /// If true, every MouseTrigger that is enabled will be drawn.
        /// </summary>
        public static bool debugDraw = false;


        /// <summary>
        /// Target button of this trigger.
        /// </summary>
        public Mouse.Button Button = Mouse.Button.Left;

        /// <summary>
        /// True if mouse is held and hovering over this trigger.
        /// </summary>
        public bool mouseHeld = false;
        /// <summary>
        /// True for a single frame when this trigger is clicked with the selected <see cref="Button"/>
        /// </summary>
        public bool mousePressed = false;
        /// <summary>
        /// True if mouse is hovering over this trigger.
        /// </summary>
        public bool mouseHovering = false;

        bool mousePressedReset = false; // used internaly, do not remove

        public sbyte zOrder { get; set; } = 5;

        /// <summary>
        /// Called once every time the trigger is clicked. Passes the current <see cref="MouseTrigger"/>
        /// </summary>
        public event Action<MouseTrigger> OnClick;

        public override object Clone()
        {
            MouseTrigger clone = new MouseTrigger(Size, Origin, Offset);
            clone.enabled = enabled;
            clone.zOrder = zOrder;
            return clone;
        }

        public MouseTrigger(Vector2 size, Vector2 origin, Vector2 offset)
        {
            Offset = offset;
            Size = size;
            Origin = origin;
        }

        public MouseTrigger(Vector2 size, Vector2 origin)
        {
            Offset = new Vector2(0, 0);
            Size = size;
            Origin = origin;
        }

        public MouseTrigger(Vector2 size)
        {
            Offset = new Vector2(0, 0);
            Size = size;
            Origin = new Vector2(0, 0);
        }

        public override void Update()
        {
            if (debugDraw)
            {
                gameObject.scene.renderManager.AddToRenderQueue(this);
            }

            Vector2 realPosition = gameObject.localPosition + Offset;

            Vector2 upperBound = realPosition;
            Vector2 lowerBound = realPosition + Size;

            upperBound -= Size * Origin;
            lowerBound -= Size * Origin;

            Vector2 mousePos = gameObject.scene.GetMousePosition();

            if (mousePressed) { mousePressed = false; }
            mouseHeld = false;

            if (Mouse.IsButtonPressed(Button) && mouseHovering == false)
            {
                return;
            }

            if (mousePos.x > upperBound.x && mousePos.y > upperBound.y && mousePos.x < lowerBound.x && mousePos.y < lowerBound.y)
            {
                mouseHovering = true;
                mouseHeld = Mouse.IsButtonPressed(Button);
            }
            else
            {
                mouseHovering = false;
            }

            if (Mouse.IsButtonPressed(Button) && !mousePressedReset)
            {
                mousePressed = true;
                mousePressedReset = true;
                OnClick(this);
            }

            if (!Mouse.IsButtonPressed(Button))
            {
                mousePressedReset = false;
            }
        }

        public void Draw(RenderTarget rt)
        {
            Vector2 worldPosition = gameObject.localPosition + Offset;
            RectangleShape shape = new RectangleShape(Size);
            shape.FillColor = new Color(255, 255, 255, 0);
            shape.OutlineColor = new Color(0, 255, 0, 255);
            shape.OutlineThickness = -1f;
            shape.Position = worldPosition;
            shape.Origin = Size * Origin;
            rt.Draw(shape);
        }
    }
}
