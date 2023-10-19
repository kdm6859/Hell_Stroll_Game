// /**
//  * Created by Pawel Homenko on  08/2022
//  */

using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.TerrainTools;
using Object = UnityEngine.Object;

namespace NatureManufacture.RAM
{
    [Serializable]
    public class TerrainManager
    {
        public TerrainManager(NmSpline nmNmSpline, ITerrainPainterGetData terrainPainterGetData)
        {
            this._terrainPainterGetData = terrainPainterGetData;
            baseTerrainPainterData = ScriptableObject.CreateInstance<TerrainPainterData>();
            terrainCarveQuality = TerrainCarveQualityEnum.Standard;
            NmSpline = nmNmSpline;
        }

        public enum TerrainCarveQualityEnum
        {
            Low = 1024,
            Standard = 2048,
            High = 4096
        }


        public struct CarveData
        {
            public Vector4[,] Distances;
            public float MaxX;
            public float MaxZ;
            public float MinX;
            public float MinZ;

            public CarveData(Vector4[,] distances, float maxX, float maxZ, float minX, float minZ)
            {
                Distances = distances;
                MaxX = maxX;
                MaxZ = maxZ;
                MinX = minX;
                MinZ = minZ;
            }
        }


        [SerializeField] private TerrainCarveQualityEnum terrainCarveQuality = TerrainCarveQualityEnum.Standard;
        [SerializeField] private NmSpline nmNmSpline;

        private ITerrainPainterGetData _terrainPainterGetData;
        [SerializeField] private TerrainPainterData baseTerrainPainterData;
        [SerializeField] private TerrainPainterData terrainPainterGetDataOld;
        private RenderTexture _texture2DCarve;
        private RenderTexture _texture2DMask;
        private RenderTexture _texture2DTest;
        private Texture2D _paramsTextureCarve;
        private RenderTexture _texture2DPaint;
        private Bounds _brushBounds;
        private Vector3 _brushCenter;
        private float _brushSize;
        private Vector2 _textureCoord = Vector3.zero;
        private Terrain _terrainUV;

        private static readonly int BrushTex = Shader.PropertyToID("_BrushTex");
        private static readonly int ParamsTex = Shader.PropertyToID("_ParamsTex");
        private static readonly int BrushParams = Shader.PropertyToID("_BrushParams");
        private static readonly int NoiseParams = Shader.PropertyToID("_NoiseParams");
        public static readonly int HeightmapOrig = Shader.PropertyToID("_HeightmapOrig");

        private CarveData _clearData;
        private float _terrainHeight;
        private static readonly int HeightMax = Shader.PropertyToID("_heightMax");
        private static readonly int BlurAdditionalSize = Shader.PropertyToID("_BlurAdditionalSize");
        private static readonly int BlurSize = Shader.PropertyToID("_BlurSize");
        private static readonly int ConvexParams = Shader.PropertyToID("_ConvexParams");
        private static readonly int MaskTex = Shader.PropertyToID("_MaskTex");
        private static readonly int NoiseParamsSecond = Shader.PropertyToID("_NoiseParamsSecond");
        private static readonly int MainParams = Shader.PropertyToID("_MainParams");
        private static readonly int PassNumber = Shader.PropertyToID("_PassNumber");
        private static readonly int HeightOffset = Shader.PropertyToID("_heightOffset");


        public TerrainCarveQualityEnum TerrainCarveQuality
        {
            get => terrainCarveQuality;
            set => terrainCarveQuality = value;
        }

        public RenderTexture Texture2DPaint
        {
            get => _texture2DPaint;
            set => _texture2DPaint = value;
        }

        public CarveData ClearData
        {
            get => _clearData;
            set => _clearData = value;
        }

        public RenderTexture Texture2DCarve
        {
            get => _texture2DCarve;
            set => _texture2DCarve = value;
        }

        public Vector2 TextureCoord
        {
            get => _textureCoord;
            set => _textureCoord = value;
        }

        public float BrushSize
        {
            get => _brushSize;
            set => _brushSize = value;
        }

        public NmSpline NmSpline
        {
            get => nmNmSpline;
            set => nmNmSpline = value;
        }

        public Terrain TerrainUV
        {
            get => _terrainUV;
            set => _terrainUV = value;
        }


