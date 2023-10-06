using SFML.Graphics;

namespace SandGen3Again.Elements
{
    internal class Metal : Element
    {
        public override string Name { get; } = "Metal";
        public override Color Color { get; set; } = new Color(135, 135, 155);
        public override float weight { get; set; } = 5f;

        public Metal() 
        {
            phsType = physicsType.Rigid;
        }
    }
}
