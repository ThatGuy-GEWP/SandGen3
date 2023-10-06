using SandGen3Again.Elements;

namespace SandGen3Again.Scripts
{
    /// <summary>
    /// Allows an element to react with another element, replacing it with another.
    /// </summary>
    internal interface IReaction
    {
        /// <summary>
        /// What this element reacts with.
        /// </summary>
        public abstract Type With { get; set; }
        /// <summary>
        /// What this element turns into when it reacts.
        /// </summary>
        public abstract Type To { get; set; }

        /// <summary>
        /// If true, reactions will not remove the <see cref="With"/> element.
        /// </summary>
        public abstract bool KeepReactive { get; set; }

        /// <summary>
        /// If true, reactions will require being surrounded by the <see cref="With"/> element to react.
        /// </summary>
        public abstract bool NeedsSurrouned { get; set;}

        /// <summary>
        /// Chance from 0.0f-100.0f of this element reacting with nearby elements.
        /// At 0, the element will not react, and at 100 it will always react.
        /// <para/>With high tickrates, you may have to set the chance very low.
        /// </summary>
        public abstract float ReactionChance { get; set; }
    }
}
