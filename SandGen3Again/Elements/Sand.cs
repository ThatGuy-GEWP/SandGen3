using SandGen3Again.Scripts;
using SFML.Graphics;

namespace SandGen3Again.Elements
{
    public class Sand : Element
    {
        public override string Name => "Sand";

        public override Color Color { get; set; } = Color.Yellow;

        public override float weight { get; set; } = 1.2f;

        static ColorPalette colors = new ColorPalette(0xFEEF5AFF, 0xDCC243FF, 0xCBAB38FF);

        public Sand()
        {
            phsType = physicsType.Sand;
            Color = colors.pickRandom();
        }
    }
}
