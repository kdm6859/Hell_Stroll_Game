namespace NatureManufacture.RAM.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;


    [CustomEditor(typeof(LakePolygon))]
    public sealed class LakePolygonEditor : Editor
    {
        private bool _dragged;

        private LakePolygon _lakePolygon;

        private Texture2D _logo;


        private TerrainManagerEditor TerrainManagerEditor { get; set; }
        private NmSplineManager NmSplineManager { get; set; }

        private LakePolygonTips LakePolygonTips { get; } = new();

        private VertexPainterEditor<LakePolygon> VertexPainterEditor { get; set; }


        public string[] toolbarStrings = new[]
        {
            "Basic",
            "Points",
            "Mesh Painting",
            "Simulate\n[ALPHA] ",
            "Terrain",
            "File Points",
            "Tips",
            "Manual",
            "Video Tutorials"
#if VEGETATION_STUDIO
        ,
        "Vegetation Studio"
#endif
#if VEGETATION_STUDIO_PRO
        ,
        "Vegetation Studio Pro"
#endif
        };


        private LakePolygonVegetationStudio _lakePolygonVegetationStudio;

        [MenuItem("GameObject/3D Object/NatureManufacture/Create Lake Polygon")]
        public static void CreateLakePolygon()
        {
            Selection.activeGameObject = LakePolygon
                .CreatePolygon(AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat")).gameObject;
        }

        private void OnEnable()
        {
            if (_lakePolygon == null)
                _lakePolygon = (LakePolygon) target;

            if (VertexPainterEditor == null)
            {
                VertexPainterEditor = new VertexPainterEditor<LakePolygon>(_lakePolygon.VertexPainterData);
                VertexPainterEditor.OnResetDrawing.AddListener(RegeneratePolygon);
                VertexPainterEditor.ColorDescriptions = "R - Slow Water \nG - Small Cascade \nB - Big Cascade \nA - Opacity";
            }

            _lakePolygonVegetationStudio ??= new LakePolygonVegetationStudio(_lakePolygon);


            _lakePolygon.MoveControlPointsToMainControlPoints();

            //Debug.Log("NmSplineEditor");
            if (NmSplineManager == null)
            {
                NmSplineManager = new NmSplineManager(_lakePolygon.NmSpline, "Lake");
                _lakePolygon.NmSpline.NmSplineChanged.AddListener(PositionMoved);
                //NmSplineEditor.AdditionalPointUI += PointGUI;
            }

            SceneView.duringSceneGui -= OnSceneGUIInvoke;
            SceneView.duringSceneGui += OnSceneGUIInvoke;

            if (TerrainManagerEditor == null)
            {
                if (_lakePolygon.TerrainManager.NmSpline != _lakePolygon.NmSpline)
                    _lakePolygon.TerrainManager.NmSpline = _lakePolygon.NmSpline;
                TerrainManagerEditor = new TerrainManagerEditor(_lakePolygon.TerrainManager);
                TerrainManagerEditor.GetTerrains.AddListener(() => _lakePolygon.GeneratePolygon());
            }

            if (_lakePolygon.BaseProfile == null)
            {
                _lakePolygon.GenerateBaseProfile();
            }

            // _lakePolygon.GeneratePolygon();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUIInvoke;

            if (!(_lakePolygon != null))
                return;

            if (_lakePolygon == null)
                _lakePolygon = (LakePolygon) target;

            if (_lakePolygon.meshTerrainGOs is {Count: > 0})
            {
                foreach (var item in _lakePolygon.meshTerrainGOs)
                {
                    DestroyImmediate(item);
                }

                _lakePolygon.meshTerrainGOs.Clear();
            }

            _lakePolygon.NmSpline.NmSplineChanged.RemoveListener(PositionMoved);


            //Debug.Log("//////////////////////////OnDisable////////////////////");
        }


        private void OnDestroy()
        {
            if (!(_lakePolygon != null))
                return;
            if (_lakePolygon == null)
                _lakePolygon = (LakePolygon) target;
            if (_lakePolygon.meshTerrainGOs is {Count: > 0})
            {
                foreach (var item in _lakePolygon.meshTerrainGOs)
                {
                    DestroyImmediate(item);
                }

                _lakePolygon.meshTerrainGOs.Clear();
            }
        }


        public override void OnInspectorGUI()
        {
            if (_lakePolygon == null)
                _lakePolygon = (LakePolygon) target;

            EditorGUILayout.Space();
            _logo = (Texture2D) Resources.Load("logoRAM");

            GUIContent btnTxt = new GUIContent(_logo);

            var rt = GUILayoutUtility.GetRect(btnTxt, GUI.skin.label, GUILayout.ExpandWidth(false));
            rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);

            GUI.Button(rt, btnTxt, GUI.skin.label);

            int toolbarNew = GUILayout.SelectionGrid(_lakePolygon.toolbarInt, toolbarStrings, 3, GUILayout.Height(125));


            if (_lakePolygon.transform.eulerAngles.magnitude != 0 || _lakePolygon.transform.localScale.x != 1 ||
                _lakePolygon.transform.localScale.y != 1 || _lakePolygon.transform.localScale.z != 1)
                EditorGUILayout.HelpBox("Lake should have scale (1,1,1) and rotation (0,0,0) during edit!",
                    MessageType.Error);


            if (toolbarNew == 0)
            {
                UIBaseSettings();
            }


            if (toolbarNew == 1)
            {
                NmSplineManager.PointsUI();
            }

            if (_lakePolygon.toolbarInt != toolbarNew)
            {
                if (toolbarNew == 2)
                {
                    VertexPainterEditor.GetMeshFilters(_lakePolygon.gameObject);
                }
                else if (_lakePolygon.toolbarInt == 2)
                {
                    VertexPainterEditor.ResetOldMaterials();
                }
            }

            if (toolbarNew == 2)
            {
                VertexPainterEditor.UIPainter();

                if (VertexPainterEditor.VertexPainterData.ToolbarInt == 1)
                    UIAutomaticFlowMap();
            }

            if (_lakePolygon.toolbarInt == 3)
            {
                UISimulation();
            }


            if (toolbarNew == 4)
            {
                if (_lakePolygon.toolbarInt != toolbarNew)
                {
                    if (_lakePolygon.TerrainManager != null && _lakePolygon.TerrainManager.PainterData && _lakePolygon.TerrainManager.PainterData.WorkTerrain == null)
                    {
                        _lakePolygon.GeneratePolygon();
                    }
                }

                TerrainManagerEditor.UITerrain();
            }

            if (_lakePolygon.toolbarInt == 5)
            {
                FilesManager();
            }

            if (toolbarNew == 6)
            {
                LakePolygonTips.Tips();
            }

            if (toolbarNew == 7)
            {
                toolbarNew = _lakePolygon.toolbarInt;
                string[] guids1 = AssetDatabase.FindAssets("RAM 2023 Manual");
                Application.OpenURL("file:///" + Application.dataPath.Replace("Assets", "") +
                                    AssetDatabase.GUIDToAssetPath(guids1[0]));
            }

            if (toolbarNew == 8)
            {
                toolbarNew = _lakePolygon.toolbarInt;
                Application.OpenURL("https://www.youtube.com/playlist?list=PLWMxYDHySK5MyWZsMYWSRtpn1glwcS99x");
            }

            if (toolbarNew == 9)
            {
                _lakePolygonVegetationStudio.UIVegetationStudio();
            }


            _lakePolygon.toolbarInt = toolbarNew;


            EditorGUILayout.Space();
        }

        private void UIAutomaticFlowMap()
        {
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Flow Map Automatic: ", EditorStyles.boldLabel);
            _lakePolygon.BaseProfile.automaticFlowMapScale = EditorGUILayout.FloatField("Automatic speed", _lakePolygon.BaseProfile.automaticFlowMapScale);
            _lakePolygon.BaseProfile.automaticFlowMapDistance = EditorGUILayout.FloatField("Automatic distance", _lakePolygon.BaseProfile.automaticFlowMapDistance);
            _lakePolygon.BaseProfile.automaticFlowMapDistanceBlend = EditorGUILayout.FloatField("Automatic distance blend", _lakePolygon.BaseProfile.automaticFlowMapDistanceBlend);
            _lakePolygon.BaseProfile.noiseFlowMap = EditorGUILayout.Toggle("Add noise", _lakePolygon.BaseProfile.noiseFlowMap);
            if (_lakePolygon.BaseProfile.noiseFlowMap)
            {
                EditorGUI.indentLevel++;
                _lakePolygon.BaseProfile.noiseMultiplierFlowMap =
                    EditorGUILayout.FloatField("Noise multiplier inside", _lakePolygon.BaseProfile.noiseMultiplierFlowMap);
                _lakePolygon.BaseProfile.noiseSizeXFlowMap = EditorGUILayout.FloatField("Noise scale X", _lakePolygon.BaseProfile.noiseSizeXFlowMap);
                _lakePolygon.BaseProfile.noiseSizeZFlowMap = EditorGUILayout.FloatField("Noise scale Z", _lakePolygon.BaseProfile.noiseSizeZFlowMap);
                EditorGUI.indentLevel--;
            }


            EditorGUILayout.Space();
            GUILayout.Label("Flow Map Physic: ", EditorStyles.boldLabel);
            _lakePolygon.floatSpeed = EditorGUILayout.FloatField("Float speed", _lakePolygon.floatSpeed);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_lakePolygon, "Lake changed");
                _lakePolygon.GeneratePolygon();
            }
        }

        private void UISimulation()
        {
            EditorGUILayout.HelpBox("\nSet 1 point and R.A.M will generate lake.\n", MessageType.Info);
            EditorGUILayout.Space();
            _lakePolygon.angleSimulation = EditorGUILayout.IntSlider("Angle", _lakePolygon.angleSimulation, 1, 180);
            _lakePolygon.closeDistanceSimulation =
                EditorGUILayout.FloatField("Point distance", _lakePolygon.closeDistanceSimulation);
            _lakePolygon.checkDistanceSimulation =
                EditorGUILayout.FloatField("Check distance", _lakePolygon.checkDistanceSimulation);
            _lakePolygon.removeFirstPointSimulation =
                EditorGUILayout.Toggle("Remove first point", _lakePolygon.removeFirstPointSimulation);
            if (GUILayout.Button("Simulate"))
            {
                _lakePolygon.LakePolygonSimulationGenerator.Simulation();
            }

            if (GUILayout.Button("Remove points except first"))
            {
                _lakePolygon.NmSpline.RemovePoints(0);
                _lakePolygon.meshfilter.sharedMesh = null;
            }

            if (GUILayout.Button("Remove all points"))
            {
                _lakePolygon.NmSpline.RemovePoints();
                _lakePolygon.meshfilter.sharedMesh = null;
            }
        }


        private void UIBaseSettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Add Point  - CTRL + Left Mouse Button Click \n" +
                                    "Add point between existing points - SHIFT + Left Button Click \n" +
                                    "Remove point - CTRL + SHIFT + Left Button Click", MessageType.Info);
            EditorGUILayout.Space();


            ProfileManage();


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(_lakePolygon, "Lake changed");

            _lakePolygon.lockHeight = EditorGUILayout.Toggle("Lock height", _lakePolygon.lockHeight);

            EditorGUILayout.BeginHorizontal();
            _lakePolygon.height = EditorGUILayout.FloatField(_lakePolygon.height);
            if (GUILayout.Button("Set heights"))
            {
                for (int i = 0; i < _lakePolygon.NmSpline.MainControlPoints.Count; i++)
                {
                    Vector4 point = _lakePolygon.NmSpline.MainControlPoints[i].position;
                    point.y = _lakePolygon.height - _lakePolygon.transform.position.y;
                    _lakePolygon.NmSpline.MainControlPoints[i].position = point;
                }

                _lakePolygon.GeneratePolygon();
            }

            EditorGUILayout.EndHorizontal();

            _lakePolygon.yOffset = EditorGUILayout.DelayedFloatField("Y offset mesh", _lakePolygon.yOffset);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();


            GUILayout.Label("Mesh settings:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            string meshResolution = "Triangles density" + "(" + _lakePolygon.trianglesGenerated + " tris)";

            EditorGUILayout.LabelField(meshResolution);

            if (_lakePolygon.vertsGenerated > 65000)
            {
                EditorGUILayout.HelpBox(
                    "Too many vertices for 16 bit mesh index buffer.  Mesh switched to 32 bit index buffer.",
                    MessageType.Warning);
            }


            _lakePolygon.BaseProfile.maximumTriangleAmount =
                EditorGUILayout.DelayedFloatField("Maximum triangle amount", _lakePolygon.BaseProfile.maximumTriangleAmount);
            if (_lakePolygon.BaseProfile.maximumTriangleAmount == 0)
                _lakePolygon.BaseProfile.maximumTriangleAmount = 50;
            _lakePolygon.BaseProfile.maximumTriangleSize =
                EditorGUILayout.DelayedFloatField("Maximum triangle size", _lakePolygon.BaseProfile.maximumTriangleSize);
            if (_lakePolygon.BaseProfile.maximumTriangleSize == 0)
                _lakePolygon.BaseProfile.maximumTriangleSize = 10;


            if (_lakePolygon.TriangleSizeByLimit > 0 && _lakePolygon.BaseProfile.maximumTriangleSize < _lakePolygon.TriangleSizeByLimit)
            {
                EditorGUILayout.HelpBox("Triangle size too small for triangle limit", MessageType.Warning);
            }


            _lakePolygon.BaseProfile.triangleDensity = EditorGUILayout.IntSlider("Spline density",
                (int) (_lakePolygon.BaseProfile.triangleDensity), 1, 100);
            _lakePolygon.BaseProfile.uvScale = EditorGUILayout.FloatField("UV scale", _lakePolygon.BaseProfile.uvScale);

            _lakePolygon.BaseProfile.depthCurve = EditorGUILayout.CurveField("Depth curve", _lakePolygon.BaseProfile.depthCurve);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            if (GUILayout.Button("Snap/Unsnap mesh to terrain"))
            {
                _lakePolygon.snapToTerrain = !_lakePolygon.snapToTerrain;
            }

            EditorGUI.indentLevel++;
            //spline.snapMask = EditorGUILayout.MaskField ("Layers", spline.snapMask, InternalEditorUtility.layers);
            _lakePolygon.BaseProfile.snapMask = LayerMaskField.ShowLayerMaskField("Layers", _lakePolygon.BaseProfile.snapMask, true);

            _lakePolygon.normalFromRaycast =
                EditorGUILayout.Toggle("Take Normal from terrain", _lakePolygon.normalFromRaycast);
            EditorGUI.indentLevel--;


            EditorGUILayout.Space();
            GUILayout.Label("Lightning settings:", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            _lakePolygon.BaseProfile.receiveShadows = EditorGUILayout.Toggle("Receive Shadows", _lakePolygon.BaseProfile.receiveShadows);

            _lakePolygon.BaseProfile.shadowCastingMode =
                (ShadowCastingMode) EditorGUILayout.EnumPopup("Shadow Casting Mode", _lakePolygon.BaseProfile.shadowCastingMode);
            EditorGUI.indentLevel--;


            EditorGUILayout.Space();
            GUILayout.Label("Optimization:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            _lakePolygon.generateMeshParts = EditorGUILayout.Toggle("Split mesh into submeshes", _lakePolygon.generateMeshParts);
            EditorGUI.indentLevel++;
            if (_lakePolygon.generateMeshParts)
                _lakePolygon.meshPartsCount = EditorGUILayout.IntSlider("Parts", _lakePolygon.meshPartsCount, 2, 100);
            EditorGUI.indentLevel--;
            _lakePolygon.generateLod = EditorGUILayout.Toggle("Generate lod system", _lakePolygon.generateLod);
            if (_lakePolygon.generateLod)
            {
                EditorGUI.indentLevel++;
                _lakePolygon.generateLodGPU = EditorGUILayout.Toggle("GPU system", _lakePolygon.generateLodGPU);

                EditorGUILayout.LabelField("Distances");
                EditorGUI.indentLevel++;
                float lod0 = EditorGUILayout.FloatField("LOD0:", _lakePolygon.lodDistance[2]);
                float lod1 = EditorGUILayout.FloatField("LOD1:", _lakePolygon.lodDistance[1]);
                float lod2 = EditorGUILayout.FloatField("LOD2:", _lakePolygon.lodDistance[0]);
                _lakePolygon.lodDistance[2] = lod0;
                _lakePolygon.lodDistance[1] = lod1;
                _lakePolygon.lodDistance[0] = lod2;
                EditorGUI.indentLevel--;

                // _lakePolygon.lodDistance = EditorGUILayout.Vector3Field("Distances", _lakePolygon.lodDistance);
                _lakePolygon.lodRefreshTime = EditorGUILayout.FloatField("Refresh time", _lakePolygon.lodRefreshTime);
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
            {
                _lakePolygon.GeneratePolygon();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate polygon"))
            {
                _lakePolygon.GeneratePolygon();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Export as mesh"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Save lake mesh", "", "asset", "Save lake mesh");


                if (path.Length != 0 && _lakePolygon.meshfilter.sharedMesh != null)
                {
                    AssetDatabase.CreateAsset(_lakePolygon.meshfilter.sharedMesh, path);

                    AssetDatabase.Refresh();
                    _lakePolygon.GeneratePolygon();
                }
            }
        }

        private void ProfileManage()
        {
            _lakePolygon.currentProfile = (LakePolygonProfile) EditorGUILayout.ObjectField("Lake profile",
                _lakePolygon.currentProfile, typeof(LakePolygonProfile), false);


            if (GUILayout.Button("Create profile from settings"))
            {
                LakePolygonProfile asset = CreateInstance<LakePolygonProfile>();

                MeshRenderer ren = _lakePolygon.GetComponent<MeshRenderer>();
                _lakePolygon.BaseProfile.lakeMaterial = ren.sharedMaterial;
                asset.SetProfileData(_lakePolygon.BaseProfile);


                string path = EditorUtility.SaveFilePanelInProject("Save new spline profile",
                    _lakePolygon.gameObject.name + ".asset", "asset",
                    "Please enter a file name to save the spline profile to");

                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    _lakePolygon.currentProfile = asset;
                }
            }

            if (_lakePolygon.currentProfile != null && GUILayout.Button("Save profile from settings"))
            {
                MeshRenderer ren = _lakePolygon.GetComponent<MeshRenderer>();
                _lakePolygon.BaseProfile.lakeMaterial = ren.sharedMaterial;

                _lakePolygon.currentProfile.SetProfileData(_lakePolygon.BaseProfile);
                EditorUtility.SetDirty(_lakePolygon.currentProfile);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }


            if (_lakePolygon.currentProfile != null && _lakePolygon.currentProfile != _lakePolygon.oldProfile)
            {
                ResetToProfile();
                _lakePolygon.GeneratePolygon();
                EditorUtility.SetDirty(_lakePolygon);
            }

            if (CheckProfileChange())
                EditorGUILayout.HelpBox("Profile data changed.", MessageType.Info);

            if (_lakePolygon.currentProfile != null && GUILayout.Button("Reset to profile"))
                if (EditorUtility.DisplayDialog("Reset to profile", "Are you sure you want to reset spline to profile?", "Reset", "Do Not Reset"))
                    ResetToProfile();
        }

        private void FilesManager()
        {
            if (GUILayout.Button("Save points to csv file"))
            {
                NMSplineExporter nmSplineExporter = new NMSplineExporter();
                nmSplineExporter.PointsToFile(_lakePolygon.NmSpline);
            }

            if (GUILayout.Button("Load points from csv file"))
            {
                NMSplineExporter nmSplineExporter = new NMSplineExporter();
                nmSplineExporter.PointsFromFile(_lakePolygon.NmSpline);
            }
        }


        private void OnSceneGUIInvoke(SceneView sceneView)
        {
            if (_lakePolygon == null)
                _lakePolygon = (LakePolygon) target;

            if (_lakePolygon == null)
                return;

            if (_lakePolygon.toolbarInt == 2 && VertexPainterEditor != null && VertexPainterEditor.OnSceneGUI(sceneView))
                return;


            if (_lakePolygon.lockHeight && _lakePolygon.NmSpline.MainControlPoints.Count > 1)
            {
                for (int i = 1; i < _lakePolygon.NmSpline.MainControlPoints.Count; i++)
                {
                    Vector4 vec = _lakePolygon.NmSpline.MainControlPoints[i].position;
                    vec.y = _lakePolygon.NmSpline.MainControlPoints[0].position.y;
                    _lakePolygon.NmSpline.MainControlPoints[i].position = vec;
                }
            }

            if (_lakePolygon.NmSpline == null)
            {
                Debug.Log("No NM spline");
                return;
            }


            if (_lakePolygon.NmSpline.Points == null)
            {
                _lakePolygon.GeneratePolygon();
                Debug.Log("No NM spline points");
                return;
            }

            Vector3[] points = new Vector3[_lakePolygon.NmSpline.Points.Count];


            for (int i = 0; i < _lakePolygon.NmSpline.Points.Count; i++)
            {
                points[i] = _lakePolygon.NmSpline.Points[i].Position + _lakePolygon.transform.position;
            }


            Handles.color = Color.white;
            if (points.Length > 1)
                Handles.DrawPolyLine(points);

            if (Event.current.commandName == "UndoRedoPerformed")
            {
                _lakePolygon.GeneratePolygon();
                return;
            }


            if (_dragged && Event.current.type == EventType.MouseUp)
            {
                _dragged = false;
                _lakePolygon.GeneratePolygon();
            }


            NmSplineManager.SceneGUI(_lakePolygon);

            if (_lakePolygon.toolbarInt == 4)
                TerrainManagerEditor.OnSceneGui();
        }

        private void RegeneratePolygon()
        {
            _lakePolygon.GeneratePolygon();
        }

        private void PositionMoved()
        {
            _dragged = true;

            Undo.RecordObject(_lakePolygon, "Change Position");
            _lakePolygon.GeneratePolygon(true);

            _lakePolygonVegetationStudio.RegenerateVegetationStudio();
        }


        private bool CheckProfileChange()
        {
            return _lakePolygon.currentProfile != null && _lakePolygon.BaseProfile.CheckProfileChange(_lakePolygon.currentProfile);
        }

        public void ResetToProfile()
        {
            if (_lakePolygon == null)
                _lakePolygon = (LakePolygon) target;


            _lakePolygon.BaseProfile.SetProfileData(_lakePolygon.currentProfile);

            var ren = _lakePolygon.GetComponent<MeshRenderer>();
            ren.sharedMaterial = _lakePolygon.BaseProfile.lakeMaterial;

            _lakePolygon.oldProfile = _lakePolygon.currentProfile;
        }
    }
}