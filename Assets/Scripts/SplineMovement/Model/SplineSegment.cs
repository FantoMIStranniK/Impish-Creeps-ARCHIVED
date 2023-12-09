using System;
using Unity.Mathematics;

namespace GC.SplineMovement
{
    [Serializable]
    public struct SplineSegment
    {
        public float3 StartPoint;
        public float3 FirstInterpolationPoint;
        public float3 SecondInterpolationPoint;
        public float3 EndPoint;

        public SplineSegmentData SplineData;

        public void Init(float startPercent)
        {
            SplineData = new SplineSegmentData();
            SplineData.Init(this, startPercent);
        }

        public float3 GetPointOnSegment(float t)
        {
            float oneMinusT = 1f - t;

            return
                oneMinusT * oneMinusT * oneMinusT * StartPoint +
                3f * oneMinusT * oneMinusT * t * FirstInterpolationPoint +
                3f * oneMinusT * t * t * SecondInterpolationPoint +
                t * t * t * EndPoint;
        }
    }
}
