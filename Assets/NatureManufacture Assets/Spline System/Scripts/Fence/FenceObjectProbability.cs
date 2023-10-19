// /**
//  * Created by Pawel Homenko on  08/2022
//  */

using UnityEngine;

namespace NatureManufacture.RAM
{
    [System.Serializable]
    public class FenceObjectProbability
    {
        public GameObject gameObject;
        public float probability = 1;

        public FenceGenerator.AlignAxis forward = FenceGenerator.AlignAxis.XAxis;
        public FenceGenerator.AlignAxis up = FenceGenerator.AlignAxis.YAxis;

        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        public Vector3 scaleOffset = Vector3.one;

        //contructor no paramaters with default values
        public FenceObjectProbability()
        {
            Reset();
        }

        //copy constructor
        public FenceObjectProbability(FenceObjectProbability other)
        {
            gameObject = other.gameObject;
            probability = other.probability;
            forward = other.forward;
            up = other.up;
            positionOffset = other.positionOffset;
            rotationOffset = other.rotationOffset;
            scaleOffset = other.scaleOffset;
        }

        public void Reset()
        {
            gameObject = null;
            probability = 1;
            forward = FenceGenerator.AlignAxis.XAxis;
            up = FenceGenerator.AlignAxis.YAxis;
            positionOffset = Vector3.zero;
            rotationOffset = Vector3.zero;
            scaleOffset = Vector3.one;
        }
    }
}