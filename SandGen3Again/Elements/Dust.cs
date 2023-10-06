using SandGen3Again.Scripts;
using SFML.Graphics;

namespace SandGen3Again.Elements
{
    internal class Dust : Element, IReaction
    {
        public override string Name { get; } = "Dust";
        public override Color Color { get; set; } = Color.White;
        public override float weight { get; set; } = 0.85f; // Floats on water.
        public Type With { get; set; } = typeof(Water); // Reacts with water, make sand!
        public Type To { get; set; } = typeof(Sand);
        public bool KeepReactive { get; set; } = false;
        public bool NeedsSurrouned { get; set; } = false;
        public float ReactionChance { get; set; } = 5f;

        public static ColorPalette colors = new ColorPalette(0xFFEC98FF, 0xFFF1A9FF, 0xFFF6B9FF);

        public Dust()
        {
            Color = colors.pickRandom();
            phsType = physicsType.Sand;
        }
    }
}
