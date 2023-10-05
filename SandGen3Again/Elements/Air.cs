using SFML.Graphics;

namespace SandGen3Again.Elements
{
    public class Air : Element
    {
        public override string Name => "Air";
        public override float weight { get; set; } = 0;
        public override Color Color { get; set; } = Color.Transparent;
    }
}
