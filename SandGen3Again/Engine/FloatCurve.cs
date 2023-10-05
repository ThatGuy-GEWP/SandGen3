using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    public struct CurvePoint
    {
        public float tPos;
        public float value;
    }


    /// <summary>
    /// A float curve made from a <see cref="CurvePoint"/> array
    /// <code>
    /// // when moving between points, linear interpolation will be applied
    /// CurvePoint[] hold = new CurvePoint[] {
    ///     new CurvePoint{tPos = 0.0f, value = 255}, // at 0.0f the result of GetValue(time) should be 255
    ///     new CurvePoint{tPos = 0.5f, value = 0}, // at 0.5f the result of GetValue(time) should be 0
    ///     new CurvePoint{tPos = 1.0f, value = 255} // at the result of GetValue(time) should be 255
    /// }
    /// </code>
    /// </summary>
    public class FloatCurve
    {
        public CurvePoint[] points;

        /// <summary>
        /// Creates a float curve with a <see cref="CurvePoint"/> array,
        /// </summary>
        /// <param name="points"></param>
        public FloatCurve(CurvePoint[] points)
        {
            this.points = points;
        }

        /// <summary>
        /// Creates a simple float curve that linearly interpolates from min to max.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public FloatCurve(float min, float max)
        {
            points = new CurvePoint[]
            {
                new CurvePoint{tPos=0.0f, value=min},
                new CurvePoint{tPos=1.0f, value=max},
            };
        }

        /// <summary>
        /// Calculates the value of the curve at T, T will be clamped between 0.0f - 1.0f
        /// </summary>
        /// <param name="T"></param>
        /// <returns><see cref="float"/></returns>
        public float GetValue(float T)
        {
            T = Math.Clamp(T, 0, 1);

            int nearest = 0;
            for (int i = 0; i < points.Length; i++)
            {
                if (i + 1 >= points.Length) // we are at last point in list
                {
                    nearest = i - 1; // set to second to last so we can interp to last
                    break;
                }

                if (T > points[i].tPos && T < points[i + 1].tPos) // we are not last point in list
                {
                    nearest = i; // lets interpolate to the next point then!
                    break;
                }
            }


            if (nearest + 1 == points.Length - 1 && points[nearest + 1].tPos < 1.0f)
            {
                if (T >= points[nearest + 1].tPos)
                {
                    T = 1f;
                    return MathGE.Map(T, 0f, 1f, points[nearest].value, points[nearest + 1].value);
                }
            }

            T = MathGE.Map(T, points[nearest].tPos, points[nearest + 1].tPos, 0, 1);

            return MathGE.Map(T, 0f, 1f, points[nearest].value, points[nearest + 1].value);
        }
    }
}
