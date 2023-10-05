using SFML.Graphics;

namespace SandGen3Again.Elements
{
    public class Air : Element
    {
        public override string Name => "Air";
        public override Color Color { get; set; } = Color.Transparent;
    }
}
