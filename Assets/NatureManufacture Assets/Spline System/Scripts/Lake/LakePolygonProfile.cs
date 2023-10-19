namespace NatureManufacture.RAM
{
    using UnityEngine;
    using UnityEngine.Rendering;

    [CreateAssetMenu(fileName = "LakePolygonProfile", menuName = "NatureManufacture/LakePolygonProfile", order = 1)]
    public class LakePolygonProfile : ScriptableObject, IProfile<LakePolygonProfile>
    {
        #region basic

        public GameObject gameObject;
        public Material lakeMaterial;
        public float uvScale = 1;
        public AnimationCurve depthCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(5, 1), new Keyframe(10, 0.5f));
        public float maximumTriangleAmount = 200;
        public float maximumTriangleSize = 50;
        public float triangleDensity = 8;
        public bool receiveShadows;
        public ShadowCastingMode shadowCastingMode = ShadowCastingMode.Off;
        public LayerMask snapMask = ~0;

        #endregion

        #region flowmap

        public float automaticFlowMapScale = 0.2f;
        public float automaticFlowMapDistance = 10;
        public float automaticFlowMapDistanceBlend = 10;
        public bool noiseFlowMap;
        public float noiseMultiplierFlowMap = 1f;
        public float noiseSizeXFlowMap = 0.2f;
        public float noiseSizeZFlowMap = 0.2f;

        #endregion

        [SerializeField] private TerrainPainterData terrainPainterData;


        public int biomeType;


        public TerrainPainterData PainterData
        {
            get => terrainPainterData;
            set => terrainPainterData = value;
        }

        public void SetProfileData(LakePolygonProfile otherProfile)
        {
            lakeMaterial = otherProfile.lakeMaterial;

            uvScale = otherProfile.uvScale;
            depthCurve = otherProfile.depthCurve;

            maximumTriangleAmount = otherProfile.maximumTriangleAmount;
            maximumTriangleSize = otherProfile.maximumTriangleSize;
            triangleDensity = otherProfile.triangleDensity;
            snapMask = otherProfile.snapMask;

            receiveShadows = otherProfile.receiveShadows;
            shadowCastingMode = otherProfile.shadowCastingMode;

            automaticFlowMapScale = otherProfile.automaticFlowMapScale;
            automaticFlowMapDistance = otherProfile.automaticFlowMapDistance;
            automaticFlowMapDistanceBlend = otherProfile.automaticFlowMapDistanceBlend;

            noiseFlowMap = otherProfile.noiseFlowMap;
            noiseMultiplierFlowMap = otherProfile.noiseMultiplierFlowMap;
            noiseSizeXFlowMap = otherProfile.noiseSizeXFlowMap;
            noiseSizeZFlowMap = otherProfile.noiseSizeZFlowMap;

            PainterData = otherProfile.PainterData;
        }

        public bool CheckProfileChange(LakePolygonProfile otherProfile)
        {
            if (uvScale != otherProfile.uvScale)
                return true;
            if (maximumTriangleAmount != otherProfile.maximumTriangleAmount)
                return true;
            if (maximumTriangleSize != otherProfile.maximumTriangleSize)
                return true;
            if (triangleDensity != otherProfile.triangleDensity)
                return true;

            if (snapMask != otherProfile.snapMask)
                return true;
            if (otherProfile.receiveShadows != receiveShadows)
                return true;
            if (otherProfile.shadowCastingMode != shadowCastingMode)
                return true;


            if (automaticFlowMapScale != otherProfile.automaticFlowMapScale)
                return true;
            if (automaticFlowMapDistance != otherProfile.automaticFlowMapDistance)
                return true;
            if (automaticFlowMapDistanceBlend != otherProfile.automaticFlowMapDistanceBlend)
                return true;
            if (noiseFlowMap != otherProfile.noiseFlowMap)
                return true;
            if (noiseMultiplierFlowMap != otherProfile.noiseMultiplierFlowMap)
                return true;
            if (noiseSizeXFlowMap != otherProfile.noiseSizeXFlowMap)
                return true;
            if (noiseSizeZFlowMap != otherProfile.noiseSizeZFlowMap)
                return true;
            if (PainterData != otherProfile.PainterData)
                return true;


            return false;
        }
    }
}