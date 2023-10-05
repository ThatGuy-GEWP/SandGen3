using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    /// <summary>
    /// Add to your components to enable custom drawing logic.
    /// <para>In order for the <see cref="Draw(RenderTarget)"/> command to work, 
    /// you will have to add your component to the current scene's <see cref="RenderManager"/> every frame,
    /// example below
    /// <code>gameObject.scene.renderManager.renderQueue.AddToRenderQueue(this)</code>
    /// </para>
    /// </summary>
    public interface IRenderable
    {
        public sbyte zOrder { get; set; }

        public void Draw(RenderTarget rt);
    }
}
