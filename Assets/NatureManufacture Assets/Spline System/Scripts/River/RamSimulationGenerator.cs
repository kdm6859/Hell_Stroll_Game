// /**
//  * Created by Pawel Homenko on  07/2022
//  */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NatureManufacture.RAM
{
    public class RamSimulationGenerator
    {
        private RamSpline _ramSpline;

        public RamSimulationGenerator(RamSpline ramSpline)
        {
            _ramSpline = ramSpline;
        }

        public RamSpline RamSpline
        {
            get => _ramSpline;
            set => _ramSpline = value;
        }

        public void SimulateRiver(bool generate = true)
        {
            if (_ramSpline.meshGo != null)
            {
                if (Application.isEditor)
                    Object.DestroyImmediate(_ramSpline.meshGo);
                else
                    Object.Destroy(_ramSpline.meshGo);
            }

            if (_ramSpline.NmSpline.MainControlPoints.Count == 0)
            {
                Debug.Log("Add one point to start Simulating River");
                return;
            }


            var ray = new Ray();


            Vector3 lastPosition = _ramSpline.transform.TransformPoint(_ramSpline.NmSpline.MainControlPoints[^1].position);

            List<Vector3> positionsGenerated = new List<Vector3>();
            if (_ramSpline.NmSpline.MainControlPoints.Count > 1)
            {
                positionsGenerated.Add(_ramSpline.transform.TransformPoint(_ramSpline.NmSpline.MainControlPoints[^2].position));
                positionsGenerated.Add(lastPosition);
            }


            List<Vector3> samplePositionsGenerated = new List<Vector3> {lastPosition};

            //Debug.DrawRay(lastPosition + new Vector3(0, 3, 0), Vector3.down * 20, Color.white, 3);

            float simulatedLength = 0;
            int i = -1;
            int added = 0;
            bool end = false;

            float widthNew = _ramSpline.NmSpline.MainControlPoints.Count > 0 ? _ramSpline.NmSpline.MainControlPoints[^1].position.w : _ramSpline.BaseProfile.width;

            do
            {
                i++;
                if (i > 0)
                {
                    Vector3 maxPosition = Vector3.zero;
                    float max = float.MinValue;
                    bool foundNextPosition = false;
                    for (float j = _ramSpline.BaseProfile.simulatedMinStepSize; j < 10; j += 0.1f)
                    {
                        for (int angle = 0; angle < 36; angle++)
                        {
                            float x = j * Mathf.Cos(angle);
                            float z = j * Mathf.Sin(angle);

                            ray.origin = lastPosition + new Vector3(0, 1000, 0) + new Vector3(x, 0, z);
                            ray.direction = Vector3.down;

                            if (Physics.Raycast(ray, out RaycastHit hit, 10000))
                                if (hit.distance > max)
                                {
                                    bool goodPoint = true;


                                    foreach (Vector3 item in positionsGenerated)
                                        if (Vector3.Distance(item, lastPosition) > Vector3.Distance(item, hit.point) + 0.5f)
                                        {
                                            goodPoint = false;
                                            break;
                                        }

                                    if (goodPoint)
                                    {
                                        foundNextPosition = true;
                                        max = hit.distance;
                                        maxPosition = hit.point;
                                    }
                                }
                            //else
                            //    Debug.DrawRay(ray.origin, ray.direction * 10000, Color.red, 3);
                        }

                        if (foundNextPosition)
                            break;
                    }

                    if (!foundNextPosition)
                        break;

                    if (maxPosition.y > lastPosition.y)
                    {
                        if (_ramSpline.BaseProfile.simulatedNoUp)
                            maxPosition.y = lastPosition.y;
                        if (_ramSpline.BaseProfile.simulatedBreakOnUp)
                            end = true;

                        //Debug.DrawRay(maxPosition + new Vector3(0, 5, 0), ray.direction * 10, Color.red, 3);
                    }
                    // else
                    //    Debug.DrawRay(maxPosition + new Vector3(0, 5, 0), ray.direction * 10, Color.blue, 3);


                    simulatedLength += Vector3.Distance(maxPosition, lastPosition);
                    if (i % _ramSpline.BaseProfile.simulatedRiverPoints == 0 || _ramSpline.BaseProfile.simulatedRiverLength <= simulatedLength || end)
                    {
                        //Debug.DrawRay(maxPosition + new Vector3(0, 5, 0), ray.direction * 20, Color.white, 3);

                        samplePositionsGenerated.Add(maxPosition);

                        if (generate)
                        {
                            added++;

                            Vector4 newPosition = maxPosition - _ramSpline.transform.position;

                            newPosition.w = widthNew + (_ramSpline.BaseProfile.noiseWidth
                                ? _ramSpline.BaseProfile.noiseMultiplierWidth * (Mathf.PerlinNoise(_ramSpline.BaseProfile.noiseSizeWidth * added, 0) - 0.5f)
                                : 0);


                            _ramSpline.NmSpline.MainControlPoints.Add(new RamControlPoint(newPosition, Quaternion.identity, 0, new AnimationCurve(_ramSpline.BaseProfile.meshCurve.keys)));
                        }
                    }


                    positionsGenerated.Add(lastPosition);
                    lastPosition = maxPosition;
                }
            } while (_ramSpline.BaseProfile.simulatedRiverLength > simulatedLength && !end);

            if (!generate)
            {
                widthNew = _ramSpline.NmSpline.MainControlPoints.Count > 0 ? _ramSpline.NmSpline.MainControlPoints[^1].position.w : _ramSpline.BaseProfile.width;
                float widthNoise;

                List<List<Vector4>> positionArray = new List<List<Vector4>>();
                var v1 = new Vector3();
                for (i = 0; i < samplePositionsGenerated.Count - 1; i++)
                {
                    widthNoise = widthNew +
                                 (_ramSpline.BaseProfile.noiseWidth
                                     ? _ramSpline.BaseProfile.noiseMultiplierWidth * (Mathf.PerlinNoise(_ramSpline.BaseProfile.noiseSizeWidth * i, 0) - 0.5f)
                                     : 0);


                    //Debug.DrawLine(samplePositionsGenerated[i], samplePositionsGenerated[i + 1], Color.white, 3);

                    v1 = Vector3.Cross(samplePositionsGenerated[i + 1] - samplePositionsGenerated[i], Vector3.up)
                        .normalized;

                    if (i > 0)
                    {
                        Vector3 v2 = Vector3
                            .Cross(samplePositionsGenerated[i] - samplePositionsGenerated[i - 1], Vector3.up).normalized;
                        v1 = (v1 + v2).normalized;
                    }

                    //Vector3 v2 = Vector3.Cross(samplePositionsGenerated[i + 1] - samplePositionsGenerated[i], v1).normalized;

                    //Debug.DrawLine(samplePositionsGenerated[i] - v1 * widthNew * 0.5f, samplePositionsGenerated[i] + v1 * widthNew * 0.5f, Color.blue, 3);

                    List<Vector4> positionRow = new List<Vector4>
                    {
                        samplePositionsGenerated[i] + v1 * (widthNoise * 0.5f),
                        samplePositionsGenerated[i] - v1 * (widthNoise * 0.5f)
                    };


                    positionArray.Add(positionRow);
                }

                widthNoise = widthNew +
                             (_ramSpline.BaseProfile.noiseWidth ? _ramSpline.BaseProfile.noiseMultiplierWidth * (Mathf.PerlinNoise(_ramSpline.BaseProfile.noiseSizeWidth * i, 0) - 0.5f) : 0);
                List<Vector4> positionRowLast = new List<Vector4>
                {
                    samplePositionsGenerated[i] + v1 * (widthNoise * 0.5f),
                    samplePositionsGenerated[i] - v1 * (widthNoise * 0.5f)
                };

                positionArray.Add(positionRowLast);


                var meshTerrain = new Mesh
                {
                    indexFormat = IndexFormat.UInt32
                };
                List<Vector3> vertices = new List<Vector3>();
                List<int> triangles = new List<int>();
                // List<Vector2> uv = new List<Vector2>();

                foreach (List<Vector4> positionRow in positionArray)
                foreach (Vector4 vert in positionRow)
                    vertices.Add(vert);

                for (i = 0; i < positionArray.Count - 1; i++)
                {
                    int count = positionArray[i].Count;
                    for (int j = 0; j < count - 1; j++)
                    {
                        triangles.Add(j + i * count);
                        triangles.Add(j + (i + 1) * count);
                        triangles.Add(j + 1 + i * count);

                        triangles.Add(j + 1 + i * count);
                        triangles.Add(j + (i + 1) * count);
                        triangles.Add(j + 1 + (i + 1) * count);
                    }
                }


                meshTerrain.SetVertices(vertices);
                meshTerrain.SetTriangles(triangles, 0);
                // meshTerrain.SetUVs(0, uv);

                meshTerrain.RecalculateNormals();
                meshTerrain.RecalculateTangents();
                meshTerrain.RecalculateBounds();

                _ramSpline.meshGo = new GameObject("TerrainMesh")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                _ramSpline.meshGo.AddComponent<MeshFilter>();
                _ramSpline.meshGo.transform.parent = _ramSpline.transform;
                var meshRenderer = _ramSpline.meshGo.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = new Material(Shader.Find("Debug Terrain Carve"))
                {
                    color = new Color(0, 0.5f, 0)
                };


                _ramSpline.meshGo.transform.position = Vector3.zero;
                _ramSpline.meshGo.GetComponent<MeshFilter>().sharedMesh = meshTerrain;
            }
        }
    }
}