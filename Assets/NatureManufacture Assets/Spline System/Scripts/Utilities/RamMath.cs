// /**
//  * Created by Pawel Homenko on  07/2022
//  */

using UnityEngine;

namespace NatureManufacture.RAM
{
    public static class RamMath
    {
        private const float Epsilon = 0.00001f;

        public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return Vector3.Distance(ProjectPointLine(point, lineStart, lineEnd), point);
        }

        public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd, out Vector3 closestPoint)
        {
            closestPoint = ProjectPointLine(point, lineStart, lineEnd);
            return Vector3.Distance(closestPoint, point);
        }

        public static float DistancePointLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            return Vector3.Distance(ProjectPointLine(point, lineStart, lineEnd), point);
        }

        public static float DistancePointLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd, out Vector2 closestPoint)
        {
            closestPoint = ProjectPointLine(point, lineStart, lineEnd);
            return Vector3.Distance(closestPoint, point);
        }

        public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 rhs = point - lineStart;
            Vector3 vector2 = lineEnd - lineStart;
            float magnitude = vector2.magnitude;
            Vector3 lhs = vector2;
            if (magnitude > 1E-06f) lhs = lhs / magnitude;

            float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
            return lineStart + lhs * num2;
        }

        public static Vector3 CalculateNormal(Vector3 tangent, Vector3 up)
        {
            Vector3 binormal = Vector3.Cross(up, tangent);
            return Vector3.Cross(tangent, binormal).normalized;
        }

        public static bool AreLinesIntersecting(Vector3 l1P1, Vector3 l1P2, Vector3 l2P1, Vector3 l2P2,
            bool shouldIncludeEndPoints = true)
        {
            bool isIntersecting = false;

            float denominator = (l2P2.z - l2P1.z) * (l1P2.x - l1P1.x) - (l2P2.x - l2P1.x) * (l1P2.z - l1P1.z);

            //Make sure the denominator is > 0, if not the lines are parallel
            if (denominator != 0f)
            {
                float uA = ((l2P2.x - l2P1.x) * (l1P1.z - l2P1.z) - (l2P2.z - l2P1.z) * (l1P1.x - l2P1.x)) /
                           denominator;
                float uB = ((l1P2.x - l1P1.x) * (l1P1.z - l2P1.z) - (l1P2.z - l1P1.z) * (l1P1.x - l2P1.x)) /
                           denominator;

                //Are the line segments intersecting if the end points are the same
                if (shouldIncludeEndPoints)
                {
                    //Is intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                    if (uA >= 0f + Epsilon && uA <= 1f - Epsilon && uB >= 0f + Epsilon && uB <= 1f - Epsilon) isIntersecting = true;
                }
                else
                {
                    //Is intersecting if u_a and u_b are between 0 and 1
                    if (uA > 0f + Epsilon && uA < 1f - Epsilon && uB > 0f + Epsilon && uB < 1f - Epsilon) isIntersecting = true;
                }
            }

            return isIntersecting;
        }
        
        public static float Remap(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
        
        public static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            Vector3 pos = 0.5f * (a + b * t + c * (t * t) + d * (t * t * t));

            return pos;
        }

        public static Vector3 GetCatmullRomTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return 0.5f * (-p0 + p2 + (2f * p0 - 5f * p1 + 4f * p2 - p3) * (2f * t) +
                           (-p0 + 3f * p1 - 3f * p2 + p3) * (3f * t * t));
        }
    }
}