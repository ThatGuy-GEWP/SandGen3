using SFML.Graphics;
using SFML_Game_Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    internal class RenderCircle : Component, IRenderable
    {
        public sbyte zOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Draw(RenderTarget rt)
        {
            throw new NotImplementedException();
        }
    }
}
