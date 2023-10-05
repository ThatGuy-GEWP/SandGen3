using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    /// <summary>
    /// Thread safe random, stolen shamelessly from <seealso href="https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/">this</seealso>
    /// </summary>
    internal static class RandomGen
    {
        private static Random _global = new Random(
            DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);

        [ThreadStatic]
        private static Random _local;

        static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        public static int Next(int low, int high)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next(low, high);
        }

        public static int Next(int high)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next(0, high);
        }

        public static float Next(float low, float high)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return Map((float)inst.NextDouble(), 0.0f, 1.0f, low, high);
        }

        public static float Next(float high)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return (float)(inst.NextDouble() * high);
        }
    }
}
