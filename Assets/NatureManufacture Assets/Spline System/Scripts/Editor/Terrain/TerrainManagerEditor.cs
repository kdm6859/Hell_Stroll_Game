//  * Created by Pawel Homenko on  08/2022
//  */


using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TerrainTools;

namespace NatureManufacture.RAM.Editor
{
    public class TerrainManagerEditor
    {
        public TerrainManagerEditor(TerrainManager baseTerrainManager)
        {
            BaseTerrainManager = baseTerrainManager;
            GetTerrains = new UnityEvent();


            _terrainPainterDataEditor = (TerrainPainterDataEditor) UnityEditor.Editor.CreateEditor(BaseTerrainManager.PainterData);
            if (BaseTerrainManager.PainterData != null)
                _terrainLayersDataCount = BaseTerrainManager.PainterData.TerrainLayersData.Count;
        }

        private bool _showCarveTerrain;
        private TerrainManager BaseTerrainManager { get; }

        public UnityEvent GetTerrains { get; set; }

        private int _terrainLayersDataCount = 0;

        public bool ShowCarveTerrain
        {
            get => _showCarveTerrain;
            set => _showCarveTerrain = value;
        }

        private TerrainPainterDataEditor _terrainPainterDataEditor;

        public void UITerrain()
        {
            if (BaseTerrainManager.PainterData != null)
            {
                if (_terrainLayersDataCount == 0 && BaseTerrainManager.PainterData.TerrainLayersData.Count == 1)
                {
                    BaseTerrainManager.PainterData.TerrainLayersData[0].Reset();
                }

                _terrainLayersDataCount = BaseTerrainManager.PainterData.TerrainLayersData.Count;
            }

            //_lakePolygon.LakePolygonTerrainManager.paramsTexture = (Texture2D) EditorGUILayout.ObjectField("Add a Texture:", _lakePolygon.LakePolygonTerrainManager.paramsTexture, typeof(Texture2D), false);
            //TerrainManager.Texture2DCarve = (RenderTexture) EditorGUILayout.ObjectField("Add a Texture:", TerrainManager.Texture2DCarve, typeof(RenderTexture), false);
            //BaseTerrainManager.Texture2DCarve = (RenderTexture) EditorGUILayout.ObjectField("Add a Texture:", BaseTerrainManager.Texture2DCarve, typeof(RenderTexture), false);
            //if (GUILayout.Button("save carve texture"))
            //    TextureManager.SaveTexture(TerrainManager.Texture2DCarve, TextureFormat.ARGB32);

            /*
              TerrainManager.Texture2DTest = (RenderTexture) EditorGUILayout.ObjectField("Add a Texture:", TerrainManager.Texture2DTest, 
                  typeof(RenderTexture), false,GUILayout.MinWidth(512), GUILayout.MinHeight(512));
              if (GUILayout.Button("save paint texture"))
                  TextureManager.SaveTexture(TerrainManager.Texture2DTest, TextureFormat.ARGB32);
  */
            BaseTerrainManager.TerrainPainterGetData = (TerrainPainterData) EditorGUILayout.ObjectField("Terrain Painter Profile", BaseTerrainManager.TerrainPainterGetData, typeof(TerrainPainterData), false);


            if (GUILayout.Button("Create profile from settings"))
            {
                var asset = ScriptableObject.CreateInstance<TerrainPainterData>();

                asset.SetProfileData(BaseTerrainManager.PainterData);

                string path = EditorUtility.SaveFilePanelInProject("Save new spline profile",
                    BaseTerrainManager.NmSpline.gameObject.name + ".asset", "asset", "Please enter a file name to save the spline profile to");

                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    BaseTerrainManager.TerrainPainterGetData = asset;
                }
            }

            if (BaseTerrainManager.TerrainPainterGetData != null && GUILayout.Button("Save profile from settings"))
            {
                //spline.currentProfile.meshCurve = spline.meshCurve;


                BaseTerrainManager.TerrainPainterGetData.SetProfileData(BaseTerrainManager.PainterData);

                EditorUtility.SetDirty(BaseTerrainManager.TerrainPainterGetData);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }


            if (BaseTerrainManager.TerrainPainterGetData != null && BaseTerrainManager.TerrainPainterGetData != BaseTerrainManager.TerrainPainterGetDataOld)
            {
                ResetToProfile();
                EditorUtility.SetDirty(BaseTerrainManager.PainterData);
            }

            bool profileChanged = CheckProfileChange();


