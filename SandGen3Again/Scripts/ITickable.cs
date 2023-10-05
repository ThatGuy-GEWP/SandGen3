namespace SandGen3Again.Scripts
{
    /// <summary>
    /// An interface for elements, allowing them to have custom ticking logic.
    /// Elements using this will tick AFTER a physics update.
    /// <para></para>
    /// Chunks that contain ITickable's will never sleep.
    /// </summary>
    public interface ITickable
    {
        public abstract void OnTick(int worldX, int worldY, World world);
    }
}
