using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace NatureManufacture.RAM
{
    public class LodManager : MonoBehaviour
    {
        [SerializeField] private MeshFilter sourceMeshFilter;

        [SerializeField] private float refreshTime = 0.1f;
        [SerializeField] private Vector3 lodDistance = new Vector3(100, 80, 50);
        [SerializeField] private Transform cameraLod;
        [SerializeField] private float trianglesRefreshRate = 20f;

        [SerializeField] private int levelStart = 3;

        private float _lastRefresh = 0.1f;
        private bool _finished = true;


        private Matrix4x4 _localToWorldMatrix;

        private Mesh _targetMesh;
        private Mesh _baseMesh;


        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector3> _normals = new List<Vector3>();
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<Vector2> _uv = new List<Vector2>();
        private readonly List<Vector2> _uv3 = new List<Vector2>();
        private readonly List<Vector2> _uv4 = new List<Vector2>();
        private readonly List<int> _indices = new List<int>();
        private int[] _indicesBase;

        private int _verticesCount;
        private int _indicesCount;

        private readonly Dictionary<Vector2Int, int> _newVertices = new Dictionary<Vector2Int, int>();
        private Vector2Int _test;

        public MeshFilter SourceMeshFilter
        {
            get => sourceMeshFilter;
            set => sourceMeshFilter = value;
        }

        public Vector3 LODDistance
        {
            get => lodDistance;
            set => lodDistance = value;
        }

        public float RefreshTime
        {
            get => refreshTime;
            set => refreshTime = value;
        }


        private void Start()
        {
            if (!cameraLod && Camera.main != null)
                cameraLod = Camera.main.transform;

            _baseMesh = SourceMeshFilter.sharedMesh;
            SourceMeshFilter.GetComponent<MeshRenderer>().enabled = false;

            _targetMesh = DuplicateMesh(_baseMesh);
            _targetMesh.indexFormat = IndexFormat.UInt32;
            _targetMesh.MarkDynamic();
            GetComponent<MeshFilter>().sharedMesh = _targetMesh;
        }

        private void Update()
        {
            if (_finished && _lastRefresh + RefreshTime < Time.time)
            {
                _finished = false;
                _lastRefresh = Time.time;
                // doSubdivide = false;

                StartCoroutine(Subdivide());
            }
        }


        #region Subdivide

        private IEnumerator Subdivide()
        {
            Profiler.BeginSample("InitArrays");
            InitArrays();
            Profiler.EndSample();


            _localToWorldMatrix = transform.localToWorldMatrix;
            yield return StartCoroutine(Subdivide4Recursive());

            Profiler.BeginSample("FinishMesh");
            FinishMesh();
            Profiler.EndSample();
        }

        #endregion Subdivide

        void InitArrays()
        {
            if (_baseMesh == null)
                _baseMesh = SourceMeshFilter.sharedMesh;

            if (_baseMesh == null)
            {
                Debug.Log(_baseMesh.vertices.Length);
                Debug.LogError("No base mesh filter with mesh");
                return;
            }

            _verticesCount = _vertices.Count;
            _indicesCount = 0;

            /*  _indices.AddRange(_baseMesh.triangles);
              _vertices.AddRange(_baseMesh.vertices);
              _normals.AddRange(_baseMesh.normals);
              _colors.AddRange(_baseMesh.colors);
              _uv.AddRange(_baseMesh.uv);
              _uv4.AddRange(_baseMesh.uv4);*/
        }

        void FinishMesh()
        {
            Profiler.BeginSample("Clear Mesh");
            _targetMesh.Clear();
            Profiler.EndSample();

            Profiler.BeginSample("Set Vertices");
            _targetMesh.SetVertices(_vertices, 0, _verticesCount, MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontValidateIndices);
            if (_normals.Count > 0)
                _targetMesh.SetNormals(_normals, 0, _verticesCount, MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontValidateIndices);
            if (_colors.Count > 0)
                _targetMesh.SetColors(_colors, 0, _verticesCount, MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontValidateIndices);
            if (_uv.Count > 0)
                _targetMesh.SetUVs(0, _uv, 0, _verticesCount, MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontValidateIndices);
            if (_uv3.Count > 0)
                _targetMesh.SetUVs(2, _uv3, 0, _verticesCount, MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontValidateIndices);
            if (_uv4.Count > 0)
                _targetMesh.SetUVs(3, _uv4, 0, _verticesCount, MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontValidateIndices);
            Profiler.EndSample();

            Profiler.BeginSample("Set Triangles");
            _targetMesh.SetTriangles(_indices, 0, _indicesCount, 0, false);
            Profiler.EndSample();

            Profiler.BeginSample("Recalculate Bounds");
            _targetMesh.RecalculateBounds();
            Profiler.EndSample();

            Profiler.BeginSample("Recalculate Tangents");
            _targetMesh.RecalculateTangents();
            Profiler.EndSample();

            /*_newVertices.Clear();
            _vertices.Clear();
            _normals.Clear();
            _colors.Clear();
            _uv.Clear();
            _uv4.Clear();
            _indices.Clear();*/
            _finished = true;
        }

        #region Subdivide4 (2x2)

        private IEnumerator Subdivide4Recursive()
        {
            Vector3 camPosition = cameraLod.position;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                camPosition = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
#endif

            //Debug.Log(triangles.Length / 3);
            for (int i = 0; i < _indicesBase.Length; i += 3)
            {
                if (i % trianglesRefreshRate == 0)
                {
                    //Debug.Log("yield");
                    yield return null;
                }

                int i1 = _indicesBase[i + 0];
                int i2 = _indicesBase[i + 1];
                int i3 = _indicesBase[i + 2];


                SubdivideTriangle(camPosition, levelStart, i1, i2, i3);
            }
        }

        private void SubdivideTriangle(Vector3 camPosition, int level, int i1, int i2, int i3)
        {
            int currentLevel = levelStart - level;
            if (currentLevel > 2)
                currentLevel = 0;


            float distanceLevel = LODDistance[currentLevel];


            bool i1In = Vector3.Distance(_localToWorldMatrix.MultiplyPoint3x4(_vertices[i1]), camPosition) > distanceLevel;
            bool i2In = Vector3.Distance(_localToWorldMatrix.MultiplyPoint3x4(_vertices[i2]), camPosition) > distanceLevel;
            bool i3In = Vector3.Distance(_localToWorldMatrix.MultiplyPoint3x4(_vertices[i3]), camPosition) > distanceLevel;


            if (!i1In && !i2In && i3In && level > 0)
            {
                int i12 = GetNewVertex4(i1, i2);

                AddOrChangeIndice(i1);
                AddOrChangeIndice(i12);
                AddOrChangeIndice(i3);

                AddOrChangeIndice(i12);
                AddOrChangeIndice(i2);
                AddOrChangeIndice(i3);

                return;
            }


            if (i1In && !i2In && !i3In && level > 0)
            {
                int i23 = GetNewVertex4(i2, i3);

                AddOrChangeIndice(i1);
                AddOrChangeIndice(i23);
                AddOrChangeIndice(i3);

                AddOrChangeIndice(i1);
                AddOrChangeIndice(i2);
                AddOrChangeIndice(i23);
                return;
            }


            if (!i1In && i2In && !i3In && level > 0)
            {
                int i13 = GetNewVertex4(i1, i3);

                AddOrChangeIndice(i13);
                AddOrChangeIndice(i2);
                AddOrChangeIndice(i3);

                AddOrChangeIndice(i1);
                AddOrChangeIndice(i2);
                AddOrChangeIndice(i13);
                return;
            }


            if (i1In || i2In || level == 0)
            {
                AddOrChangeIndice(i1);
                AddOrChangeIndice(i2);
                AddOrChangeIndice(i3);

                return;
            }

            if (level <= 0) return;

            int a = GetNewVertex4(i1, i2);
            int b = GetNewVertex4(i2, i3);
            int c = GetNewVertex4(i3, i1);


            SubdivideTriangle(camPosition, level - 1, i1, a, c);
            SubdivideTriangle(camPosition, level - 1, i2, b, a);
            SubdivideTriangle(camPosition, level - 1, i3, c, b);
            SubdivideTriangle(camPosition, level - 1, a, b, c);
        }

        private void AddOrChangeIndice(int indice)
        {
            if (_indices.Count > _indicesCount)
            {
                _indices[_indicesCount] = indice;
            }
            else
                _indices.Add(indice);

            _indicesCount++;
        }


        //dictionary 6.67ms 2.05ms
        //List list
        private int GetNewVertex4(int i1, int i2)
        {
            Profiler.BeginSample("Get New Vertex");
            // uint t1 = 0;
            // if (i1 < i2)
            //     t1 = ((uint) i1 << 16) | (uint) i2;
            // else
            //     t1 = ((uint) i2 << 16) | (uint) i1;

            Profiler.BeginSample("Get Dictionary");
            if (i1 < i2)
            {
                _test.x = i1;
                _test.y = i2;
            }
            else
            {
                _test.x = i2;
                _test.y = i1;
            }

            if (_newVertices.ContainsKey(_test))
            {
                Profiler.EndSample();
                Profiler.EndSample();
                return _newVertices[_test];
            }


            int newIndex = _vertices.Count;
            _newVertices.Add(_test, newIndex);
            Profiler.EndSample();
            Profiler.BeginSample("Defining new vertice");


            if (_vertices.Count > _verticesCount)
            {
                Debug.Log(_verticesCount);
                _vertices[_verticesCount] = (_vertices[i1] + _vertices[i2]) * 0.5f;
                _uv[_verticesCount] = (_uv[i1] + _uv[i2]) * 0.5f;
                //_normals[_verticesCount] = Vector3.up;

                if (_colors.Count > 0)
                    _colors[_verticesCount] = Color.Lerp(_colors[i1], _colors[i2], 0.5f);

                if (_uv3.Count > 0)
                    _uv3[_verticesCount] = (_uv3[i1] + _uv3[i2]) * 0.5f;
                if (_uv4.Count > 0)
                    _uv4[_verticesCount] = (_uv4[i1] + _uv4[i2]) * 0.5f;
            }
            else
            {
                _vertices.Add((_vertices[i1] + _vertices[i2]) * 0.5f);
                _normals.Add(Vector3.up);
                _uv.Add((_uv[i1] + _uv[i2]) * 0.5f);

                if (_colors.Count > 0)
                    _colors.Add(Color.Lerp(_colors[i1], _colors[i2], 0.5f));
                if (_uv3.Count > 0)
                    _uv3.Add((_uv3[i1] + _uv3[i2]) * 0.5f);
                if (_uv4.Count > 0)
                    _uv4.Add((_uv4[i1] + _uv4[i2]) * 0.5f);
            }

            _verticesCount++;


            Profiler.EndSample();
            Profiler.EndSample();

            return newIndex;
        }

        #endregion Subdivide4 (2x2)


        private Mesh DuplicateMesh(Mesh mesh)
        {
            _indicesBase = mesh.triangles;
            _vertices.AddRange(mesh.vertices);
            _normals.AddRange(mesh.normals);
            _colors.AddRange(mesh.colors);
            _uv.AddRange(mesh.uv);
            _uv3.AddRange(mesh.uv3);
            _uv4.AddRange(mesh.uv4);

            return Instantiate(mesh);
        }
    }
}