            if (BaseTerrainManager.TerrainPainterGetData != null &&
                GUILayout.Button("Reset to profile" + (profileChanged ? " (Profile data changed)" : "")))
                if (EditorUtility.DisplayDialog("Reset to profile", "Are you sure you want to reset spline to profile?", "Reset", "Do Not Reset"))
                    ResetToProfile();

            if (BaseTerrainManager.PainterData == null)
                return;


            int terrainNumber = BaseTerrainManager.PainterData.TerrainsUnder.Count;


            string[] optionsTerrain = new string[terrainNumber];
            for (int i = 0; i < terrainNumber; i++)
            {
                optionsTerrain[i] = i + " - ";

                if (BaseTerrainManager.PainterData.TerrainsUnder[i] != null) optionsTerrain[i] += BaseTerrainManager.PainterData.TerrainsUnder[i].name;
            }


            if (BaseTerrainManager.PainterData.TerrainsUnder is {Count: > 0} &&
                BaseTerrainManager.PainterData.CurrentWorkTerrain >= BaseTerrainManager.PainterData.TerrainsUnder.Count)
            {
                BaseTerrainManager.PainterData.CurrentWorkTerrain = 0;
                BaseTerrainManager.PainterData.WorkTerrain = BaseTerrainManager.PainterData.TerrainsUnder[BaseTerrainManager.PainterData.CurrentWorkTerrain];
            }
            else if (BaseTerrainManager.PainterData.TerrainsUnder == null || BaseTerrainManager.PainterData.TerrainsUnder.Count == 0)
            {
                BaseTerrainManager.PainterData.WorkTerrain = null;
            }
            else
            {
                BaseTerrainManager.PainterData.WorkTerrain = BaseTerrainManager.PainterData.TerrainsUnder[BaseTerrainManager.PainterData.CurrentWorkTerrain];
            }

            if (BaseTerrainManager.PainterData.WorkTerrain == null)
            {
                EditorGUILayout.HelpBox("No terrain under Spline, add terrain and regenerate Spline", MessageType.Warning);
                if (GUILayout.Button("Generate polygon"))
                {
                    GetTerrains?.Invoke();
                }

                return;
            }

            EditorGUILayout.Space();

            BaseTerrainManager.PainterData.CurrentWorkTerrain = EditorGUILayout.Popup("Terrain:", BaseTerrainManager.PainterData.CurrentWorkTerrain, optionsTerrain);
            BaseTerrainManager.PainterData.WorkTerrain = BaseTerrainManager.PainterData.TerrainsUnder[BaseTerrainManager.PainterData.CurrentWorkTerrain];


