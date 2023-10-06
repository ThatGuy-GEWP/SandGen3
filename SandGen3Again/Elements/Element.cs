using SFML.Graphics;

namespace SandGen3Again.Elements
{
    /// <summary>
    /// 
    /// </summary>
    public enum physicsType
    {
        /// <summary>
        /// None, element will not move
        /// </summary>
        None,
        /// <summary>
        /// Sand-like, elements will fall down, then go to the bottom left and right randomly while sinking below lighter elements
        /// </summary>
        Sand,
        /// <summary>
        /// Liquid, elements will fall down, then move left and right randomly, tends to even out over time.
        /// </summary>
        Liquid,
        /// <summary>
        /// Rigid, elements will only fall down, will still pass through lighter elements
        /// </summary>
        Rigid
    }

    public abstract class Element
    {
        public abstract string Name { get; }

        public abstract Color Color { get; set; }

        public abstract float weight { get; set; }

        public physicsType phsType { get; set; } = physicsType.None;


    }
}
