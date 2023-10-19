// /**
//  * Created by Pawel Homenko on  08/2022
//  */

using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace NatureManufacture.RAM
{
    public class LakePolygonSimulationGenerator
    {
        private readonly LakePolygon _lakePolygon;

        public LakePolygonSimulationGenerator(LakePolygon lakePolygon)
        {
            _lakePolygon = lakePolygon;
        }

        public void Simulation()
        {
#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(_lakePolygon, "Simulate lake");
            Undo.RegisterCompleteObjectUndo(_lakePolygon.transform, "Simulate lake");
            if (_lakePolygon.meshfilter != null)
                Undo.RegisterCompleteObjectUndo(_lakePolygon.meshfilter, "Simulate lake");
#endif
            List<Vector3> vectorPoints = new List<Vector3> {_lakePolygon.transform.TransformPoint(_lakePolygon.NmSpline.MainControlPoints[0].position)};

            int iterations = 1;

            for (int i = 0; i < iterations; i++)
            {
                List<Vector3> newPoints = new List<Vector3>();
                foreach (Vector3 vec in vectorPoints)
                    for (int angle = 0; angle <= 360; angle += _lakePolygon.angleSimulation)
                    {
                        var ray = new Ray(vec,
                            new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad))
                                .normalized);
                        if (Physics.Raycast(ray, out RaycastHit hit, _lakePolygon.checkDistanceSimulation))
                        {
                            //Debug.Log(hit.distance);
                            //Debug.DrawRay(hit.point, Vector3.up * angle * 0.1f, Color.red, 1 + angle / (float)100);
                            // Debug.DrawLine(hit.point, vec, Color.green, 1 + angle / (float)100);
                            bool tooClose = false;
                            Vector3 point = hit.point;
                            foreach (Vector3 item in vectorPoints)
                                if (Vector3.Distance(point, item) < _lakePolygon.closeDistanceSimulation)
                                {
                                    tooClose = true;
                                    break;
                                }

                            foreach (Vector3 item in newPoints)
                                if (Vector3.Distance(point, item) < _lakePolygon.closeDistanceSimulation)
                                {
                                    tooClose = true;
                                    break;
                                }

                            if (!tooClose) newPoints.Add(point + ray.direction * 0.3f);
                        }
                        else
                        {
                            bool tooClose = false;
                            Vector3 point = ray.origin + ray.direction * 50;
                            foreach (Vector3 item in vectorPoints)
                                if (Vector3.Distance(point, item) < _lakePolygon.closeDistanceSimulation)
                                {
                                    tooClose = true;
                                    break;
                                }

                            foreach (Vector3 item in newPoints)
                                if (Vector3.Distance(point, item) < _lakePolygon.closeDistanceSimulation)
                                {
                                    tooClose = true;
                                    break;
                                }

                            if (!tooClose) newPoints.Add(point);
                        }
                    }

                if (i == 0)
                    vectorPoints.AddRange(newPoints);
                else
                    for (int k = 0; k < newPoints.Count; k++)
                    {
                        float min = float.MaxValue;
                        int idMin = -1;
                        Vector3 point = newPoints[k];
                        for (int p = 0; p < vectorPoints.Count; p++)
                        {
                            Vector3 posOne = vectorPoints[p];
                            Vector3 posTwo = vectorPoints[(p + 1) % vectorPoints.Count];

                            bool intersects = false;
                            for (int f = 0; f < vectorPoints.Count; f++)
                                if (p != f)
                                {
                                    Vector3 posCheckOne = vectorPoints[f];
                                    Vector3 posCheckTwo = vectorPoints[(f + 1) % vectorPoints.Count];

                                    if (RamMath.AreLinesIntersecting(posOne, point, posCheckOne, posCheckTwo) || RamMath.AreLinesIntersecting(point, posTwo, posCheckOne, posCheckTwo))
                                    {
                                        intersects = true;
                                        break;
                                    }
                                }

                            if (!intersects)
                            {
                                float dist = Vector3.Distance(point, posTwo);
                                if (min > dist)
                                {
                                    min = dist;
                                    idMin = (p + 1) % vectorPoints.Count;
                                }
                                //vectorPoints.Insert(p + 1, point);
                                //break;
                            }
                        }

                        if (idMin > -1)
                            vectorPoints.Insert(idMin, point);
                    }

                if (i == 0 && _lakePolygon.removeFirstPointSimulation)
                    vectorPoints.RemoveAt(0);
            }


            _lakePolygon.NmSpline.MainControlPoints.Clear();

            foreach (Vector3 vec in vectorPoints) _lakePolygon.NmSpline.AddPoint(_lakePolygon.transform.InverseTransformPoint(vec), _lakePolygon.snapToTerrain);

            _lakePolygon.GeneratePolygon();
        }
    }
}