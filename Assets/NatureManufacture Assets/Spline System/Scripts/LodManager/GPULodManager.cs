using System.Collections;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace NatureManufacture.RAM
{
    public class GPULodManager : MonoBehaviour
    {
        [SerializeField] private MeshFilter sourceMeshFilter;

        [SerializeField] private float refreshTime = 0.1f;
        [SerializeField] private Vector3 lodDistance = new Vector3(100, 80, 50);
        [SerializeField] private Transform cameraLod;

        [SerializeField] private ComputeShader computeShader;


        private Mesh _targetMesh;

        [StructLayout(LayoutKind.Sequential)]
        private struct SourceVertex
        {
            public Vector3 position;
            public Vector3 normal;
            public Color color;
            public Vector2 uv;
            public Vector2 uv3;
            public Vector2 uv4;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DrawVertex
        {
            private readonly float3 position;
            private readonly float3 normal;
            private readonly Color color;
            private readonly float2 uv;
            private readonly float2 uv3;
            private readonly float2 uv4;
        };


        private bool _initialized;
        private ComputeBuffer _sourceVertBuffer;
        private ComputeBuffer _sourceTriBuffer;
        private ComputeBuffer _drawBuffer;
        private ComputeBuffer _countBuffer;
        private int _idPyramidKernel;
        private int _idTriToVertKernel;
        private int _dispatchSize;
        private Bounds _localBounds;
        private int _numTriangles;
        private bool _read = true;
        private int _verticesCount;
        private NativeArray<VertexAttributeDescriptor> _vertexLayout;
        private NativeArray<int> _indexArray;
        private Coroutine _coroutineMesh;

        private const int SourceVertStride = sizeof(float) * (3 + 3 + 4 + 2 + 2 + 2);
        private const int SourceTriStride = sizeof(int);
        private const int DrawStride = sizeof(float) * (3 + 3 + 4 + 2 + 2 + 2);
        private const int CountStride = sizeof(int) * 1;

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

        // Start is called before the first frame update
        private void Start()
        {
            if (!cameraLod && Camera.main != null)
                cameraLod = Camera.main.transform;


            if (_initialized)
            {
                OnDisable();
            }

            _initialized = true;

            _targetMesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh = _targetMesh;

            // specify vertex count and layout
            _vertexLayout = new NativeArray<VertexAttributeDescriptor>(
                6, Allocator.Persistent, NativeArrayOptions.UninitializedMemory
            );

            _vertexLayout[0] = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3);
            _vertexLayout[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3);
            _vertexLayout[2] = new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4);
            _vertexLayout[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2);
            _vertexLayout[4] = new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.Float32, 2);
            _vertexLayout[5] = new VertexAttributeDescriptor(VertexAttribute.TexCoord3, VertexAttributeFormat.Float32, 2);

            Mesh sourceMesh = SourceMeshFilter.sharedMesh;
            SourceMeshFilter.GetComponent<MeshRenderer>().enabled = false;

            Vector3[] positions = sourceMesh.vertices;
            Vector3[] normals = sourceMesh.normals;
            Color[] colors = sourceMesh.colors;
            Vector2[] uvs = sourceMesh.uv;
            Vector2[] uvs3 = sourceMesh.uv3;
            Vector2[] uvs4 = sourceMesh.uv4;
            int[] tris = sourceMesh.triangles;

            SourceVertex[] vertices = new SourceVertex[positions.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new SourceVertex
                {
                    position = positions[i],
                    normal = normals.Length > 0 ? normals[i] : Vector3.up,
                    color = colors.Length > 0 ? colors[i] : Color.black,
                    uv = uvs[i],
                    uv3 = uvs3.Length > 0 ? uvs3[i] : uvs[i],
                    uv4 = uvs4.Length > 0 ? uvs4[i] : uvs[i]
                };
            }

            _numTriangles = tris.Length / 3;
            // int numVertices = _numTriangles * 27 * 3 * 3 * 4;
            int numVertices = tris.Length * 4 * 4 * 4;

            _indexArray = new NativeArray<int>(numVertices, Allocator.Persistent);
            for (int i = 0; i < numVertices; i++)
                _indexArray[i] = i;

            _sourceVertBuffer = new ComputeBuffer(vertices.Length, SourceVertStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            _sourceVertBuffer.SetData(vertices);
            _sourceTriBuffer = new ComputeBuffer(tris.Length, SourceTriStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            _sourceTriBuffer.SetData(tris);

            //Debug.Log($"drawBuffer {numTriangles * 27 * 3}");
            _drawBuffer = new ComputeBuffer(numVertices, DrawStride, ComputeBufferType.Append);
            _drawBuffer.SetCounterValue(0);
            _countBuffer = new ComputeBuffer(1, CountStride, ComputeBufferType.IndirectArguments);
            //_countBuffer.SetData(new int[] {0, 1, 0, 0});

            computeShader = Instantiate(computeShader);
            _idPyramidKernel = computeShader.FindKernel("Main");
            //_idTriToVertKernel = computeShader.FindKernel("Main");

            computeShader.SetBuffer(_idPyramidKernel, "_SourceVertices", _sourceVertBuffer);
            computeShader.SetBuffer(_idPyramidKernel, "_SourceTriangles", _sourceTriBuffer);
            computeShader.SetBuffer(_idPyramidKernel, "_DrawTriangles", _drawBuffer);
            computeShader.SetInt("_NumSourceTriangles", _numTriangles);

            //triToVertComputeShader.SetBuffer(_idTriToVertKernel, "_IndirectArgsBuffer", _countBuffer);

            //material.SetBuffer("_DrawTriangles", drawBuffer);

            computeShader.GetKernelThreadGroupSizes(_idPyramidKernel, out uint threadGroupSize, out _, out _);
            _dispatchSize = Mathf.CeilToInt((float) _numTriangles / threadGroupSize);

            _localBounds = sourceMesh.bounds;
            //localBounds.Expand(pyramidHeight);
        }


/*        private Bounds TransformBounds(Bounds boundsOS)
        {
            var center = transform.TransformPoint(boundsOS.center);

            // transform the local extents' axes
            var extents = boundsOS.extents;
            var axisX = transform.TransformVector(extents.x, 0, 0);
            var axisY = transform.TransformVector(0, extents.y, 0);
            var axisZ = transform.TransformVector(0, 0, extents.z);

            // sum their absolute value to get the world extents
            extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
            extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
            extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

            return new Bounds {center = center, extents = extents};
        }
*/

        private void LateUpdate()
        {
            //Debug.LogError("error");
            //Bounds bounds = TransformBounds(localBounds);
            if (_read)
            {
                _drawBuffer.SetCounterValue(0);
                computeShader.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
                computeShader.SetVector("_LODDistance", LODDistance);
                computeShader.SetVector("_CameraPosition", transform.InverseTransformPoint(cameraLod.position));

                computeShader.Dispatch(_idPyramidKernel, _dispatchSize, 1, 1);
                ComputeBuffer.CopyCount(_drawBuffer, _countBuffer, 0);


                _read = false;
                _coroutineMesh = StartCoroutine(UpdateMesh());
            }


            //triToVertComputeShader.Dispatch(idTriToVertKernel, 1, 1, 1);

            //Graphics.DrawProceduralIndirect(material, bounds, MeshTopology.Triangles, argsBuffer, 0,
            //    null, null, ShadowCastingMode.Off, true, gameObject.layer);
        }

        private IEnumerator UpdateMesh()
        {
            AsyncGPUReadbackRequest countBufferRequest = AsyncGPUReadback.Request(_countBuffer);

            yield return new WaitUntil(() => countBufferRequest.done);

            if (countBufferRequest.hasError) yield break;

            _verticesCount = countBufferRequest.GetData<int>()[0] * 3;

            AsyncGPUReadbackRequest drawBufferRequest = AsyncGPUReadback.Request(_drawBuffer);

            yield return new WaitUntil(() => drawBufferRequest.done);

            if (drawBufferRequest.hasError) yield break;

            var data = drawBufferRequest.GetData<DrawVertex>();

            //Debug.Log(_verticesCount+" "+_drawBuffer.count);

            GenerateMeshNative(data, _verticesCount);

            yield return new WaitForSeconds(RefreshTime);
            _read = true;
        }

        private void GenerateMeshNative(NativeArray<DrawVertex> data, int verticesCount)
        {
            var vertexCount = verticesCount;
            _targetMesh.SetIndexBufferParams(vertexCount, IndexFormat.UInt32);

            _targetMesh.SetVertexBufferParams(vertexCount, _vertexLayout);

            _targetMesh.SetVertexBufferData(data, 0, 0, vertexCount);


            //Debug.Log(ib.Length);

            _targetMesh.SetIndexBufferData(_indexArray, 0, 0, vertexCount);


            //Bounds bounds = TransformBounds(localBounds);
            _targetMesh.subMeshCount = 1;
            _targetMesh.SetSubMesh(0, new SubMeshDescriptor(0, vertexCount)
            {
                bounds = _localBounds,
                vertexCount = vertexCount
            }, MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontRecalculateBounds);

            _targetMesh.bounds = _localBounds;

            //targetMesh.RecalculateBounds();
            //Debug.Log(localBounds.min);
            //Debug.Log(targetMesh.bounds.min);
        }


        private void OnDisable()
        {
            if (_initialized)
            {
                _read = true;
                StopCoroutine(_coroutineMesh);
                _sourceVertBuffer.Release();
                _sourceTriBuffer.Release();
                _drawBuffer.Release();
                _countBuffer.Release();
                _vertexLayout.Dispose();
                _indexArray.Dispose();
            }

            _initialized = false;
        }
    }
}