        public Texture2D ParamsTextureCarve
        {
            get => _paramsTextureCarve;
            set => _paramsTextureCarve = value;
        }

        public RenderTexture Texture2DMask
        {
            get => _texture2DMask;
            set => _texture2DMask = value;
        }

        public RenderTexture Texture2DTest
        {
            get => _texture2DTest;
            set => _texture2DTest = value;
        }

        public TerrainPainterData TerrainPainterGetData
        {
            get
            {
                _terrainPainterGetData ??= NmSpline.GetComponent<ITerrainPainterGetData>();

                return _terrainPainterGetData.PainterData;
            }
            set => _terrainPainterGetData.PainterData = value;
        }

        public TerrainPainterData PainterData
        {
            get
            {
                baseTerrainPainterData ??= ScriptableObject.CreateInstance<TerrainPainterData>();

                return baseTerrainPainterData;
            }
        }

        public TerrainPainterData TerrainPainterGetDataOld
        {
            get => terrainPainterGetDataOld;
            set => terrainPainterGetDataOld = value;
        }


        #region Generate Brush

        public void GenerateTerrainBrushTexture(Terrain terrain)
        {
            if (_texture2DCarve)
            {
                RenderTexture.ReleaseTemporary(_texture2DCarve);
            }

            if (_texture2DMask)
            {
                RenderTexture.ReleaseTemporary(_texture2DMask);
            }

            if (_texture2DTest)
            {
                RenderTexture.ReleaseTemporary(_texture2DTest);
            }


            _terrainHeight = terrain.terrainData.size.y;

            MeshRenderer renderer = NmSpline.Transform.GetComponent<MeshRenderer>();


            var bounds = renderer.bounds;
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            float extent = Mathf.Max(extents.x, extents.z);

            float minX = center.x - extent;
            float maxX = center.x + extent;

            minX -= PainterData.BrushAdditionalSize;
            maxX += PainterData.BrushAdditionalSize;

            BrushSize = maxX - minX;

            _brushCenter = center;
            _brushBounds = bounds;
            //Debug.Log($"brush center {center} brush bounds {bounds} brush size {BrushSize}");

            GetTerrainUV();
            Texture2DCarve = GenerateBrushFromCamera();

            int textureParamsSize = (int) (0.5f * (int) terrainCarveQuality);
            ushort[] paramsColors = new ushort[textureParamsSize];

            ParamsTextureCarve = new Texture2D(textureParamsSize, 1, TextureFormat.R16, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            for (int i = 0; i < textureParamsSize; i++)
            {
                float power = RamMath.Remap(i, 0, textureParamsSize - 1, -BrushSize, BrushSize);

                power = PainterData.TerrainCarve.Evaluate(power);

                //power = 100 * i / (float) textureParamsSize;
                power = RamMath.Remap(power, -_terrainHeight, _terrainHeight, 0, 1);

                //Debug.Log(i + " " + power);

                paramsColors[i] = (ushort) (power * ushort.MaxValue);
            }

            ParamsTextureCarve.SetPixelData(paramsColors, 0);
            ParamsTextureCarve.Apply();
        }

        /// <summary>
        /// r - height of spline
        /// g - distance to border
        /// b - mask of spline
        /// a - blend distance by brush additional
        /// </summary>
        /// <returns></returns>
        private RenderTexture GenerateBrushFromCamera()
        {
            var currentActiveRT = RenderTexture.active;

            var depthRenderTexture = GenerateDepthMask();


            RenderTexture renderTexture = RenderTexture.GetTemporary(depthRenderTexture.width, depthRenderTexture.height, 0, RenderTextureFormat.ARGB64, RenderTextureReadWrite.Linear);
            RenderTexture renderTextureSecond = RenderTexture.GetTemporary(depthRenderTexture.width, depthRenderTexture.height, 0, RenderTextureFormat.ARGB64, RenderTextureReadWrite.Linear);

            Material materialRenderBrush = new Material(Shader.Find("Hidden/NatureManufacture Shaders/BrushGenerator"));

            Graphics.Blit(depthRenderTexture, renderTexture, materialRenderBrush, 1);

            float brushAdditionalSize = PainterData.BrushAdditionalSize / BrushSize;
            materialRenderBrush.SetFloat(BlurAdditionalSize, brushAdditionalSize);

            for (int i = 0; i < (int) (0.25f * (int) terrainCarveQuality); i++)
            {
                materialRenderBrush.SetFloat(BlurSize, (i * 2) / (float) (depthRenderTexture.height));
                materialRenderBrush.SetFloat(PassNumber, i * 2);
                Graphics.Blit(renderTexture, renderTextureSecond, materialRenderBrush, 2);
                materialRenderBrush.SetFloat(BlurSize, (i * 2 + 1) / (float) (depthRenderTexture.height));
                materialRenderBrush.SetFloat(PassNumber, i * 2 + 1);
                Graphics.Blit(renderTextureSecond, renderTexture, materialRenderBrush, 2);
            }


            for (int i = 0; i < 0.25f * (int) terrainCarveQuality; i++)
            {
                Graphics.Blit(renderTexture, renderTextureSecond, materialRenderBrush, 3);
                Graphics.Blit(renderTextureSecond, renderTexture, materialRenderBrush, 3);
            }


            Graphics.Blit(renderTexture, depthRenderTexture, materialRenderBrush, 0);


            RenderTexture.ReleaseTemporary(renderTexture);
            RenderTexture.ReleaseTemporary(renderTextureSecond);

            RenderTexture.active = currentActiveRT;
            return depthRenderTexture;
        }

        private RenderTexture GenerateDepthMask()
        {
            MeshFilter filter = NmSpline.Transform.GetComponent<MeshFilter>();

            GameObject cameraGameObject = new GameObject();
            Camera depthCamera = cameraGameObject.AddComponent<Camera>();
            Vector3 position = _brushCenter;
            position.y = _brushBounds.max.y + 10;

            depthCamera.transform.position = position;
            depthCamera.transform.LookAt(_brushCenter);
            depthCamera.nearClipPlane = 0.1f;
            depthCamera.farClipPlane = _brushBounds.size.y + 10;
            depthCamera.orthographic = true;
            depthCamera.orthographicSize = _brushSize * 0.5f;
            depthCamera.rect = new Rect(0, 0, 1, 1);

            Material depthMaterial = new Material(Shader.Find("Hidden/NatureManufacture Shaders/WorldDepth"));
            depthMaterial.SetFloat(HeightMax, _terrainHeight);
            depthMaterial.SetFloat(HeightOffset, -_terrainUV.transform.position.y);
            Matrix4x4 orthoMatrix = Matrix4x4.Ortho(-_brushSize * 0.5f, _brushSize * 0.5f, -_brushSize * 0.5f, _brushSize * 0.5f, 2, 2000);

            RenderTexture depthRenderTexture = RenderTexture.GetTemporary((int) terrainCarveQuality, (int) terrainCarveQuality, 0, RenderTextureFormat.ARGB64, RenderTextureReadWrite.Linear);
            CommandBuffer depthCommandBuffer = new CommandBuffer();
            depthCommandBuffer.name = "ModelWorldDepthBaker";


            depthCommandBuffer.Clear();
            depthCommandBuffer.SetRenderTarget(depthRenderTexture);
            depthCommandBuffer.ClearRenderTarget(true, true, Color.black);
            depthCommandBuffer.SetViewProjectionMatrices(depthCamera.worldToCameraMatrix, orthoMatrix);
            //Debug.Log($"{filter.sharedMesh} {NmSpline.Transform}");
            depthCommandBuffer.DrawMesh(filter.sharedMesh, NmSpline.Transform.localToWorldMatrix, depthMaterial, 0);
            Graphics.ExecuteCommandBuffer(depthCommandBuffer);
            depthCommandBuffer.Release();
            Object.DestroyImmediate(cameraGameObject);
            return depthRenderTexture;
        }

        #endregion

        private void GetTerrainUV()
        {
            // Debug.DrawLine(_brushCenter, _brushCenter + Vector3.up * 1000, Color.cyan, 20);
            Ray ray = new Ray(_brushCenter + Vector3.up * 10000, Vector3.down);

            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

            if (hits.Length <= 0) return;


            foreach (var hit in hits)
            {
                if (hit.collider is not TerrainCollider) continue;


                TextureCoord = hit.textureCoord;
                TerrainUV = hit.collider.GetComponent<Terrain>();
                break;
            }
        }

        #region Carve Terrain

        public void CarveTerrain()
        {
            GenerateTerrainBrushTexture(PainterData.WorkTerrain);
            if (TerrainUV == null)
                return;

#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(TerrainUV.terrainData, "Carve");
#endif


            BrushTransform brushTransform = TerrainPaintUtility.CalculateBrushTransform(TerrainUV, TextureCoord, BrushSize, 0);
            PaintContext paintContext = TerrainPaintUtility.BeginPaintHeightmap(TerrainUV, brushTransform.GetBrushXYBounds());

            BrushCarve(paintContext, PainterData.Smooth, ParamsTextureCarve, PainterData.TerrainNoiseParametersCarve, Texture2DCarve, brushTransform);

            TerrainPaintUtility.EndPaintHeightmap(paintContext, "Terrain Paint - Raise or Lower Height");
            //Debug.Log(paintContext.terrainCount);

            //get terraincount from paint context then loop through terrain 
            //and call tterrain.terrainData.SyncHeightmap() on each terrain
            int terrainCount = paintContext.terrainCount;
            for (int i = 0; i < terrainCount; i++)
            {
                Terrain terrain = paintContext.GetTerrain(i);
                terrain.terrainData.SyncHeightmap();
            }
        }


        public void BrushCarve(PaintContext paintContext, float brushStrength, Texture2D paramsTexture, TerrainNoiseParameters terrainNoiseParameters, Texture brushTexture, BrushTransform brushTransform)
        {
            Material mat = Object.Instantiate(TerrainPaintUtility.GetBuiltinPaintMaterial());
            mat.shader = Shader.Find("Hidden/NatureManufacture Shaders/PaintHeight");

            Vector4 noiseParams = new Vector4(terrainNoiseParameters.NoiseMultiplierInside, terrainNoiseParameters.NoiseSizeX, terrainNoiseParameters.NoiseSizeZ, terrainNoiseParameters.NoiseMultiplierOutside);
            mat.SetVector(NoiseParams, noiseParams);
            Vector4 noiseParamsSecond = new Vector4(terrainNoiseParameters.NoiseMultiplierPower, 0, 0, 0);
            mat.SetVector(NoiseParamsSecond, noiseParamsSecond);

            Vector4 brushParams = new Vector4(brushStrength, terrainNoiseParameters.UseNoise ? 1 : 0, 1, 0.0f);
            mat.SetTexture(BrushTex, brushTexture);
            mat.SetTexture(ParamsTex, paramsTexture);
            mat.SetVector(BrushParams, brushParams);


            TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushTransform, mat);

            Graphics.Blit(paintContext.sourceRenderTexture, paintContext.destinationRenderTexture, mat, (int) TerrainBuiltinPaintMaterialPasses.StampHeight);
        }

