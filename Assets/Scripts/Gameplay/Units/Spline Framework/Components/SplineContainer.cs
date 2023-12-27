using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GC.SplineFramework
{
    [ChunkSerializable]
    public struct SplineContainer : IComponentData
    {
        public NativeArray<Spline> Splines;

        public bool IsSetUp;

        public Spline GetSplineByIndex(int splineIndex)
        {
            if (splineIndex >= Splines.Length || splineIndex < 0)
            {
                Debug.LogError("Spline Container: Spline Index was out of bounds!");

                return new Spline();
            }

            return Splines[splineIndex];
        }
    }
}
