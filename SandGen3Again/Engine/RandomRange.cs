using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A struct for representing a random number range.<para></para>
    /// good for properties,
    /// bad for one time uses, use <see cref="RandomGen.Next(int, int)"/> or <see cref="RandomGen.Next(float, float)"/> instead <para></para>
    /// <seealso cref="Random.Next(int, int)"/>
    /// Example:
    /// <code>
    /// RandomRange rand = RandomRange{min = -20, max = 20}
    /// rand.Value // will return a new random number between -20 and 20 every time .Value is used.
    /// </code>
    /// </summary>
    public class RandomRange
    {
        /// <summary>
        /// The minimum value of this random range
        /// </summary>
        public float min = 0;
        /// <summary>
        /// The maximum value of this random range
        /// </summary>
        public float max = 0;

        public RandomRange()
        {

        }

        /// <summary>
        /// Creates a random range that will always return <paramref name="num"/>
        /// great for if you dont actually want a random range but want the option for one
        /// </summary>
        /// <param name="num"></param>
        public RandomRange(float num)
        {
            min = num;
            max = num;
        }

        /// <summary>
        /// Creates a random range from <paramref name="min"/> to <paramref name="max"/>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public RandomRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Returns a new random float from <see cref="min"/> to <see cref="max"/>
        /// </summary>
        public float Value
        {
            get
            {
                if (min == max) { return min; }
                return RandomGen.Next(min, max);
            }

        }
    }
}
