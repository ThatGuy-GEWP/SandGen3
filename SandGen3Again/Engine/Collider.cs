

namespace SFML_Game_Engine
{
    public struct Line
    {
        public Vector2 startPos;
        public Vector2 endPos;
    }

    public struct Contact
    {
        public Vector2 point;
        public Collider colliderA;
        public Collider colliderB;
    }

    /// <summary>
    /// A Collider, colliders functions differ from collider to collider but generaly
    /// it lets you know if, and where other colliders are overlapping.
    /// </summary>
    public abstract class Collider : Component
    {
        /// <summary>
        /// Requests a rebuild of the collider.
        /// </summary>
        protected abstract void Rebuild();

        public float angularVel = 0;

        // From Jeffery Thompsons amazing book http://www.jeffreythompson.org/collision-detection/
        /// <summary>
        /// Used to get the point of contact between two lines.
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="lineB"></param>
        /// <returns>The point of contact if two lines touch, <c>null</c> otherwise</returns>
        protected static Vector2? LineLine(Line lineA, Line lineB)
        {
            float uA = ((lineB.endPos.x - lineB.startPos.x) * (lineA.startPos.y - lineB.startPos.y) - (lineB.endPos.y - lineB.startPos.y) * (lineA.startPos.x - lineB.startPos.x)) / ((lineB.endPos.y - lineB.startPos.y) * (lineA.endPos.x - lineA.startPos.x) - (lineB.endPos.x - lineB.startPos.x) * (lineA.endPos.y - lineA.startPos.y));
            float uB = ((lineA.endPos.x - lineA.startPos.x) * (lineA.startPos.y - lineB.startPos.y) - (lineA.endPos.y - lineA.startPos.y) * (lineA.startPos.x - lineB.startPos.x)) / ((lineB.endPos.y - lineB.startPos.y) * (lineA.endPos.x - lineA.startPos.x) - (lineB.endPos.x - lineB.startPos.x) * (lineA.endPos.y - lineA.startPos.y));
            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                float intersectionX = lineA.startPos.x + (uA * (lineA.endPos.x - lineA.startPos.x));
                float intersectionY = lineA.startPos.y + (uA * (lineA.endPos.y - lineA.startPos.y));
                return new Vector2(intersectionX, intersectionY);
            }
            return null;
        }
    }
}
