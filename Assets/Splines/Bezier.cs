using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spline
{
    public static class Bezier
    {
        public static Vector3 GetPoint(Vector3 point0, Vector3 point1, Vector3 point2, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * point0 +
                2f * oneMinusT * t * point1 +
                t * t * point2;
        }

        public static Vector3 GetFirstDerivative(Vector3 point0, Vector3 point1, Vector3 point2, float t)
        {
            return 2f * (1f - t) * (point1 - point0) + 2f * t * (point2 - point1);
        }

        public static Vector3 GetPoint(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * oneMinusT * point0 +
                3f * oneMinusT * oneMinusT * t * point1 +
                3f * oneMinusT * t * t * point2 +
                t * t * t * point3;
        }

        public static Vector3 GetFirstDerivative(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return 3f * oneMinusT * oneMinusT * (point1 - point0) +
                6f * oneMinusT * t * (point2 - point1) +
                3f * t * t * (point3 - point2);
        }

        public static Vector3 GetSecondDerivative(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3, float t)
        {
            return (6*t*(point1 + 3*(point2 - point3) - point0) + 6 * (point0 - 2*point2 + point3));
        }
    }
}
