using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

namespace NatureManufacture.RAM
{
    [SelectionBase]
    [RequireComponent(typeof(NmSpline))]
    public class FenceGenerator : MonoBehaviour
    {
        [SerializeField] private NmSpline nmSpline;

        public FenceProfile baseProfile;
        public FenceProfile currentProfile;
        public FenceProfile oldProfile;

        public NmSpline NmSpline
        {
            get
            {
                if (nmSpline != null && nmSpline.gameObject == gameObject)
                    return nmSpline;

                nmSpline = GetComponentInParent<NmSpline>();

                if (BaseProfile == null)
                    GenerateBaseProfile();

                nmSpline.SetData(0, 1, false, true, false, true, false, false);

                return nmSpline;
            }
        }

        public int ToolbarInt
        {
            get => toolbarInt;
            set => toolbarInt = value;
        }

        public bool AutoGenerate
        {
            get => autoGenerate;
            set => autoGenerate = value;
        }

        public bool Debug1
        {
            get => debug;
            set => debug = value;
        }

        public NmSpline OtherSpline
        {
            get => otherSpline;
            set => otherSpline = value;
        }

        public float SplineLerp
        {
            get => splineLerp;
            set => splineLerp = value;
        }

        public bool BendMeshesPreview
        {
            get => bendMeshesPreview;
            set => bendMeshesPreview = value;
        }

        public List<GameObject> GeneratedPrefabs
        {
            get => generatedPrefabs;
            set => generatedPrefabs = value;
        }

        public FenceProfile BaseProfile
        {
            get
            {
                if (baseProfile == null)
                    GenerateBaseProfile();
                return baseProfile;
            }
            set => baseProfile = value;
        }

        private static readonly Vector3[] VectorAxes = new Vector3[]
        {
            Vector3.right,
            Vector3.up,
            Vector3.forward,
            Vector3.left,
            Vector3.down,
            Vector3.back
        };


        [SerializeField] private int toolbarInt;


        [SerializeField] private bool autoGenerate;

        [SerializeField] private bool debug;


        //Ramspline
        [SerializeField] private NmSpline otherSpline;
        [SerializeField] private float splineLerp;

        [SerializeField] private List<GameObject> generatedPrefabs = new List<GameObject>();

        private NmSplinePoint[] _points;

        public enum AlignAxis
        {
            /// <summary> Object space X axis. </summary>
            [InspectorName("Object X+")] XAxis,

            /// <summary> Object space Y axis. </summary>
            [InspectorName("Object Y+")] YAxis,

            /// <summary> Object space Z axis. </summary>
            [InspectorName("Object Z+")] ZAxis,

            /// <summary> Object space negative X axis. </summary>
            [InspectorName("Object X-")] NegativeXAxis,

            /// <summary> Object space negative Y axis. </summary>
            [InspectorName("Object Y-")] NegativeYAxis,

            /// <summary> Object space negative Z axis. </summary>
            [InspectorName("Object Z-")] NegativeZAxis
        }

        [SerializeField] private bool bendMeshesPreview;
        //private bool _duplicate;

        public void OnValidate()
        {
            if (BaseProfile == null || BaseProfile.GameObject == gameObject) return;

            FenceProfile tempProfile = BaseProfile;
            BaseProfile = ScriptableObject.CreateInstance<FenceProfile>();
            BaseProfile.SetProfileData(tempProfile);
            BaseProfile.GameObject = gameObject;
            //_duplicate = true;
        }

        #region spline

        public void GenerateSplineAndPointList(bool quick = false)
        {
            if (BaseProfile == null)
            {
                GenerateBaseProfile();
            }

            GeneratePointList();

            if (AutoGenerate)
                GenerateSplineObjects(quick);
        }


        private void GeneratePointList()
        {
            nmSpline.PrepareSpline();


            nmSpline.GenerateFullSpline(BaseProfile.TriangleDensity);


            if (nmSpline.Points.Count > 2)
            {
                NmSplinePoint nmSplinePoint = nmSpline.Points[0];
                nmSplinePoint.Normal = nmSpline.Points[1].Normal;
                nmSplinePoint.Binormal = nmSpline.Points[1].Binormal;
                nmSplinePoint.Tangent = nmSpline.Points[1].Tangent;
                nmSplinePoint.Orientation = nmSpline.Points[1].Orientation;
                nmSplinePoint.Tangent = nmSpline.Points[1].Tangent;


                NmSplinePoint lastNmSplinePoint = nmSpline.Points[^1];
                lastNmSplinePoint.Normal = nmSpline.Points[^2].Normal;
                lastNmSplinePoint.Binormal = nmSpline.Points[^2].Binormal;
                lastNmSplinePoint.Tangent = nmSpline.Points[^2].Tangent;
                lastNmSplinePoint.Orientation = nmSpline.Points[^2].Orientation;
                lastNmSplinePoint.Rotation = nmSpline.Points[^2].Rotation;

                nmSpline.GenerateArrayForDistanceSearch();
            }
        }

