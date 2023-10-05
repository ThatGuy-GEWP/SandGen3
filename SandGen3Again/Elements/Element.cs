using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandGen3Again.Elements
{
    public enum physicsType
    {
        None,
        Sand,
        Liquid
    }

    public abstract class Element
    {
        public abstract string Name { get; }

        public abstract Color Color { get; set; }

        public physicsType phsType { get; set; } = physicsType.None;


    }
}