            if (BaseTerrainManager.PainterData != null && BaseTerrainManager.PainterData.WorkTerrain != null &&
                BaseTerrainManager.PainterData.WorkTerrain.terrainData != null)
            {
                if (_terrainPainterDataEditor == null || _terrainPainterDataEditor != null && _terrainPainterDataEditor.target != BaseTerrainManager.PainterData)
                {
                    _terrainPainterDataEditor = (TerrainPainterDataEditor) UnityEditor.Editor.CreateEditor(BaseTerrainManager.PainterData);
                }

                if (BaseTerrainManager.TerrainCarveQuality == 0)
                    BaseTerrainManager.TerrainCarveQuality = TerrainManager.TerrainCarveQualityEnum.Standard;

                BaseTerrainManager.TerrainCarveQuality = (TerrainManager.TerrainCarveQualityEnum) EditorGUILayout.EnumPopup("Brush quality", BaseTerrainManager.TerrainCarveQuality);
                bool carveDataChange = _terrainPainterDataEditor.UIBrushAdditional();

                EditorGUILayout.Space();

                BaseTerrainManager.PainterData.ToolbarInt = GUILayout.Toolbar(BaseTerrainManager.PainterData.ToolbarInt, BaseTerrainManager.PainterData.ToolbarStrings);


                if (BaseTerrainManager.PainterData.ToolbarInt == 0)
                {
                    //EditorGUI.indentLevel++;


                    EditorGUILayout.Space();

                    carveDataChange = carveDataChange || _terrainPainterDataEditor.UICarve();


                    //EditorGUI.indentLevel--;

                    if (!ShowCarveTerrain)
                    {
                        if (GUILayout.Button("Show Terrain Carve"))
                        {
                            ShowCarveTerrain = true;
                            BaseTerrainManager.GenerateTerrainBrushTexture(BaseTerrainManager.PainterData.WorkTerrain);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Hide Terrain Carve"))
                        {
                            ShowCarveTerrain = false;
                        }
                    }


                    if (GUILayout.Button("Carve Terrain"))
                    {
                        ShowCarveTerrain = false;
                        BaseTerrainManager.CarveTerrain();
                    }

                    if (carveDataChange)
                    {
                        if (ShowCarveTerrain)
                            BaseTerrainManager.GenerateTerrainBrushTexture(BaseTerrainManager.PainterData.WorkTerrain);
                    }
                }
                else if (BaseTerrainManager.PainterData.ToolbarInt == 1)
                {
                    ShowCarveTerrain = false;
                    EditorGUILayout.Space();
                    //EditorGUI.indentLevel++;

                    //_lakePolygon.TerrainPainterData.TerrainLayerData.power =EditorGUILayout.CurveField("Terrain paint", _lakePolygon.TerrainPainterData.TerrainLayerData.power);

                    int splatNumber = BaseTerrainManager.PainterData.WorkTerrain.terrainData.terrainLayers.Length;
                    if (splatNumber > 0)
                    {
                        if (BaseTerrainManager.PainterData != null)
                        {
                            _terrainPainterDataEditor.UILayers(true);
                        }


                        if (GUILayout.Button("Paint Terrain"))
                        {
                            BaseTerrainManager.PaintTerrain();
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Terrain has no Splatmaps.", MessageType.Info);
                    }
                }
                else
                {
                    ShowCarveTerrain = false;
                    EditorGUILayout.Space();
                    _terrainPainterDataEditor.UIDistanceClearFoliage();


                    if (GUILayout.Button("Remove Details Foliage"))
                    {
                        ShowCarveTerrain = false;
                        BaseTerrainManager.TerrainClearTrees();
                    }

                    _terrainPainterDataEditor.UIDistanceClearFoliageTrees();

                    if (GUILayout.Button("Remove Trees"))
                    {
                        ShowCarveTerrain = false;
                        BaseTerrainManager.TerrainClearTrees(false);
                    }
                }
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("No Terrain On Scene. Try to regenerate lake.", MessageType.Info);

                if (GUILayout.Button("Remove Trees"))
                {
                    ShowCarveTerrain = false;
                    BaseTerrainManager.TerrainClearTrees(false);
                }
            }
        }

        private bool CheckProfileChange()
        {
            return BaseTerrainManager.PainterData.CheckProfileChange(BaseTerrainManager.TerrainPainterGetData);
        }

        private void ResetToProfile()
        {
            BaseTerrainManager.PainterData.SetProfileData(BaseTerrainManager.TerrainPainterGetData);

            BaseTerrainManager.TerrainPainterGetDataOld = BaseTerrainManager.TerrainPainterGetData;
        }

        public void OnSceneGui()
        {
            if (!_showCarveTerrain)
                return;


            // Don't render preview if this isn't a repaint. losing performance if we do
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }


            //Debug.Log("DrawBrushPreview " + texture2D + " " + terrain.name);

            //Debug.Log($"textureCoord {raycastHit.textureCoord.x} {raycastHit.textureCoord.y}");
            BrushTransform brushTransform = TerrainPaintUtility.CalculateBrushTransform(BaseTerrainManager.TerrainUV, BaseTerrainManager.TextureCoord, BaseTerrainManager.BrushSize, 0);

            //Debug.Log(brushXform.brushOrigin + " " + brushXform.targetOrigin);
            PaintContext paintContext = TerrainPaintUtility.BeginPaintHeightmap(BaseTerrainManager.TerrainUV, brushTransform.GetBrushXYBounds(), 1);

            Material material = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();
            // Material material =  new Material(Shader.Find("Debug Terrain Carve"));

            TerrainPaintUtilityEditor.DrawBrushPreview(
                paintContext, TerrainBrushPreviewMode.SourceRenderTexture, BaseTerrainManager.Texture2DCarve, brushTransform, material, 0);


            // draw result preview
            {
                //Debug.Log($"strength {_newHeight / terrain.terrainData.size.y}");
                BaseTerrainManager.BrushCarve(paintContext, BaseTerrainManager.PainterData.Smooth, BaseTerrainManager.ParamsTextureCarve,
                    BaseTerrainManager.PainterData.TerrainNoiseParametersCarve, BaseTerrainManager.Texture2DCarve, brushTransform);

                // restore old render target
                RenderTexture.active = paintContext.oldRenderTexture;

                material.SetTexture(TerrainManager.HeightmapOrig, paintContext.sourceRenderTexture);
                TerrainPaintUtilityEditor.DrawBrushPreview(
                    paintContext, TerrainBrushPreviewMode.DestinationRenderTexture, BaseTerrainManager.Texture2DCarve, brushTransform, material, 2);
            }

            TerrainPaintUtility.ReleaseContextResources(paintContext);
        }
    }
}