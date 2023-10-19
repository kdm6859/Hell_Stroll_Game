using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Smoothing;
using TriangleNet.Topology;
using UnityEngine;
using UnityEngine.Rendering;
using Vertex = TriangleNet.Geometry.Vertex;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NatureManufacture.RAM
{
#if VEGETATION_STUDIO
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationStudio;
#endif
#if VEGETATION_STUDIO_PRO
using AwesomeTechnologies.VegetationSystem.Biomes;
#endif

    [RequireComponent(typeof(NmSpline))]
    [RequireComponent(typeof(MeshFilter))]
    public class LakePolygon : MonoBehaviour, IVertexPaintable, ITerrainPainterGetData
    {
        public int toolbarInt;

        public LakePolygonProfile baseProfile;
        public LakePolygonProfile currentProfile;
        public LakePolygonProfile oldProfile;

        #region Obsolete

        [Obsolete("This is an obsolete field moved to Main Control Points")]
        public List<Vector3> points = new List<Vector3>();

        #endregion


        //public bool overrideLakeRender;


        public bool generateMeshParts;
        public int meshPartsCount = 3;
        public List<Transform> meshesPartTransforms = new List<Transform>();
        public bool generateLod;
        public bool generateLodGPU = true;
        public Vector3 lodDistance = new Vector3(100, 80, 50);
        public float lodRefreshTime = 0.1f;


        public float height;
        public bool lockHeight = true;


        public float yOffset;


        public int trianglesGenerated;
        public float vertsGenerated;
        public Mesh currentMesh;

        public MeshFilter meshfilter;


        public List<Vector2> depth = new List<Vector2>();


        public float closeDistanceSimulation = 5f;
        public int angleSimulation = 5;
        public float checkDistanceSimulation = 50;
        public bool removeFirstPointSimulation = true;


        public bool normalFromRaycast;
        public bool snapToTerrain;


        public float floatSpeed = 10;

        [SerializeField] private NmSpline nmSpline;

        public NmSpline NmSpline
        {
            get
            {
                if (nmSpline != null && nmSpline.gameObject == gameObject)
                    return nmSpline;


                if (BaseProfile == null)
                    GenerateBaseProfile();

                nmSpline = GetComponentInParent<NmSpline>();
                nmSpline.SetData(snapToTerrain ? 1 : 0, 1, true, false, false, true, false, false, false);
                MoveControlPointsToMainControlPoints();
                GeneratePolygon();

                return nmSpline;
            }
            set => nmSpline = value;
        }

        public LakePolygonSimulationGenerator LakePolygonSimulationGenerator => _lakePolygonSimulationGenerator ??= new LakePolygonSimulationGenerator(this);

        public TerrainManager TerrainManager => terrainManager ??= new TerrainManager(nmSpline, this);

        public TerrainPainterData PainterData
        {
            get => BaseProfile.PainterData;
            set => BaseProfile.PainterData = value;
        }


        public VertexPainterData VertexPainterData => vertexPainterData ??= new VertexPainterData(true);

        [field: SerializeField] public float TriangleSizeByLimit { get; set; }

        public LakePolygonProfile BaseProfile
        {
            get
            {
                if (baseProfile == null)
                    GenerateBaseProfile();
                return baseProfile;
            }
            set => baseProfile = value;
        }


#if VEGETATION_STUDIO_PRO
    public float biomMaskResolution = 0.5f;
    public float vegetationBlendDistance = 1;
    public float vegetationMaskSize = 3;
    public BiomeMaskArea biomeMaskArea;
    public bool refreshMask = false;
#endif
#if VEGETATION_STUDIO
    public float vegetationMaskResolution = 0.5f;
    public float vegetationMaskPerimeter = 5;
    public VegetationMaskArea vegetationMaskArea;

#endif


        public List<GameObject> meshTerrainGOs = new();
        private readonly List<Vector3> _verts = new();
        private LakePolygonSimulationGenerator _lakePolygonSimulationGenerator;
        [SerializeField] private TerrainManager terrainManager;
        [SerializeField] private VertexPainterData vertexPainterData = new(true);
        private bool _duplicate;


        public void OnValidate()
        {
            if (BaseProfile == null || BaseProfile.gameObject == gameObject) return;

            LakePolygonProfile tempProfile = BaseProfile;
            BaseProfile = ScriptableObject.CreateInstance<LakePolygonProfile>();
            BaseProfile.SetProfileData(tempProfile);
            BaseProfile.gameObject = gameObject;
            _duplicate = true;
        }


        private int GetNewVertex(float x, float y, float z)
        {
            for (int i = 0; i < _verts.Count; i++)
            {
                if (Vector3.Distance(_verts[i], new Vector3(x, y, z)) < 0.0001f)
                    return i;
            }

            int id = _verts.Count;
            _verts.Add(new Vector3(x, y, z));
            return id;
        }

        public void GeneratePolygon(bool quick = false)
        {
            meshfilter = GetComponent<MeshFilter>();

            if (meshfilter.sharedMesh != null)
            {
                currentMesh = Instantiate(meshfilter.sharedMesh);
                currentMesh.name = $"mesh {gameObject.name}";
            }


            if (TerrainManager.PainterData != null)
                TerrainManager.PainterData.TerrainsUnder.Clear();

            if (BaseProfile == null)
            {
                GenerateBaseProfile();
            }

            MoveControlPointsToMainControlPoints();

            //Debug.Log("generate polygon");
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
                meshRenderer.receiveShadows = BaseProfile.receiveShadows;
                meshRenderer.shadowCastingMode = BaseProfile.shadowCastingMode;
            }


            if (lockHeight)
                for (int i = 1; i < NmSpline.MainControlPoints.Count; i++)
                {
                    Vector4 vec = NmSpline.MainControlPoints[i].position;
                    vec.y = NmSpline.MainControlPoints[0].position.y;
                    NmSpline.MainControlPoints[i].position = vec;
                }

            if (NmSpline.MainControlPoints.Count < 3)
                return;

            NmSpline.CenterSplinePivot();

            NmSpline.PrepareSpline();
            NmSpline.CalculateCatmullRomSideSplines();

            NmSpline.CalculateSplinePositions(BaseProfile.triangleDensity, NmSpline.SplineSide.Center);


            _verts.Clear();
            List<int> indices = new List<int>();

#if UNITY_EDITOR
            //double time = EditorApplication.timeSinceStartup;
#endif
            TriangulateSpline(quick, indices);

#if UNITY_EDITOR
            //Debug.Log($"Generation time: {EditorApplication.timeSinceStartup - time}");
#endif


            Vector3[] vertices = _verts.ToArray();
            int vertCount = vertices.Length;

            Vector3[] normals = new Vector3[vertCount];
            Vector2[] uvs = new Vector2[vertCount];
            Color[] colors;
            List<Vector2> colorsFlowMap = new();


            if (currentMesh != null && currentMesh.uv4 != null && VertexPainterData.OverridenFlow)
            {
                colorsFlowMap.AddRange(currentMesh.uv4);
            }


            if (currentMesh != null && currentMesh.colors != null && VertexPainterData.OverridenColors)
            {
                if (currentMesh.colors.Length == vertCount)
                    colors = currentMesh.colors;
                else
                {
                    Color[] oldColors = currentMesh.colors;
                    colors = new Color[vertCount];
                    for (int i = 0; i < oldColors.Length && i < colors.Length; i++)
                    {
                        colors[i] = oldColors[i];
                    }
                }
            }
            else
                colors = new Color[vertCount];


            if (!quick)
            {
                Terrain checkTerrain;
                depth.Clear();
                Vector3 position;
                for (int i = 0; i < vertCount; i++)
                {
                    position = transform.position;
                    //ssDebug.DrawLine(vertices[i] + position + Vector3.up * 10, vertices[i] + position - Vector3.up * 100, Color.red, 3);
                    if (Physics.Raycast(vertices[i] + position + Vector3.up * 10000, Vector3.down, out RaycastHit hit, 30000, BaseProfile.snapMask.value, QueryTriggerInteraction.Ignore))
                    {
                        checkTerrain = hit.collider.gameObject.GetComponent<Terrain>();


                        //Debug.Log("add terrain " + checkTerrain + " " + TerrainPainterData.terrainsUnder.Count);
                        if (checkTerrain && TerrainManager.PainterData != null && !TerrainManager.PainterData.TerrainsUnder.Contains(checkTerrain))
                        {
                            //Debug.Log("add terrain");
                            TerrainManager.PainterData.TerrainsUnder.Add(checkTerrain);
                        }

                        if (snapToTerrain)
                            vertices[i] = hit.point - position + new Vector3(0, 0.1f, 0);


                        depth.Add(new Vector2(BaseProfile.depthCurve.Evaluate((position.y + vertices[i].y - hit.point.y + yOffset) / 5), 0));
                    }
                    else
                        depth.Add(new Vector2(0, 0));


                    vertices[i].y += yOffset;


                    if (normalFromRaycast)
                        normals[i] = hit.normal;
                    else
                        normals[i] = Vector3.up;

                    uvs[i] = new Vector2(vertices[i].x, vertices[i].z) * (0.01f * BaseProfile.uvScale);

                    if (!VertexPainterData.OverridenColors)
                    {
                        colors[i] = Color.black;
                    }
                }

                if (VertexPainterData.OverridenFlow)
                {
                    while (colorsFlowMap.Count < vertCount) colorsFlowMap.Add(new Vector2(0, 1));

                    while (colorsFlowMap.Count > vertCount) colorsFlowMap.RemoveAt(colorsFlowMap.Count - 1);
                }
                else
                {
                    List<Vector2> lines = new List<Vector2>();
                    List<Vector2> vert2 = new List<Vector2>();

                    for (int i = 0; i < NmSpline.Points.Count; i++) lines.Add(new Vector2(NmSpline.Points[i].Position.x, NmSpline.Points[i].Position.z));

                    for (int i = 0; i < vertices.Length; i++) vert2.Add(new Vector2(vertices[i].x, vertices[i].z));


                    colorsFlowMap.Clear();

                    Vector2 flow;
                    for (int i = 0; i < vertCount; i++)
                    {
                        float minDist = float.MaxValue;
                        Vector2 minPoint = vert2[i];
                        for (int k = 0; k < NmSpline.Points.Count; k++)
                        {
                            int idOne = k;
                            int idTwo = (k + 1) % lines.Count;

                            float dist = RamMath.DistancePointLine(vert2[i], lines[idOne], lines[idTwo], out Vector2 point);
                            if (minDist > dist)
                            {
                                minDist = dist;
                                minPoint = point;
                            }
                        }

                        flow = minPoint - vert2[i];
                        // flow = -flow.normalized * (automaticFlowMapScale + (noiseflowMap ? Mathf.PerlinNoise(vert2[i].x * noiseSizeXflowMap * 0.1f, vert2[i].y * noiseSizeZflowMap * 0.1f) * noiseMultiplierflowMap - noiseMultiplierflowMap * 0.5f                        : 0));
                        flow = -flow.normalized * (BaseProfile.automaticFlowMapScale *
                                                   (BaseProfile.noiseFlowMap ? Mathf.PerlinNoise(vert2[i].x * BaseProfile.noiseSizeXFlowMap * 0.1f, vert2[i].y * BaseProfile.noiseSizeZFlowMap * 0.1f) * BaseProfile.noiseMultiplierFlowMap : 1));

                        if (minDist > BaseProfile.automaticFlowMapDistance)
                        {
                            flow *= 1 - Mathf.Clamp01((minDist - BaseProfile.automaticFlowMapDistance) / BaseProfile.automaticFlowMapDistanceBlend);
                            //20 -20 /40 = 0   40-20/40 = 0,5
                        }

                        colorsFlowMap.Add(flow);
                    }
                }
            }
            else
            {
                VertexPainterData.OverridenColors = false;
                VertexPainterData.OverridenFlow = false;
                for (int i = 0; i < vertCount; i++)
                {
                    normals[i] = Vector3.up;
                }
            }

            if (currentMesh && !_duplicate)
            {
                _duplicate = false;
                currentMesh.Clear();
            }
            else
                currentMesh = new Mesh();

            vertsGenerated = vertCount;

            if (vertCount > 65000) currentMesh.indexFormat = IndexFormat.UInt32;


            currentMesh.vertices = vertices;
            currentMesh.subMeshCount = 1;
            currentMesh.SetTriangles(indices, 0);

            currentMesh.SetUVs(0, uvs);
            currentMesh.normals = normals;


            if (!quick)
            {
                currentMesh.SetUVs(2, depth);
                currentMesh.colors = colors;
                currentMesh.SetUVs(3, colorsFlowMap);
            }
            else
            {
                currentMesh.SetUVs(2, (Vector2[]) null);
                currentMesh.colors = null;
                currentMesh.SetUVs(3, (Vector2[]) null);
            }

            trianglesGenerated = indices.Count / 3;


            currentMesh.RecalculateTangents();
            currentMesh.RecalculateBounds();

            meshfilter.sharedMesh = currentMesh;


            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider != null) meshCollider.sharedMesh = currentMesh;

            if (!quick)
            {
                if (generateMeshParts)
                    GenerateMeshParts(currentMesh);
                else if (generateLod)
                {
                    meshPartsCount = 1;
                    GenerateMeshParts(currentMesh);
                }
                else
                    foreach (Transform item in meshesPartTransforms)
                        if (item != null)
                            DestroyImmediate(item.gameObject);
            }
            else
                foreach (Transform item in meshesPartTransforms)
                    if (item != null)
                        DestroyImmediate(item.gameObject);
        }

        private void TriangulateSpline(bool quick, List<int> indices)
        {
            List<Vector3> verticesList = new List<Vector3>();
            foreach (var point in nmSpline.Points)
            {
                verticesList.Add(point.Position);
            }


            var polygon = new Polygon();

            List<Vertex> vertexes = new();

            for (int i = 0; i < verticesList.Count; i++)
            {
                var vert = new Vertex(verticesList[i].x, verticesList[i].z)
                {
                    z = verticesList[i].y
                };
                vertexes.Add(vert);
            }

            polygon.Add(new Contour(vertexes));

            TriangleSizeByLimit = (float) (polygon.Bounds().Height * polygon.Bounds().Width / (quick ? Mathf.Min(BaseProfile.maximumTriangleAmount, 200) : BaseProfile.maximumTriangleAmount)) * 1.5f;


            float triangleSize = BaseProfile.maximumTriangleSize < TriangleSizeByLimit ? TriangleSizeByLimit : BaseProfile.maximumTriangleSize;


            var contour = new Contour(vertexes);
            Point testP = contour.FindInteriorPoint();


            polygon.Regions.Add(new RegionPointer(vertexes[0].x, vertexes[0].y, 1));
            polygon.Regions.Add(new RegionPointer(testP.x, testP.y, 2));

            var options = new ConstraintOptions {ConformingDelaunay = true};

            QualityOptions quality = new QualityOptions {MinimumAngle = 30, MaximumArea = triangleSize};


            var mesh = (TriangleNet.Mesh) polygon.Triangulate(options, quality);

            if (!quick)
            {
                mesh.Refine(quality);

                try
                {
                    new SimpleSmoother().Smooth(mesh, 100);
                }
                catch (Exception)
                {
                    Debug.LogError("Wrong lake shape");
                }
            }


            indices.Clear();


            foreach (Triangle triangle in mesh.triangles)
            {
                Vertex vertex = mesh.vertices[triangle.vertices[2].id];
                indices.Add(GetNewVertex((float) vertex.x, (float) vertex.z, (float) vertex.y));

                vertex = mesh.vertices[triangle.vertices[1].id];
                indices.Add(GetNewVertex((float) vertex.x, (float) vertex.z, (float) vertex.y));


                vertex = mesh.vertices[triangle.vertices[0].id];
                indices.Add(GetNewVertex((float) vertex.x, (float) vertex.z, (float) vertex.y));
            }
        }

        private void GenerateMeshParts(Mesh baseMesh)
        {
            foreach (Transform item in meshesPartTransforms)
                if (item != null)
                    DestroyImmediate(item.gameObject);

            int[] triangles = baseMesh.triangles;
            Vector3[] vertices = baseMesh.vertices;
            Vector3[] normals = baseMesh.normals;
            Vector2[] uvs = baseMesh.uv;
            Vector2[] uvs2 = baseMesh.uv3;
            Vector2[] uvs3 = baseMesh.uv4;
            Color[] colorsMesh = baseMesh.colors;


            GetComponent<MeshRenderer>().enabled = false;

            int countTrianglePart = triangles.Length / 3;
            countTrianglePart = Mathf.CeilToInt(countTrianglePart / (float) meshPartsCount) * 3;
            //Debug.Log(countTrianglePart);

            for (var i = 0; i < meshPartsCount; i++)
            {
                var go = new GameObject(gameObject.name + "- Mesh part " + i);
                go.transform.SetParent(transform, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localScale = Vector3.one;

                meshesPartTransforms.Add(go.transform);

                var meshRendererPart = go.AddComponent<MeshRenderer>();

                meshRendererPart.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
                meshRendererPart.receiveShadows = BaseProfile.receiveShadows;
                meshRendererPart.shadowCastingMode = BaseProfile.shadowCastingMode;


                var meshFilter = go.AddComponent<MeshFilter>();
                var meshPart = new Mesh();
                meshPart.Clear();

                List<Vector3> verticesPart = new();
                List<Vector3> normalsPart = new();
                List<Vector2> uvPart = new();
                List<Vector2> uv2Part = new();
                List<Vector2> uv3Part = new();
                List<Color> colorsPart = new();
                List<int> trianglesPart = new();

                int curVertIndex;
                int newVertIndex;
                int curSubVertIndex = 0;
                List<int> vertexIndices = new();


                for (int t = countTrianglePart * i; t < countTrianglePart * (i + 1) && t < triangles.Length; t++)
                {
                    curVertIndex = triangles[t];

                    if (!vertexIndices.Contains(curVertIndex))
                    {
                        newVertIndex = curSubVertIndex;

                        vertexIndices.Add(curVertIndex);
                        verticesPart.Add(vertices[curVertIndex]);
                        colorsPart.Add(colorsMesh[curVertIndex]);
                        normalsPart.Add(normals[curVertIndex]);
                        uvPart.Add(uvs[curVertIndex]);
                        uv2Part.Add(uvs2[curVertIndex]);
                        uv3Part.Add(uvs3[curVertIndex]);

                        curSubVertIndex++;
                    }
                    else
                    {
                        newVertIndex = vertexIndices.IndexOf(curVertIndex);
                    }

                    trianglesPart.Add(newVertIndex);
                }

                //Debug.Log(verticesPart.Count);
                if (verticesPart.Count <= 0) continue;


                meshPart.SetVertices(verticesPart);
                meshPart.SetTriangles(trianglesPart, 0);
                meshPart.SetNormals(normalsPart);
                meshPart.SetUVs(0, uvPart);
                meshPart.SetUVs(2, uv2Part);
                meshPart.SetUVs(3, uv3Part);
                meshPart.colors = colorsPart.ToArray();


                meshPart.RecalculateTangents();
                meshFilter.sharedMesh = meshPart;


                if (generateLod)
                {
                    var goLod = new GameObject(gameObject.name + "- MeshPartLOD_" + (generateLodGPU ? "GPU" : "CPU") + "_" + i);
                    goLod.transform.SetParent(go.transform, false);
                    goLod.transform.localPosition = Vector3.zero;
                    goLod.transform.localEulerAngles = Vector3.zero;
                    goLod.transform.localScale = Vector3.one;
                    if (generateLodGPU)
                    {
                        GPULodManager gpuLodManager = goLod.AddComponent<GPULodManager>();
                        gpuLodManager.SourceMeshFilter = meshFilter;
                        gpuLodManager.LODDistance = lodDistance;
                        gpuLodManager.RefreshTime = lodRefreshTime;
                    }
                    else
                    {
                        LodManager lodManager = goLod.AddComponent<LodManager>();
                        lodManager.SourceMeshFilter = meshFilter;
                        lodManager.LODDistance = lodDistance;
                        lodManager.RefreshTime = lodRefreshTime;
                    }


                    goLod.AddComponent<MeshFilter>();
                    MeshRenderer goLodMeshRenderer = goLod.AddComponent<MeshRenderer>();
                    goLodMeshRenderer.sharedMaterial = meshRendererPart.sharedMaterial;
                    goLodMeshRenderer.receiveShadows = BaseProfile.receiveShadows;
                    goLodMeshRenderer.shadowCastingMode = BaseProfile.shadowCastingMode;
                }
            }
        }


        public static LakePolygon CreatePolygon(Material material, List<Vector3> positions = null)
        {
            var gameObject = new GameObject("Lake Polygon")
            {
                layer = LayerMask.NameToLayer("Water")
            };

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.receiveShadows = false;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
#if UNITY_EDITOR
            meshRenderer.receiveGI = ReceiveGI.Lightmaps;
#endif

            var polygon = gameObject.AddComponent<LakePolygon>();

            polygon.nmSpline = polygon.GetComponentInParent<NmSpline>();
            polygon.nmSpline.SetData(polygon.snapToTerrain ? 1 : 0, 1, true, false, false, true, false, false, false);
#if UNITY_EDITOR
            var flags = StaticEditorFlags.ContributeGI;

            GameObjectUtility.SetStaticEditorFlags(gameObject, flags);
#endif


            if (material != null)
                meshRenderer.sharedMaterial = material;

            if (positions != null)
                for (int i = 0; i < positions.Count; i++)
                    polygon.NmSpline.AddPoint(positions[i], polygon.snapToTerrain);

            return polygon;
        }


        public void GenerateBaseProfile()
        {
            //Debug.Log("GenerateBaseProfile");

            BaseProfile = ScriptableObject.CreateInstance<LakePolygonProfile>();
            MeshRenderer ren = GetComponent<MeshRenderer>();
            BaseProfile.lakeMaterial = ren.sharedMaterial;
        }

        #region Obsolete

        public void MoveControlPointsToMainControlPoints()
        {
#pragma warning disable 612, 618

            if (points.Count > 0 && NmSpline.MainControlPoints.Count == 0)
            {
                Debug.Log("Move Control Points To Main Control Points");
                nmSpline = GetComponentInParent<NmSpline>();
                nmSpline.SetData(snapToTerrain ? 1 : 0, 1, true, false, false, true, false, false, false);

                for (int i = 0; i < points.Count; i++)
                {
                    RamControlPoint ramControlPoint = new RamControlPoint(points[i]);
                    NmSpline.MainControlPoints.Add(ramControlPoint);
                }
            }

            if (NmSpline.MainControlPoints.Count > 0)
            {
                points.Clear();
            }
#pragma warning restore 612, 618
        }

        #endregion

        public VertexPainterData GetVertexPainterData()
        {
            return VertexPainterData;
        }
    }
}