        #endregion

        #region Paint Terrain

        public void PaintTerrain()
        {
            if (_texture2DCarve)
            {
                RenderTexture.ReleaseTemporary(_texture2DCarve);
            }

            if (_texture2DMask)
            {
                RenderTexture.ReleaseTemporary(_texture2DMask);
            }

            if (_texture2DTest)
            {
                RenderTexture.ReleaseTemporary(_texture2DTest);
            }


            GenerateTerrainBrushTexture(PainterData.WorkTerrain);


            if (TerrainUV == null)
                return;

            if (TerrainUV.terrainData.terrainLayers.Length == 0)
                return;

            if (PainterData.WorkTerrain.terrainData.terrainLayers.Length == 0)
                return;


            BrushTransform brushTransform = TerrainPaintUtility.CalculateBrushTransform(TerrainUV, TextureCoord, BrushSize, 0.0f);

            int textureParamsSize = (int) (0.5f * (int) terrainCarveQuality);
            Color[] paramsColors = new Color[textureParamsSize];
            foreach (var terrainLayerData in PainterData.TerrainLayersData)
            {
                if (!terrainLayerData.IsActive)
                    continue;
                int layerID = terrainLayerData.SplatMapID;

                if (PainterData.WorkTerrain.terrainData.terrainLayers.Length <= layerID)
                {
                    Debug.LogWarning($"Error in paint layer \"{terrainLayerData.LayerName}\" - Terrain \"{PainterData.WorkTerrain.name}\" does not have {layerID + 1} layers.");
                    continue;
                }

                PaintContext paintContext = TerrainPaintUtility.BeginPaintTexture(TerrainUV, brushTransform.GetBrushXYBounds(), PainterData.WorkTerrain.terrainData.terrainLayers[layerID]);


                Texture2D paramsTexture = new Texture2D(textureParamsSize, 1, TextureFormat.RGB24, false)
                {
                    wrapMode = TextureWrapMode.Clamp
                };
                for (int i = 0; i < textureParamsSize; i++)
                {
                    float angle = terrainLayerData.Angle.Evaluate(90 * i / (float) textureParamsSize);
                    float power = terrainLayerData.Power.Evaluate(RamMath.Remap(i, 0, textureParamsSize - 1, -BrushSize, BrushSize));

                    float height = terrainLayerData.Height.Evaluate(RamMath.Remap(i, 0, textureParamsSize - 1, 0, TerrainUV.terrainData.size.y));


                    paramsColors[i] = new Color(angle, power, height);
                }

                paramsTexture.SetPixels(paramsColors, 0);
                paramsTexture.Apply();

                Vector4 mainParams = new Vector4(terrainLayerData.PowerMultiplier, terrainLayerData.HeightMultiplier, terrainLayerData.HeightPower, 0.0f);

                Texture2DMask = Texture2DTest = GenerateMaskFromTerrainHeight(brushTransform, terrainLayerData.ConvexParameters);

                BrushPaint(paintContext, mainParams, terrainLayerData.NoiseParameters.UseNoise ? 1 : 0, paramsTexture, terrainLayerData.NoiseParameters, Texture2DMask, Texture2DCarve, brushTransform);

                RenderTexture.ReleaseTemporary(Texture2DMask);
                TerrainPaintUtility.EndPaintTexture(paintContext, "Terrain Paint - Texture");
            }
        }

