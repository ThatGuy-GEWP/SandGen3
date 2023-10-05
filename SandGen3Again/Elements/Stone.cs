using SandGen3Again.Scripts;
using SFML.Graphics;

namespace SandGen3Again.Elements
{
    public class Stone : Element
    {
        public override string Name => "Stone";
        public override Color Color { get; set; } = Color.Blue;

        public override float weight { get; set; } = 10;

        static ColorPalette colors = new ColorPalette(0x0F1116FF, 0x111318FF, 0x13151AFF, 0x1A1B22FF);

        public Stone()
        {
            phsType = physicsType.None;
            Color = colors.pickRandom();
        }
    }
}
