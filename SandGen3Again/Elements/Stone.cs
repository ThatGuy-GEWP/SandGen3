using SFML.Graphics;

namespace SandGen3Again.Elements
{
    public class Stone : Element
    {
        public override string Name => "Stone";
        public override Color Color { get; set; } = Color.Blue;

        public Stone()
        {
            phsType = physicsType.None;
        }
    }
}
