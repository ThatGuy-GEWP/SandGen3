using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    /// <summary>
    /// Base class for all Resources.
    /// </summary>
    public abstract class Resource : IDisposable
    {
        public string name { get; protected set; } = null!;
        public string? Description { get; protected set; }
        public abstract void Dispose();
    }
}
