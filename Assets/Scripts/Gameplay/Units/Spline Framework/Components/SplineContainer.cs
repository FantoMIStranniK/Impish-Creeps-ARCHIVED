using UnityEngine;
using Unity.Entities;
using Unity.Collections;

namespace GC.Gameplay.SplineFramework
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