        private RenderTexture GenerateMaskFromTerrainHeight(BrushTransform brushTransform, TerrainConvexParameters convexParameters)
        {
            PaintContext paintContextHeight = TerrainPaintUtility.BeginPaintHeightmap(TerrainUV, brushTransform.GetBrushXYBounds());
            RenderTexture source = paintContextHeight.sourceRenderTexture;
            RenderTexture renderTexture = RenderTexture.GetTemporary(source.width, source.height, source.depth, RenderTextureFormat.ARGB64);


            Material materialPaintHeight = new Material(Shader.Find("Hidden/NatureManufacture Shaders/PaintHeight"));

            Vector4 brushParams = new Vector4(0, 0, TerrainUV.terrainData.size.y, 0);
            materialPaintHeight.SetVector(BrushParams, brushParams);

            //Debug.Log((int)convexParameters.Convex);

            Vector4 convexParams = new Vector4(convexParameters.Steps, convexParameters.StepSize, convexParameters.Strength, (int) convexParameters.Convex);
            materialPaintHeight.SetVector(ConvexParams, convexParams);


            TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContextHeight, brushTransform, materialPaintHeight);

            Graphics.Blit(paintContextHeight.sourceRenderTexture, renderTexture, materialPaintHeight, 3);
            Graphics.Blit(paintContextHeight.sourceRenderTexture, paintContextHeight.destinationRenderTexture, materialPaintHeight, 0);

