// /**
//  * Created by Pawel Homenko on  05/2023
//  */

using System.Collections.Generic;
using UnityEngine;

namespace NatureManufacture.RAM
{
    public class NmSplinePointSearcher
    {
        private NmSpline _nmSpline;
        private NmSplinePoint[] _pointsArray;
        private readonly Dictionary<float, NmSplinePoint> _positions = new();

        public NmSplinePointSearcher(NmSpline nmSpline)
        {
            _nmSpline = nmSpline;
        }

        public NmSplinePoint[] PointsArray
        {
            get => _pointsArray;
            set => _pointsArray = value;
        }

        public Dictionary<float, NmSplinePoint> Positions => _positions;

        public NmSplinePoint FindPosition(float lengthToFind, int searchFrom, out int lastID)
        {
            if (PointsArray == null || PointsArray.Length == 0)
            {
                Debug.LogError("No points in array to search. Use GenerateArrayForDistanceSearch to generate array");
                lastID = 0;
                return new NmSplinePoint();
            }


            if (_nmSpline.IsLooping)
            {
                lengthToFind %= _nmSpline.Length;
            }


            if (Positions.TryGetValue(lengthToFind, out var newSplinePoint))
            {
                //Debug.Log($"dict found");
                lastID = newSplinePoint.ID;
                return newSplinePoint;
            }

            newSplinePoint = new NmSplinePoint();


            for (int i = searchFrom; i < PointsArray.Length - 1; i++)
            {
                NmSplinePoint splinePointFirst = PointsArray[i];
                NmSplinePoint splinePoint = PointsArray[i + 1];

                if (!(splinePointFirst.Distance <= lengthToFind) || !(lengthToFind < splinePoint.Distance)) continue;

                float distance = splinePoint.Distance - splinePointFirst.Distance;

                float distanceBasePoint = lengthToFind - splinePointFirst.Distance;
                float lerpValue = distanceBasePoint / distance;
                newSplinePoint.Position = Vector3.Lerp(splinePointFirst.Position, splinePoint.Position, lerpValue) + _nmSpline.transform.position;

                if (_nmSpline.IsSnapping)
                {
                    if (Physics.Raycast(newSplinePoint.Position + Vector3.up * 1000, Vector3.down, out var raycastHit))
                    {
                        newSplinePoint.Position = raycastHit.point;
                    }
                }

                newSplinePoint.Tangent = Vector3.Lerp(splinePointFirst.Tangent, splinePoint.Tangent, lerpValue);
                newSplinePoint.Normal = Vector3.Lerp(splinePointFirst.Normal, splinePoint.Normal, lerpValue);
                newSplinePoint.Binormal = Vector3.Lerp(splinePointFirst.Binormal, splinePoint.Binormal, lerpValue);
                newSplinePoint.Orientation = Quaternion.Slerp(splinePointFirst.Orientation, splinePoint.Orientation, lerpValue);
                newSplinePoint.Rotation = Quaternion.Slerp(splinePointFirst.Rotation, splinePoint.Rotation, lerpValue);
                newSplinePoint.ID = i;
                Positions.Add(lengthToFind, newSplinePoint);

                lastID = i;

                return newSplinePoint;
            }


            if (!_nmSpline.IsLooping && lengthToFind <= 0)
            {
                NmSplinePoint splinePointFirst = PointsArray[0];
                NmSplinePoint splinePoint = PointsArray[1];

                float distance = splinePoint.Distance - splinePointFirst.Distance;
                float distanceBasePoint = lengthToFind - splinePointFirst.Distance;
                float lerpValue = distanceBasePoint / distance;
                newSplinePoint.Position = Vector3.LerpUnclamped(splinePointFirst.Position, splinePoint.Position, lerpValue) + _nmSpline.transform.position;

                // newSplinePoint.Position = splinePointFirst.Position + transform.position;
                if (_nmSpline.IsSnapping)
                {
                    if (Physics.Raycast(newSplinePoint.Position + Vector3.up * 1000, Vector3.down, out var raycastHit))
                    {
                        newSplinePoint.Position = raycastHit.point;
                    }
                }

                newSplinePoint.Tangent = splinePointFirst.Tangent;
                newSplinePoint.Normal = splinePointFirst.Normal;
                newSplinePoint.Binormal = splinePointFirst.Binormal;
                newSplinePoint.Orientation = splinePointFirst.Orientation;
                newSplinePoint.Rotation = splinePointFirst.Rotation;

                Positions.Add(lengthToFind, newSplinePoint);


                lastID = -1;
                return newSplinePoint;
            }

            if (!_nmSpline.IsLooping && lengthToFind >= _nmSpline.Length)
            {
                NmSplinePoint splinePoint = PointsArray[^1];
                NmSplinePoint splinePointFirst = PointsArray[^2];

                float distance = splinePoint.Distance - splinePointFirst.Distance;
                float distanceBasePoint = lengthToFind - splinePointFirst.Distance;
                float lerpValue = distanceBasePoint / distance;

                newSplinePoint.Position = Vector3.LerpUnclamped(splinePointFirst.Position, splinePoint.Position, lerpValue) + _nmSpline.transform.position;

                if (_nmSpline.IsSnapping)
                {
                    if (Physics.Raycast(newSplinePoint.Position + Vector3.up * 1000, Vector3.down, out var raycastHit))
                    {
                        newSplinePoint.Position = raycastHit.point;
                    }
                }

                newSplinePoint.Tangent = splinePoint.Tangent;
                newSplinePoint.Normal = splinePoint.Normal;
                newSplinePoint.Binormal = splinePoint.Binormal;
                newSplinePoint.Orientation = splinePoint.Orientation;
                newSplinePoint.Rotation = splinePoint.Rotation;


                Positions.Add(lengthToFind, newSplinePoint);
                lastID = -2;
                return newSplinePoint;
            }


            if (_nmSpline.IsLooping)
            {
                NmSplinePoint lastPoint = PointsArray[^1];
                NmSplinePoint splinePoint = PointsArray[^2];
                NmSplinePoint splinePointFirst = PointsArray[0];
                float distance = _nmSpline.Length - splinePoint.Distance;
                float distanceBasePoint = (lastPoint.Distance + lengthToFind) - splinePoint.Distance;
                float lerpValue = distanceBasePoint / distance;

                newSplinePoint.Position = Vector3.Lerp(splinePoint.Position, splinePointFirst.Position, lerpValue) + _nmSpline.transform.position;
                if (_nmSpline.IsSnapping)
                {
                    if (Physics.Raycast(newSplinePoint.Position + Vector3.up * 1000, Vector3.down, out var raycastHit))
                    {
                        newSplinePoint.Position = raycastHit.point;
                    }
                }

                newSplinePoint.Tangent = Vector3.Lerp(splinePoint.Tangent, splinePointFirst.Tangent, lerpValue);
                newSplinePoint.Normal = Vector3.Lerp(splinePoint.Normal, splinePointFirst.Normal, lerpValue);
                newSplinePoint.Binormal = Vector3.Lerp(splinePoint.Binormal, splinePointFirst.Binormal, lerpValue);
                newSplinePoint.Orientation = Quaternion.Slerp(splinePoint.Orientation, splinePointFirst.Orientation, lerpValue);
                newSplinePoint.Rotation = Quaternion.Slerp(splinePoint.Orientation, splinePointFirst.Rotation, lerpValue);

                Positions.Add(lengthToFind, newSplinePoint);
                lastID = -3;
                return newSplinePoint;
            }

            lastID = -4;
            return newSplinePoint;
        }

        public void ClearPositions()
        {
            Positions.Clear();
        }


        public void GenerateArrayForDistanceSearch(List<NmSplinePoint> points)
        {
            PointsArray = points.ToArray();
        }
    }
}