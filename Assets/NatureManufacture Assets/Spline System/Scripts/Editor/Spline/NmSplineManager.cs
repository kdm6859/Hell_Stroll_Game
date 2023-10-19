// /**
//  * Created by Pawel Homenko on  07/2022
//  */

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using XDiffGui;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace NatureManufacture.RAM.Editor
{
    public class NmSplineManager
    {
        public NmSplineManager(NmSpline nmSpline, string name)
        {
            NmSpline = nmSpline;
            NmSpline.NmSplineChanged ??= new UnityEvent();
            _name = name;
        }

        public Action<int> AdditionalPointUI { get; set; }

        private string _name = "";

        private Rect _pointWindowRect = new Rect(50, 5, 340, 120);
        private readonly Color _selectedTextColor = new Color(0.38f, 1f, 0.55f, 1);

        private readonly Color _selectedHandleColor = new Color(0.38f, 1f, 0.55f, 0.5f);

        private readonly Color _notSelectedTextColor = Color.red;

        private readonly Color _notSelectedHandleColor = new Color(1.0f, 0.36f, 0.36f, 0.5f);

        private NmSpline NmSpline { get; }

        private bool _splineChanged = false;


        public void PointsUI()
        {
            if (GUILayout.Button("Add point at end"))
            {
                NmSpline.AddPointAtEnd();
                NmSpline.NmSplineChanged?.Invoke();
            }

            if (GUILayout.Button("Remove last point"))
            {
                NmSpline.RemoveLastPoint();
                NmSpline.NmSplineChanged?.Invoke();
            }

            if (GUILayout.Button(new GUIContent("Remove all points", "Removes all points"))) NmSpline.RemovePoints();

            if (GUILayout.Button(new GUIContent("Reverse all points", "Reverses all points"))) NmSpline.ReversePoints();

            for (int i = 0; i < NmSpline.MainControlPoints.Count; i++)
            {
                GUILayout.Label("Point: " + i, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                PointGUI(i);

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
        }

        private void PointGUI(int i)
        {
            if (NmSpline.MainControlPoints.Count <= i)
                return;

            EditorGUILayout.BeginHorizontal();
            if (NmSpline.UseWidth)
            {
                NmSpline.MainControlPoints[i].position = EditorGUILayout.Vector4Field("", NmSpline.MainControlPoints[i].position);
                if (NmSpline.MainControlPoints[i].position.w <= 0)
                {
                    Vector4 vec4 = NmSpline.MainControlPoints[i].position;
                    vec4.w = 0;
                    NmSpline.MainControlPoints[i].position = vec4;
                }
            }
            else
            {
                NmSpline.MainControlPoints[i].position = EditorGUILayout.Vector3Field("", NmSpline.MainControlPoints[i].position);
            }

            if (GUILayout.Button(new GUIContent("A", "Add point after this point"), GUILayout.MaxWidth(20)))
            {
                NmSpline.AddPointAfter(i);
                NmSpline.NmSplineChanged?.Invoke();

                NmSpline.SelectedPosition = i + 1;
            }

            if (GUILayout.Button(new GUIContent("R", "Remove this Point"), GUILayout.MaxWidth(20)))
            {
                NmSpline.RemovePoint(i);
                NmSpline.NmSplineChanged?.Invoke();


                if (NmSpline.SelectedPosition == i)
                    NmSpline.SelectedPosition--;
            }

            if (NmSpline.SelectedPosition != i && GUILayout.Toggle(NmSpline.SelectedPosition == i, new GUIContent("S", "Select point"), "Button",
                    GUILayout.MaxWidth(20)))
                NmSpline.SelectedPosition = i;

            EditorGUILayout.EndHorizontal();

            if (NmSpline.UseRotation)
            {
                EditorGUILayout.BeginHorizontal();
                if (NmSpline.MainControlPoints.Count > i)
                    NmSpline.MainControlPoints[i].rotation =
                        Quaternion.Euler(EditorGUILayout.Vector3Field("", NmSpline.MainControlPoints[i].rotation.eulerAngles));
                if (GUILayout.Button(new GUIContent("    Clear rotation    ", "Clear Rotation")))
                {
                    NmSpline.MainControlPoints[i].rotation = Quaternion.identity;
                    NmSpline.NmSplineChanged?.Invoke();
                }


                EditorGUILayout.EndHorizontal();
            }
            else
                NmSpline.MainControlPoints[i].rotation = Quaternion.identity;

            if (NmSpline.UsePointSnap)
                if (NmSpline.MainControlPoints.Count > i)
                    NmSpline.MainControlPoints[i].snap =
                        EditorGUILayout.Toggle("Snap to terrain", NmSpline.MainControlPoints[i].snap == 1)
                            ? 1
                            : 0;

            if (NmSpline.UseMeshCurve)
                if (NmSpline.MainControlPoints.Count > i)
                    NmSpline.MainControlPoints[i].meshCurve =
                        EditorGUILayout.CurveField("Mesh curve", NmSpline.MainControlPoints[i].meshCurve);


            if (NmSpline.MainControlPoints.Count > i)
            {
                if (NmSpline.UseSplinePointDensity)
                    NmSpline.MainControlPoints[i].additionalDensityU = Mathf.NextPowerOfTwo(EditorGUILayout.IntSlider("Triangles Density U", (int) NmSpline.MainControlPoints[i].additionalDensityU, 1, 32));
                if (NmSpline.UseSplineWidthDensity)
                    NmSpline.MainControlPoints[i].additionalDensityV = Mathf.NextPowerOfTwo(EditorGUILayout.IntSlider("Triangles Density V", (int) NmSpline.MainControlPoints[i].additionalDensityV, 1, 32));
            }

            AdditionalPointUI?.Invoke(i);
        }

        private void PointWindow(int id)
        {
            if (NmSpline.SelectedPosition < 0 || NmSpline.MainControlPoints.Count <= NmSpline.SelectedPosition) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            PointGUI(NmSpline.SelectedPosition);

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
            HandleUtility.Repaint();


            if (EditorGUI.EndChangeCheck())
            {
                if (NmSpline.SelectedPosition < 0 || NmSpline.MainControlPoints.Count <= NmSpline.SelectedPosition) return;
                if (NmSpline != null)
                {
                    NmSpline.NmSplineChanged.Invoke();
                }
            }
        }


        private void InSceneUIPoint()
        {
            if (NmSpline.SelectedPosition < 0 || NmSpline.MainControlPoints.Count <= NmSpline.SelectedPosition) return;

            Handles.BeginGUI();

            GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.99f);
            string label = _name + " Point " + NmSpline.SelectedPosition;

            _pointWindowRect = GUILayout.Window(0, _pointWindowRect, PointWindow, label);

            Handles.EndGUI();
        }


        public void SceneGUI(Object objectToUndo, bool blockFirstPoint = false, bool blockLastPoint = false)
        {
            NmSpline.CheckForNanRotation();
            
            InSceneUIPoint();

            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            Color baseColor = Handles.color;
            int controlPointToDelete = -1;

            for (int j = 0; j < NmSpline.MainControlPoints.Count; j++)
            {
                EditorGUI.BeginChangeCheck();


                Vector3 handlePos = (Vector3) NmSpline.MainControlPoints[j].position + NmSpline.Transform.position;
                Quaternion handleRot = NmSpline.MainControlPoints[j].orientation * Quaternion.LookRotation(Vector3.up);
                float handleSize = HandleUtility.GetHandleSize(handlePos);


                var style = new GUIStyle();


                if (NmSpline.SelectedPosition == j)
                {
                    style.normal.textColor = _selectedTextColor;
                    Handles.color = _selectedHandleColor;
                }
                else
                {
                    style.normal.textColor = _notSelectedTextColor;
                    Handles.color = _notSelectedHandleColor;
                }


                Handles.CircleHandleCap(
                    controlId,
                    handlePos,
                    handleRot,
                    0.3f * handleSize,
                    EventType.Repaint
                );

                Handles.DrawSolidDisc(handlePos, handleRot * Vector3.forward, 0.15f * handleSize);


                //if (NmSpline.SelectedPosition != j) - can't use because of hot control
                if (Handles.Button(handlePos, handleRot,
                        0.2f * handleSize, 0.25f * handleSize,
                        Handles.CircleHandleCap))
                    NmSpline.SelectedPosition = j;


                Vector3 screenPoint = Camera.current.WorldToScreenPoint(handlePos);

                if (screenPoint.z > 0)
                    Handles.Label(handlePos + Vector3.up * handleSize,
                        "Point: " + j, style);

                float width = NmSpline.MainControlPoints[j].position.w;


                // if (_splineChanged && j != NmSpline.SelectedPosition)
                //    continue;


                if (Event.current.control && Event.current.shift && NmSpline.MainControlPoints.Count > 1)
                {
                    int id = GUIUtility.GetControlID(FocusType.Passive);


                    if (HandleUtility.nearestControl == id)
                    {
                        Handles.color = Color.white;
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            controlPointToDelete = j;
                    }
                    else
                    {
                        Handles.color = Handles.xAxisColor;
                    }

                    float size = 0.6f;
                    size = handleSize * size;
                    if (Event.current.type == EventType.Repaint)
                        Handles.SphereHandleCap(id, (Vector3) NmSpline.MainControlPoints[j].position + NmSpline.Transform.position,
                            Quaternion.identity, size, EventType.Repaint);
                    else if (Event.current.type == EventType.Layout)
                        Handles.SphereHandleCap(id, (Vector3) NmSpline.MainControlPoints[j].position + NmSpline.Transform.position,
                            Quaternion.identity, size, EventType.Layout);
                }
                else if (Tools.current == Tool.Move)
                {
                    float size = 0.6f;
                    size = handleSize * size;
                    Vector3 position = NmSpline.Transform.position;

                    if (Tools.pivotRotation == PivotRotation.Global)
                    {
                        Handles.color = Handles.xAxisColor;
                        //Vector3 position = NmSpline.Transform.position;
                        Vector4 pos = Handles.Slider((Vector3) NmSpline.MainControlPoints[j].position + position,
                            Vector3.right, size, Handles.ArrowHandleCap, 0.01f) - position;
                        Handles.color = Handles.yAxisColor;
                        pos = Handles.Slider((Vector3) pos + position, Vector3.up, size,
                            Handles.ArrowHandleCap, 0.01f) - position;
                        Handles.color = Handles.zAxisColor;
                        pos = Handles.Slider((Vector3) pos + position, Vector3.forward, size,
                            Handles.ArrowHandleCap, 0.01f) - position;

                        Vector3 halfPos = (Vector3.right + Vector3.forward) * size * 0.3f;
                        Handles.color = Handles.yAxisColor;
                        pos = Handles.Slider2D((Vector3) pos + position + halfPos, Vector3.up,
                                  Vector3.right, Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) -
                              position - halfPos;
                        halfPos = (Vector3.right + Vector3.up) * size * 0.3f;
                        Handles.color = Handles.zAxisColor;
                        pos = Handles.Slider2D((Vector3) pos + position + halfPos, Vector3.forward,
                                  Vector3.right, Vector3.up, size * 0.3f, Handles.RectangleHandleCap, 0.01f) -
                              position - halfPos;
                        halfPos = (Vector3.up + Vector3.forward) * size * 0.3f;
                        Handles.color = Handles.xAxisColor;
                        pos = Handles.Slider2D((Vector3) pos + position + halfPos, Vector3.right,
                                  Vector3.up, Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) -
                              position - halfPos;

                        pos.w = width;

                        if (Vector3.Distance(NmSpline.MainControlPoints[j].position, pos) > 0.0001f)
                        {
                            //Debug.Log($"name {_name}");
                            NmSpline.SelectedPosition = j;
                        }


                        NmSpline.MainControlPoints[j].position = pos;
                    }
                    else
                    {
                        Handles.color = Handles.xAxisColor;
                        //Vector3 position = NmSpline.Transform.position;
                        Vector4 pos = Handles.Slider((Vector3) NmSpline.MainControlPoints[j].position + position,
                            handleRot * Vector3.right, size, Handles.ArrowHandleCap, 0.01f) - position;
                        Handles.color = Handles.yAxisColor;
                        pos = Handles.Slider((Vector3) pos + position, handleRot * Vector3.forward, size,
                            Handles.ArrowHandleCap, 0.01f) - position;
                        Handles.color = Handles.zAxisColor;
                        pos = Handles.Slider((Vector3) pos + position, handleRot * Vector3.down, size,
                            Handles.ArrowHandleCap, 0.01f) - position;

                        Vector3 halfPos = (handleRot * Vector3.right + handleRot * Vector3.forward) * size * 0.3f;
                        Handles.color = Handles.zAxisColor;
                        pos = Handles.Slider2D((Vector3) pos + position + halfPos, handleRot * Vector3.up,
                                  handleRot * Vector3.right, handleRot * Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) -
                              position - halfPos;

                        halfPos = (handleRot * Vector3.right + handleRot * Vector3.down) * size * 0.3f;
                        Handles.color = Handles.yAxisColor;
                        pos = Handles.Slider2D((Vector3) pos + position + halfPos, handleRot * Vector3.forward,
                                  handleRot * Vector3.right, handleRot * Vector3.down, size * 0.3f, Handles.RectangleHandleCap, 0.01f) -
                              position - halfPos;

                        halfPos = (handleRot * Vector3.down + handleRot * Vector3.forward) * size * 0.3f;
                        Handles.color = Handles.xAxisColor;
                        pos = Handles.Slider2D((Vector3) pos + position + halfPos, handleRot * Vector3.right,
                                  handleRot * Vector3.down, handleRot * Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) -
                              position - halfPos;

                        pos.w = width;

                        if (Vector3.Distance(NmSpline.MainControlPoints[j].position, pos) > 0.0001f)
                        {
                            //Debug.Log($"name {_name}");
                            NmSpline.SelectedPosition = j;
                        }


                        NmSpline.MainControlPoints[j].position = pos;
                    }
                }
                else if (Tools.current == Tool.Rotate && NmSpline.UseRotation)
                {
                    if (NmSpline.MainControlPoints.Count > j)
                        if (!(blockFirstPoint && j == 0 ||
                              blockLastPoint && j == NmSpline.MainControlPoints.Count - 1))
                        {
                            float size = 0.6f;
                            size = handleSize * size;

                            Handles.color = Handles.zAxisColor;
                            Quaternion rotation = Handles.Disc(NmSpline.MainControlPoints[j].orientation, handlePos,
                                NmSpline.MainControlPoints[j].orientation * new Vector3(0, 0, 1), size, true, 0.1f);

                            Handles.color = Handles.yAxisColor;
                            rotation = Handles.Disc(rotation, handlePos, rotation * new Vector3(0, 1, 0), size, true,
                                0.1f);

                            Handles.color = Handles.xAxisColor;
                            rotation = Handles.Disc(rotation, handlePos, rotation * new Vector3(1, 0, 0), size, true,
                                0.1f);

                            if ((Quaternion.Inverse(NmSpline.MainControlPoints[j].orientation) * rotation).eulerAngles.magnitude > 0)
                            {
                                //Debug.Log($"Rotation changed {j}");
                                NmSpline.SelectedPosition = j;
                            }

                            NmSpline.MainControlPoints[j].rotation *=
                                Quaternion.Inverse(NmSpline.MainControlPoints[j].orientation) * rotation;

                            if (float.IsNaN(NmSpline.MainControlPoints[j].rotation.x) ||
                                float.IsNaN(NmSpline.MainControlPoints[j].rotation.y) ||
                                float.IsNaN(NmSpline.MainControlPoints[j].rotation.z) ||
                                float.IsNaN(NmSpline.MainControlPoints[j].rotation.w))
                            {
                                NmSpline.MainControlPoints[j].rotation = Quaternion.identity;
                                NmSpline.NmSplineChanged?.Invoke();
                            }

                            Handles.color = baseColor;
                            Handles.FreeRotateHandle(Quaternion.identity, handlePos, size);

                            Handles.CubeHandleCap(0, handlePos, NmSpline.MainControlPoints[j].orientation, size * 0.3f,
                                EventType.Repaint);

                            Vector3 position = NmSpline.Transform.position;

                            Handles.DrawLine(NmSpline.ControlPointsPositionUp[j] + position, NmSpline.ControlPointsPositionDown[j] + position);
                        }
                }
                else if (Tools.current == Tool.Scale && NmSpline.UseWidth)
                {
                    Handles.color = Handles.xAxisColor;
                    //Vector3 handlePos = (Vector3)spline.controlPoints [j] + spline.NmSpline.Transform.position;

                    width = Handles.ScaleSlider(NmSpline.MainControlPoints[j].position.w,
                        (Vector3) NmSpline.MainControlPoints[j].position + NmSpline.Transform.position, new Vector3(0, 0.5f, 0),
                        Quaternion.Euler(-90, 0, 0), handleSize, 0.01f);

                    Vector4 pos = NmSpline.MainControlPoints[j].position;
                    pos.w = width;


                    NmSpline.MainControlPoints[j].position = pos;

                    if (Vector3.Distance(NmSpline.MainControlPoints[j].position, pos) > 0.0001f) NmSpline.SelectedPosition = j;
                }


                if (EditorGUI.EndChangeCheck())
                {
                    //NmSpline.SelectedPosition = j;

                    Undo.RecordObject(objectToUndo, "Change Position");
                    Undo.RecordObject(NmSpline, "Change Position");
                    NmSpline.NmSplineChanged?.Invoke();
                    _splineChanged = true;
                }
            }

            if (controlPointToDelete >= 0)
            {
                if (NmSpline.SelectedPosition == controlPointToDelete) NmSpline.SelectedPosition--;
                Undo.RecordObject(objectToUndo, "Remove point");
                Undo.RecordObject(NmSpline, "Change Position");


                NmSpline.RemovePoint(controlPointToDelete);

                NmSpline.NmSplineChanged?.Invoke();

                GUIUtility.hotControl = controlId;
                Event.current.Use();
                HandleUtility.Repaint();
            }

            //Add Point at end
            if (Event.current.control && !Event.current.alt && !Event.current.shift)
            {
                AddPointAtEnd(objectToUndo, controlId);
            }

            //Add Point between 
            if (!Event.current.control && Event.current.shift && NmSpline.MainControlPoints.Count > 1)
            {
                AddPointBetween(objectToUndo, controlId);
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && Event.current.control && !Event.current.alt) GUIUtility.hotControl = 0;

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && Event.current.shift) GUIUtility.hotControl = 0;

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && _splineChanged)
            {
                //Debug.Log($"changed ended {_splineChanged}");
                _splineChanged = false;
                GUIUtility.hotControl = 0;
            }
        }

        private void AddPointAtEnd(Object objectToUndo, int controlId)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            Vector3 handlePos = hit.point;

            if (NmSpline.MainControlPoints.Count > 0)
                Handles.DrawLine(hit.point, (Vector3) NmSpline.MainControlPoints[^1].position + NmSpline.Transform.position);

            float handleSize = HandleUtility.GetHandleSize(handlePos);

            Handles.DrawSolidDisc(handlePos, hit.normal, 0.25f * handleSize);


            Handles.CircleHandleCap(
                0,
                handlePos,
                Quaternion.LookRotation(hit.normal),
                0.5f * handleSize,
                EventType.Repaint
            );


            if (Event.current.type != EventType.MouseDown || Event.current.button != 0) return;


            Undo.RecordObject(objectToUndo, "Add point");
            Undo.RecordObject(NmSpline, "Change Position");

            Vector4 position = hit.point - NmSpline.Transform.position;

            if (!Event.current.alt)
            {
                NmSpline.AddPoint(position, NmSpline.IsSnapping, NmSpline.Width);
                NmSpline.SelectedPosition = NmSpline.MainControlPoints.Count - 1;
            }
            else
            {
                NmSpline.AddPointAfter(-1, NmSpline.IsSnapping);
                NmSpline.ChangePointPosition(0, position);
                NmSpline.SelectedPosition = 0;

                NmSpline.NmSplineChanged?.Invoke();
            }


            NmSpline.NmSplineChanged?.Invoke();


            GUIUtility.hotControl = controlId;
            Event.current.Use();
            HandleUtility.Repaint();
        }

        private void AddPointBetween(Object objectToUndo, int controlId)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return;


            int idMin = -1;
            float distanceMin = float.MaxValue;
            Vector3 handlePos;
            for (int j = 0; j < NmSpline.MainControlPoints.Count; j++)
            {
                handlePos = (Vector3) NmSpline.MainControlPoints[j].position + NmSpline.Transform.position;

                float pointDist = Vector3.Distance(hit.point, handlePos);
                if (pointDist < distanceMin)
                {
                    distanceMin = pointDist;
                    idMin = j;
                }
            }

            Vector3 posOne = (Vector3) NmSpline.MainControlPoints[idMin].position + NmSpline.Transform.position;
            Vector3 posTwo;


            if (idMin == 0)
            {
                posTwo = (Vector3) NmSpline.MainControlPoints[1].position + NmSpline.Transform.position;
            }
            else if (idMin == NmSpline.MainControlPoints.Count - 1 && !NmSpline.IsLooping)
            {
                posTwo = (Vector3) NmSpline.MainControlPoints[^2].position +
                         NmSpline.Transform.position;

                idMin = idMin - 1;
            }
            else
            {
                Vector3 position = NmSpline.Transform.position;
                Vector3 posPrev = (Vector3) NmSpline.MainControlPoints[NmSpline.ClampListPos(idMin - 1)].position + position;
                Vector3 posNext = (Vector3) NmSpline.MainControlPoints[NmSpline.ClampListPos(idMin + 1)].position + position;

                if (Vector3.Distance(hit.point, posPrev) > Vector3.Distance(hit.point, posNext))
                {
                    posTwo = posNext;
                }
                else
                {
                    posTwo = posPrev;
                    idMin = idMin - 1;
                }
            }


            Handles.DrawLine(hit.point, posOne);
            Handles.DrawLine(hit.point, posTwo);


            handlePos = hit.point;

            float handleSize = HandleUtility.GetHandleSize(handlePos);

            Handles.DrawSolidDisc(handlePos, hit.normal, 0.25f * handleSize);


            Handles.CircleHandleCap(
                0,
                handlePos,
                Quaternion.LookRotation(hit.normal),
                0.5f * handleSize,
                EventType.Repaint
            );

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Undo.RecordObject(objectToUndo, "Add point");
                Undo.RecordObject(NmSpline, "Change Position");

                Vector4 position = handlePos - NmSpline.Transform.position;
                NmSpline.AddPointAfter(idMin, NmSpline.IsSnapping);
                NmSpline.ChangePointPosition(idMin + 1, position);

                NmSpline.NmSplineChanged?.Invoke();

                GUIUtility.hotControl = controlId;
                Event.current.Use();
                HandleUtility.Repaint();
            }
        }
    }
}