using SFML.Graphics;
using Sprite = SFML.Graphics.Sprite;

namespace SFML_Game_Engine.Engine
{
    /// <summary>
    /// Plays a animation at the set FPS, uses a <see cref="TextureResource[]"/> array to hold the frames of the animation.
    /// </summary>
    internal class AnimatedSequence : Component, IRenderable
    {
        Sprite sprite = new Sprite();
        TextureResource[] frames;

        public sbyte zOrder { get; set; } = 1;
        public Vector2 origin = new Vector2(0, 0);

        public bool loop = false;
        public bool isPlaying { get; private set; } = false;
        public bool reversed = false;
        public float fps = 30f;
        public byte alpha = 255;
        int curFrame = 0;

        float timePassed = 0;

        public override object Clone()
        {
            AnimatedSequence clone = new AnimatedSequence(frames); ;
            clone.zOrder = zOrder;
            clone.origin = origin;
            clone.loop = loop;
            clone.isPlaying = isPlaying;
            clone.reversed = reversed;
            clone.fps = fps;
            clone.alpha = alpha;
            return clone;
        }

        public AnimatedSequence(TextureResource[] frames)
        {
            this.frames = frames;
        }

        public void Reset()
        {
            curFrame = 0;
        }

        public void Play()
        {
            isPlaying = true;
        }

        public void Play(bool reversed)
        {
            this.reversed = reversed;
            isPlaying = true;
        }

        public override void Update()
        {
            gameObject.scene.renderManager.AddToRenderQueue(this);
        }

        public void Draw(RenderTarget rt)
        {
            sprite.Color = new Color(255, 255, 255, alpha);
            sprite.Position = gameObject.localPosition;
            sprite.Rotation = gameObject.rotation;
            if (isPlaying)
            {
                timePassed += gameObject.scene.deltaTime;
                if (timePassed >= 1f / fps) { curFrame++; timePassed = 0; }
                if (!loop)
                {
                    if (curFrame >= frames.Length) { curFrame = frames.Length - 1; isPlaying = false; }
                }
                else
                {
                    if (curFrame >= frames.Length) { curFrame = 0; }
                }
            }

            if (reversed)
            {
                sprite.Texture = frames[frames.Length - 1 - curFrame].resource;
            }
            else
            {
                sprite.Texture = frames[curFrame].resource;
            }
            sprite.Origin = (Vector2)sprite.Texture.Size * origin;
            rt.Draw(sprite);
        }

        public override void OnDestroy()
        {
            sprite.Dispose();
        }
    }
}
