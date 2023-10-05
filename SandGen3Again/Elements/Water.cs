using SFML.Graphics;

namespace SandGen3Again.Elements
{
    internal class Water : Element
    {
        public override string Name => "Water";

        public override float weight { get; set; } = 1f;

        public override Color Color { get; set; } = new Color(0, 0, 255, 255);

        public Water() 
        {
            phsType = physicsType.Liquid;
        }
    }
}
