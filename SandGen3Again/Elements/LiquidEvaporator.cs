using SandGen3Again.Scripts;
using SFML.Graphics;

namespace SandGen3Again.Elements
{
    /// <summary>
    /// An example of how to use <see cref="ITickable"/>, turns water into sand.
    /// </summary>
    public class LiquidEvaporator : Element, ITickable
    {
        public override string Name => "Liquid Evaporator";
        public override Color Color { get; set; } = Color.White;
        public override float weight { get; set; } = 10;

        public void OnTick(int worldX, int worldY, World world)
        {
            if(world.GetElement(worldX, worldY+1) is Water)
            {
                world.SetElement(worldX, worldY+1, new Sand());
            }
        }
    }
}