        #endregion

        public static FenceGenerator CreateFenceGenerator(List<Vector3> positions = null)
        {
            var gameObject = new GameObject("Fence Generator");


            var fenceGenerator = gameObject.AddComponent<FenceGenerator>();
#if UNITY_EDITOR
            EditorGUIUtility.SetIconForObject(gameObject, EditorGUIUtility.GetIconForObject(fenceGenerator));
#endif

            fenceGenerator.nmSpline = fenceGenerator.GetComponentInParent<NmSpline>();
            fenceGenerator.nmSpline.SetData(0, 1, false, true, false, true, false, false, false);

            if (positions != null)
                for (int i = 0; i < positions.Count; i++)
                {
                    fenceGenerator.nmSpline.AddPoint(positions[i]);
                }

            return fenceGenerator;
        }

        public void GenerateSplineObjects(bool quick = false)
        {
            if (nmSpline.MainControlPoints == null || nmSpline.MainControlPoints.Count < 3)
                return;
            nmSpline.CenterSplinePivot();

            BaseProfile.Posts.RemoveAll(item => item == null);
            BaseProfile.Spans.RemoveAll(item => item == null);


            GeneratePointList();

            if (BaseProfile.Posts.Count == 0 && BaseProfile.Spans.Count == 0)
                return;


            DestroyPrefabs();

            GameObject span = null;

            if (BaseProfile.Spans.Count > 0 && BaseProfile.Spans[0].gameObject != null)
                span = BaseProfile.Spans[0].gameObject;

            if (span == null)
                return;


            NmSplinePoint nextPosition;

            int stop = 0;

            if (baseProfile.RandomSeed)
                baseProfile.Seed = Mathf.Abs((int) System.DateTime.Now.Ticks);

            Random.InitState(baseProfile.Seed);

            float currentLength = 0;

            NmSplinePoint newPosition = nmSpline.NmSplinePointSearcher.FindPosition(currentLength, 0, out int searchLast);
            nextPosition = newPosition;
            float sizePrefab = 0;
            float sizeSpan;

            const int maxLengthChecks = 10000;
            List<FenceObjectProbability> fenceObjectProbabilities = new List<FenceObjectProbability>();

            while (currentLength < nmSpline.Length && stop < maxLengthChecks)
            {
                var probability = BaseProfile.SpanRandomType switch
                {
                    0 => GetRandomFromList(BaseProfile.Spans),
                    1 => BaseProfile.Spans[stop % BaseProfile.Spans.Count],
                    _ => BaseProfile.Spans[0]
                };
                sizePrefab = GetSpanSize(probability, out sizeSpan);

                if (nmSpline.Length < currentLength && Vector3.Distance(nextPosition.Position, newPosition.Position) < sizePrefab * 0.5f)
                    break;
                fenceObjectProbabilities.Add(probability);
                currentLength += sizePrefab;
                stop++;
            }

            float scaleSize = nmSpline.Length / currentLength;
            currentLength = 0;
            int i;
            for (i = 0; i < fenceObjectProbabilities.Count; i++)
            {
                var probability = fenceObjectProbabilities[i];

                sizePrefab = GetSpanSize(probability, out sizeSpan, scaleSize);

                currentLength += sizePrefab;
                nextPosition = nmSpline.NmSplinePointSearcher.FindPosition(currentLength, searchLast, out searchLast);


                if (nmSpline.IsLooping && nmSpline.Length < currentLength)
                    nextPosition = nmSpline.NmSplinePointSearcher.FindPosition(0, 0, out searchLast);


                if (nmSpline.Length < currentLength && Vector3.Distance(nextPosition.Position, newPosition.Position) < sizePrefab * 0.5f)
                    break;


                if (BaseProfile.Posts.Count > 0)
                {
                    var probabilityPost = BaseProfile.PostRandomType switch
                    {
                        0 => GetRandomFromList(BaseProfile.Posts),
                        1 => BaseProfile.Posts[i % BaseProfile.Posts.Count],
                        _ => BaseProfile.Posts[0]
                    };
                    SetPrefabPosition(quick, 1, newPosition, nextPosition, i, currentLength - sizePrefab, probabilityPost, false);
                }

                SetPrefabPosition(quick, sizeSpan, newPosition, nextPosition, i, currentLength - sizePrefab, probability);


                newPosition = nextPosition;
            }

            if (!nmSpline.IsLooping && BaseProfile.Posts.Count > 0)
            {
                var probabilityPost = BaseProfile.PostRandomType switch
                {
                    0 => GetRandomFromList(BaseProfile.Posts),
                    1 => BaseProfile.Posts[i % BaseProfile.Posts.Count],
                    _ => BaseProfile.Posts[0]
                };
                SetPrefabPosition(quick, 1, newPosition, nextPosition, i, currentLength - sizePrefab, probabilityPost, false);
            }
        }


