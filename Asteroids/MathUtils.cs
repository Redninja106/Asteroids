using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;

namespace Asteroids
{
    internal class MathUtils
    {
        public static bool Matrix3x2Invert(Matrix3x2 matrix, out Matrix3x2 result)
        {
            float det = (matrix.M11 * matrix.M22) - (matrix.M21 * matrix.M12);

            if (MathF.Abs(det) < float.Epsilon)
            {
                result = new Matrix3x2(float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
                return false;
            }

            float invDet = 1.0f / det;

            result.M11 = matrix.M22 * invDet;
            result.M12 = -matrix.M12 * invDet;

            result.M21 = -matrix.M21 * invDet;
            result.M22 = matrix.M11 * invDet;

            result.M31 = (matrix.M21 * matrix.M32 - matrix.M31 * matrix.M22) * invDet;
            result.M32 = (matrix.M31 * matrix.M12 - matrix.M11 * matrix.M32) * invDet;

            return true;
        }
    }
}