            TerrainPaintUtility.EndPaintHeightmap(paintContextHeight, "Generate normal");

            return renderTexture;
        }


        private void BrushPaint(PaintContext paintContext, Vector4 mainParams, float noise, Texture2D paramsTexture, TerrainNoiseParameters terrainNoiseParameters, RenderTexture maskTexture, Texture brushTexture, BrushTransform brushTransform)
        {
            Material materialPaintHeight = Object.Instantiate(TerrainPaintUtility.GetBuiltinPaintMaterial());
            materialPaintHeight.shader = Shader.Find("Hidden/NatureManufacture Shaders/PaintHeight");

            Vector4 brushParams = new Vector4(1, noise, TerrainUV.terrainData.size.y, 0.0f);

            materialPaintHeight.SetTexture(BrushTex, brushTexture);
            materialPaintHeight.SetTexture(MaskTex, maskTexture);
            materialPaintHeight.SetTexture(ParamsTex, paramsTexture);
            materialPaintHeight.SetVector(BrushParams, brushParams);
            materialPaintHeight.SetVector(MainParams, mainParams);


            Vector4 noiseParams = new Vector4(terrainNoiseParameters.NoiseMultiplierInside, terrainNoiseParameters.NoiseSizeX, terrainNoiseParameters.NoiseSizeZ, terrainNoiseParameters.NoiseMultiplierOutside);
            Vector4 noiseParamsSecond = new Vector4(terrainNoiseParameters.NoiseMultiplierPower, 0, 0, 0);
            materialPaintHeight.SetVector(NoiseParams, noiseParams);
            materialPaintHeight.SetVector(NoiseParamsSecond, noiseParamsSecond);

            TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushTransform, materialPaintHeight);