        public void DestroyPrefabs()
        {
            bool allDestroyed;
            do
            {
                allDestroyed = true;
                foreach (GameObject obj in GeneratedPrefabs)
                {
                    if (Application.isEditor)
                        DestroyImmediate(obj);
                    else
                        Destroy(obj);
                }

                foreach (GameObject obj in GeneratedPrefabs)
                {
                    if (obj == null) continue;

                    allDestroyed = false;
                    break;
                }
            } while (!allDestroyed);


            GeneratedPrefabs.Clear();
        }

        private float GetSpanSize(FenceObjectProbability prefabProbability, out float sizeSpan, float scaleSize = 0)
        {
            var meshFilter = prefabProbability.gameObject.GetComponentInChildren<MeshFilter>();
            Mesh spanMesh = meshFilter.sharedMesh;
            var size = spanMesh.bounds.size;

            Vector3 scale = prefabProbability.scaleOffset;
            if (prefabProbability.scaleOffset.x == 0)
                scale.x = 1;
            if (prefabProbability.scaleOffset.y == 0)
                scale.y = 1;
            if (prefabProbability.scaleOffset.z == 0)
                scale.z = 1;

            prefabProbability.scaleOffset = scale;

            size = new Vector3(size.x * scale.x,
                size.y * scale.y,
                size.z * scale.z);

            sizeSpan = prefabProbability.forward switch
            {
                AlignAxis.XAxis or AlignAxis.NegativeXAxis => Mathf.Abs(size.x),
                AlignAxis.YAxis or AlignAxis.NegativeYAxis => Mathf.Abs(size.y),
                _ => Mathf.Abs(size.z)
            };


            float sizePrefab = sizeSpan * BaseProfile.DistanceMultiplier;


            if (sizePrefab + BaseProfile.AdditionalDistance > 0)
            {
                sizePrefab += BaseProfile.AdditionalDistance;
            }


            if (BaseProfile.ScaleMesh && scaleSize > 0)
            {
                sizePrefab *= scaleSize;
            }

            return sizePrefab;
        }

        private void SetPrefabPosition(bool quick, float sizeSpan, NmSplinePoint newPosition, NmSplinePoint nextPosition, int count, float currentLength, FenceObjectProbability probability, bool canBend = true)
        {
            GameObject go;

            if (probability == null || probability.gameObject == null) return;

#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
                go = (GameObject) PrefabUtility.InstantiatePrefab(probability.gameObject);
            else
                go = Instantiate(probability.gameObject);
#else
            go = Instantiate(probability.gameObject);
#endif


            go.transform.position = newPosition.Position;

            var localScale = go.transform.localScale;

            if (canBend && BaseProfile.ScaleMesh)
                localScale = new Vector3(Vector3.Distance(newPosition.Position, nextPosition.Position) / sizeSpan, 1, 1);


            localScale = new Vector3(localScale.x * probability.scaleOffset.x,
                localScale.y * probability.scaleOffset.y,
                localScale.z * probability.scaleOffset.z);
            go.transform.localScale = localScale;

            var rotation = GetRotation(probability);

            var position = go.transform.position;

            var lookPosition = nextPosition.Position - position;
            if (lookPosition.magnitude == 0)
                lookPosition += newPosition.Tangent;

            if (BaseProfile.HoldUp)
                lookPosition.y = 0;


            go.transform.rotation = Quaternion.LookRotation(lookPosition, newPosition.Normal) * newPosition.Rotation * Quaternion.Euler(probability.rotationOffset) * rotation;


            go.name = probability.gameObject.name + "_" + count.ToString();

            if (!canBend || (!BaseProfile.BendMeshes || (quick && !BendMeshesPreview)))
                position += go.transform.up * probability.positionOffset.y + go.transform.forward * probability.positionOffset.x + go.transform.right * probability.positionOffset.z;

            go.transform.position = position;
            GeneratedPrefabs.Add(go);
            go.transform.SetParent(transform, true);

            if (!canBend || !BaseProfile.BendMeshes || (quick && !BendMeshesPreview))
                return;

            BendMeshesWithLod(newPosition, currentLength, go, rotation, probability);
        }


