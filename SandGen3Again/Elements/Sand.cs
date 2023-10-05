using SFML.Graphics;
using SFML_Game_Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandGen3Again.Elements
{
    public class Sand : Element
    {
        public override string Name => "Sand";

        public override Color Color { get; set; } = Color.Yellow;

        public Sand()
        {
            phsType = physicsType.Sand;
            Color += new Color();
        }
    }
}
