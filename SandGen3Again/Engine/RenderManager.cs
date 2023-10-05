using SFML.Graphics;

namespace SFML_Game_Engine
{
    /// <summary>
    /// Top level class that handles all rendering calls.
    /// allows for stuff like Z-Ordering and such.
    /// </summary>
    public class RenderManager
    {
        /// <summary>
        /// A List of <see cref="IRenderable"/>'s to be rendered next frame, Cleared after every <see cref="Render(RenderWindow)"/>
        /// </summary>
        List<IRenderable> renderQueue = new List<IRenderable>();

        List<(Vector2 position, float radius)> pointsQueue = new List<(Vector2 position, float radius)>();

        public RenderManager() { }

        CircleShape pointShape = new CircleShape(32);

        /// <summary>
        /// Adds an <see cref="IRenderable"/> to the renderQueue
        /// </summary>
        /// <param name="renderableComponent"></param>
        public void AddToRenderQueue(IRenderable renderableComponent)
        {
            renderQueue.Add(renderableComponent);
        }

        public void DrawPoint(Vector2 position, float Size)
        {
            pointsQueue.Add((position, Size));
        }

        /// <summary>
        /// Renders all <see cref="IRenderable"/>'s after sorting them by zOrder,
        /// then clears the render queue.
        /// </summary>
        /// <param name="target"></param>
        public void Render(RenderTarget target)
        {
            if (renderQueue.Count > 0)
            {
                renderQueue.Sort((x, y) => { return x.zOrder - y.zOrder; });

                for (int i = 0; i < renderQueue.Count; i++)
                {
                    renderQueue[i].Draw(target);
                }

                renderQueue.Clear();
            }

            if (pointsQueue.Count > 0)
            {
                for (int i = 0; i < pointsQueue.Count; ++i)
                {
                    pointShape.Origin = new SFML.System.Vector2f(pointsQueue[i].radius, pointsQueue[i].radius);
                    pointShape.Position = pointsQueue[i].position;
                    pointShape.Radius = pointsQueue[i].radius;
                    target.Draw(pointShape);
                }

                pointsQueue.Clear();
            }
        }
    }
}