            Graphics.Blit(paintContext.sourceRenderTexture, paintContext.destinationRenderTexture, materialPaintHeight, (int) TerrainBuiltinPaintMaterialPasses.PaintTexture);
        }

        #endregion

        #region Clear Foliage

        public void TerrainClearTrees(bool details = true)
        {
            Terrain[] terrains = Terrain.activeTerrains;

            Physics.autoSyncTransforms = false;


            foreach (Terrain terrain in terrains)
            {
                TerrainData terrainData = terrain.terrainData;

                Transform transformTerrain = terrain.transform;
                float polygonHeight = NmSpline.Transform.position.y;
                //var posY = terrain.transform.position.y;
                float sizeX = terrainData.size.x;
                //var sizeY = terrainData.size.y;
                float sizeZ = terrainData.size.z;


#if UNITY_EDITOR
                Undo.RegisterCompleteObjectUndo(terrainData, "Paint");
                Undo.RegisterCompleteObjectUndo(terrain, "Terrain draw texture");
#endif
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minZ = float.MaxValue;
                float maxZ = float.MinValue;

                for (int i = 0; i < NmSpline.Points.Count; i++)
                {
                    Vector3 point = NmSpline.Transform.TransformPoint(NmSpline.Points[i].Position);


                    if (minX > point.x)
                        minX = point.x;

                    if (maxX < point.x)
                        maxX = point.x;

                    if (minZ > point.z)
                        minZ = point.z;

                    if (maxZ < point.z)
                        maxZ = point.z;
                }


                //Debug.DrawLine(new Vector3(minX, 0, minZ), new Vector3(minX, 0, minZ) + Vector3.up * 100, Color.green, 3);
                // Debug.DrawLine(new Vector3(maxX, 0, maxZ), new Vector3(maxX, 0, maxZ) + Vector3.up * 100, Color.blue, 3);


                float terrainToWidth = 1 / sizeZ * (terrainData.detailWidth - 1);
                float terrainToHeight = 1 / sizeX * (terrainData.detailHeight - 1);
                Vector3 position1 = terrain.transform.position;
                minX -= position1.x + PainterData.DistanceClearFoliage;
                maxX -= position1.x - PainterData.DistanceClearFoliage;

                minZ -= position1.z + PainterData.DistanceClearFoliage;
                maxZ -= position1.z - PainterData.DistanceClearFoliage;


                minX *= terrainToHeight;
                maxX *= terrainToHeight;

                minZ *= terrainToWidth;
                maxZ *= terrainToWidth;

                minX = (int) Mathf.Clamp(minX, 0, terrainData.detailWidth);
                maxX = (int) Mathf.Clamp(maxX, 0, terrainData.detailWidth);
                minZ = (int) Mathf.Clamp(minZ, 0, terrainData.detailHeight);
                maxZ = (int) Mathf.Clamp(maxZ, 0, terrainData.detailHeight);

                int[,] detailLayer = terrainData.GetDetailLayer((int) minX, (int) minZ, (int) (maxX - minX), (int) (maxZ - minZ), 0);

                Vector4[,] distances = new Vector4[detailLayer.GetLength(0), detailLayer.GetLength(1)];

                var meshCollider = NmSpline.Transform.gameObject.AddComponent<MeshCollider>();


                Vector3 position = Vector3.zero;
                position.y = polygonHeight;

                for (int x = 0; x < detailLayer.GetLength(0); x++)
                for (int z = 0; z < detailLayer.GetLength(1); z++)
                {
                    Vector3 position2 = transformTerrain.position;
                    position.x = (z + minX) / terrainToHeight + position2.x; //, polygonHeight
                    position.z = (x + minZ) / terrainToWidth + position2.z;

                    var ray = new Ray(position + Vector3.up * 1000, Vector3.down);

                    if (meshCollider.Raycast(ray, out RaycastHit hit, 10000))
                    {
                        // Debug.DrawLine(hit.point, hit.point + Vector3.up * 30, Color.green, 3);

                        float minDist = float.MaxValue;
                        for (int i = 0; i < NmSpline.Points.Count; i++)
                        {
                            int idOne = i;
                            int idTwo = (i + 1) % NmSpline.Points.Count;

                            float dist = RamMath.DistancePointLine(hit.point, NmSpline.Transform.TransformPoint(NmSpline.Points[idOne].Position), NmSpline.Transform.TransformPoint(NmSpline.Points[idTwo].Position));
                            if (minDist > dist)
                                minDist = dist;
                        }

                        float angle = 0;


                        distances[x, z] = new Vector4(hit.point.x, minDist, hit.point.z, angle);
                    }
                    else
                    {
                        float minDist = float.MaxValue;
                        for (int i = 0; i < NmSpline.Points.Count; i++)
                        {
                            int idOne = i;
                            int idTwo = (i + 1) % NmSpline.Points.Count;

                            float dist = RamMath.DistancePointLine(position, NmSpline.Transform.TransformPoint(NmSpline.Points[idOne].Position), NmSpline.Transform.TransformPoint(NmSpline.Points[idTwo].Position));
                            if (minDist > dist)
                                minDist = dist;
                        }

                        float angle = 0;

                        distances[x, z] = new Vector4(position.x, -minDist, position.z, angle);
                    }
                }

                if (!details)
                {
                    List<TreeInstance> newTrees = new List<TreeInstance>();
                    TreeInstance[] oldTrees = terrainData.treeInstances;

                    position.y = polygonHeight;
                    foreach (TreeInstance tree in oldTrees)
                    {
                        //Debug.DrawRay(new Vector3(, 0, tree.position.z * sizeZ) + terrain.transform.position, Vector3.up * 5, Color.red, 3);

                        Vector3 position2 = transformTerrain.position;
                        position.x = tree.position.x * sizeX + position2.x; //, polygonHeight
                        position.z = tree.position.z * sizeZ + position2.z;

                        var ray = new Ray(position + Vector3.up * 1000, Vector3.down);

                        if (!meshCollider.Raycast(ray, out RaycastHit _, 10000))
                        {
                            float minDist = float.MaxValue;
                            for (int i = 0; i < NmSpline.Points.Count; i++)
                            {
                                int idOne = i;
                                int idTwo = (i + 1) % NmSpline.Points.Count;

                                float dist = RamMath.DistancePointLine(position, NmSpline.Transform.TransformPoint(NmSpline.Points[idOne].Position),
                                    NmSpline.Transform.TransformPoint(NmSpline.Points[idTwo].Position));
                                if (minDist > dist)
                                    minDist = dist;
                            }

                            if (minDist > PainterData.DistanceClearFoliageTrees) newTrees.Add(tree);
                        }
                    }

                    terrainData.treeInstances = newTrees.ToArray();
                    Object.DestroyImmediate(meshCollider);
                }

                ClearData = new CarveData(distances, minX, maxX, minZ, maxZ);


                // terrainData.treeInstances = newTrees.ToArray();
                if (details)
                    for (int l = 0; l < terrainData.detailPrototypes.Length; l++)
                    {
                        detailLayer = terrainData.GetDetailLayer((int) ClearData.MinX,
                            (int) ClearData.MinZ, (int) (ClearData.MaxX - ClearData.MinX),
                            (int) (ClearData.MaxZ - ClearData.MinZ), l);

                        for (int x = 0; x < detailLayer.GetLength(0); x++)
                        {
                            for (int z = 0; z < detailLayer.GetLength(1); z++)
                            {
                                Vector4 distance = ClearData.Distances[x, z];

                                if (-distance.y <= PainterData.DistanceClearFoliage || distance.y > 0)
                                {
                                    // float oldValue = detailLayer[x, z];
                                    detailLayer[x, z] = 0;
                                }
                            }
                        }

                        terrainData.SetDetailLayer((int) ClearData.MinX, (int) ClearData.MinZ, l,
                            detailLayer);
                    }


                terrain.Flush();


                Object.DestroyImmediate(meshCollider);
            }

            Physics.autoSyncTransforms = true;
        }

        #endregion
    }
}