        private void BendMeshesWithLod(NmSplinePoint newPosition, float currentLength, GameObject go, Quaternion rotation, FenceObjectProbability probability)
        {
            MeshCollider[] meshColliders = go.GetComponentsInChildren<MeshCollider>();

            Transform goTransform = go.transform;

            goTransform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up) * Quaternion.Euler(probability.rotationOffset) * rotation;

            goTransform.localScale = probability.scaleOffset;


            MeshFilter[] meshFilter = go.GetComponentsInChildren<MeshFilter>();

            Vector3 newPositionVertice;

            Vector3 position = goTransform.position;
            float xPosition = position.x;
            float newPositionZ = newPosition.Position.z;
            float newPositionY = newPosition.Position.y;

            NmSplinePoint splinePoint = nmSpline.NmSplinePointSearcher.FindPosition(currentLength, 0, out int searchLast);

            searchLast = searchLast > 0 ? searchLast - 1 : 0;
            Vector3 eulerAngles;
            bool holdUp = BaseProfile.HoldUp;
            foreach (var item in meshFilter)
            {
                var mesh = Instantiate(item.sharedMesh);
                Vector3[] vertices = mesh.vertices;
                int verticeCount = vertices.Length;
                Transform meshFilterTransform = item.transform;

                for (int i = 0; i < verticeCount; i++)
                {
                    newPositionVertice = meshFilterTransform.TransformPoint(vertices[i]);


                    float splinePosition = currentLength - (newPositionVertice.x - xPosition);

                    splinePoint = nmSpline.NmSplinePointSearcher.FindPosition(splinePosition, splinePosition >= currentLength ? searchLast : 0, out _);

                    eulerAngles = splinePoint.Rotation.eulerAngles;

                    Quaternion splineRotation = Quaternion.AngleAxis(eulerAngles.z, splinePoint.Tangent) * Quaternion.AngleAxis(eulerAngles.y, splinePoint.Normal) *
                                                Quaternion.AngleAxis(eulerAngles.x, splinePoint.Binormal);


                    //newPositionVertice += splineRotation * splinePoint.Normal * yOffsetSpan;
                    vertices[i] = splinePoint.Position + splineRotation * (splinePoint.Binormal * (newPositionVertice.z - newPositionZ + probability.positionOffset.x)
                                                                           + (holdUp
                                                                               ? new Vector3(0, (newPositionVertice.y - newPositionY + probability.positionOffset.y), 0)
                                                                               : splinePoint.Normal * (newPositionVertice.y - newPositionY + probability.positionOffset.y)));


                    vertices[i] = meshFilterTransform.InverseTransformPoint(vertices[i]);
                }

                mesh.vertices = vertices;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                if (meshColliders.Length > 0)
                {
                    for (int i = 0; i < meshColliders.Length; i++)
                    {
                        if (meshColliders[i].sharedMesh == item.sharedMesh)
                        {
                            meshColliders[i].sharedMesh = mesh;
                        }
                    }
                }

                item.sharedMesh = mesh;
            }
        }


        private Vector3 GetAxis(AlignAxis axis)
        {
            return VectorAxes[(int) axis];
        }


        private Quaternion GetRotation(FenceObjectProbability probability)
        {
            var remappedForward = GetAxis(probability.forward);
            var remappedUp = GetAxis(probability.up);
            var rotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));


            return rotation;
        }


        private FenceObjectProbability GetRandomFromList(List<FenceObjectProbability> objectProbabilities)
        {
            float probabilitySum = 0;
            for (int i = 0; i < objectProbabilities.Count; i++)
            {
                probabilitySum += objectProbabilities[i].probability;
            }

            float random = Random.Range(0, probabilitySum);

            for (int i = 0; i < objectProbabilities.Count; i++)
            {
                random -= objectProbabilities[i].probability;
                if (random < 0)
                    return objectProbabilities[i];
            }

            return objectProbabilities[0];
        }

        public void GenerateBaseProfile()
        {
            BaseProfile = ScriptableObject.CreateInstance<FenceProfile>();
        }
    }
}