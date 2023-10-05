using SFML.Graphics;

namespace SFML_Game_Engine
{
    internal class Particle
    {
        public Vector2 position;
        public Vector2 velocity;

        public float maxLifeTime;

        public float realLifeTime;

        public float rotation;

        public float rotationSpeed;

        // to not waste as much memory
        public static FloatCurve alphaDefault = new FloatCurve(new CurvePoint[]
        {
            new CurvePoint{tPos = 0.0f, value = 0.0f},
            new CurvePoint{tPos = 1.0f, value = 1.0f},
        });

        public FloatCurve alphaCurve = alphaDefault;

        public Particle(Vector2 position, Vector2 velocity, float lifeTime, float rotation, float rotationSpeed)
        {
            this.position = position;
            this.velocity = velocity;
            this.rotation = rotation;
            this.rotationSpeed = rotationSpeed;
            maxLifeTime = lifeTime;
            realLifeTime = maxLifeTime;
        }
    }

    public class ParticleSystem : Component, IRenderable
    {
        public sbyte zOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TextureResource particleSprite = null!;

        public int particleCount { get { return particles.Count; } }

        public int maxParticles = 200;

        /// <summary>
        /// Starting rotation for each particle + <see cref="rotationRandom"/>
        /// </summary>
        public float rotation = 0;

        /// <summary>
        /// Total lifetime in seconds of each particle + <see cref="lifeTimeRandom"/>
        /// </summary>
        public float particleLifetime = 1f;

        /// <summary>
        /// A <see cref="FloatCurve"/> defining the particles alpha value over its <see cref="particleLifetime"/>
        /// <para>In a range of 1.0 - 0.0, 1.0 meaning fully opaque, 0.0 meaning fully transparent</para>
        /// </summary>
        public FloatCurve particleAlpha = Particle.alphaDefault;

        /// <summary>
        /// A <see cref="RandomRange"/> added to each particles <see cref="particleLifetime"/>
        /// </summary>
        public RandomRange lifeTimeRandom = new RandomRange { min = -0.5f, max = 1.0f };

        /// <summary>
        /// Starting rotation for each particle.
        /// </summary>
        public RandomRange rotationRandom = new RandomRange { min = -5, max = 5 };

        /// <summary>
        /// Rotation speed per particle.
        /// </summary>
        public RandomRange rotationSpeed = new RandomRange { min = -50, max = 50 };

        public RandomRange velocityX = new RandomRange { min = -300, max = 300 };
        public RandomRange velocityY = new RandomRange { min = -300, max = 300 };

        public bool emit = true;

        /// <summary>
        /// Gravity for all particles.
        /// </summary>
        public Vector2 gravity = new Vector2(0, 500);

        /// <summary>
        /// Drag for all particles.
        /// </summary>
        public float drag = 0.03f;

        Sprite drawingSprite = new Sprite();

        List<Particle> particles = new List<Particle>();

        public override void OnAdded()
        {
            particleSprite = gameObject.scene.resources.GetResourceByName<TextureResource>("DefaultSprite");
        }

        Random random = new Random();

        public override void Update()
        {
            gameObject.scene.renderManager.AddToRenderQueue(this);

            if (emit && particleCount < maxParticles)
            {
                particles.Add(new Particle(
                    gameObject.position,
                    new Vector2(random.Next(-3000, 3000) * 0.1f, random.Next(-400, 0)),
                    particleLifetime + lifeTimeRandom.Value,
                    rotation + rotationRandom.Value,
                    rotationSpeed.Value
                    ));
                particles[particles.Count - 1].alphaCurve = particleAlpha;
            }
        }

        public void Draw(RenderTarget rt)
        {
            drawingSprite.Texture = particleSprite;
            drawingSprite.Origin = new Vector2(drawingSprite.Texture.Size.X / 2, drawingSprite.Texture.Size.Y / 2);
            drawingSprite.Scale = new Vector2(0.5f, 0.5f);

            for (int i = 0; i < particles.Count; i++)
            {
                Particle particle = particles[i];

                particle.realLifeTime -= gameObject.scene.deltaTime;

                particle.velocity -= particle.velocity * drag * gameObject.scene.deltaTime;
                particle.velocity += gravity * gameObject.scene.deltaTime;
                particle.position += particle.velocity * gameObject.scene.deltaTime;

                particle.rotation += particle.rotationSpeed * gameObject.scene.deltaTime;

                drawingSprite.Position = particle.position;
                drawingSprite.Color = new Color(255, 255, 255,
                    (byte)Math.Clamp(particle.alphaCurve.GetValue(1 - (particle.realLifeTime / particle.maxLifeTime)) * 255, 0, 255)

                    );
                drawingSprite.Rotation = particle.rotation;

                rt.Draw(drawingSprite);

                if (particle.realLifeTime <= 0)
                {
                    particles.RemoveAt(i);
                }
            }
        }


    }
}
