using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System;

namespace GC.Gameplay.SplineFramework
{
    [ChunkSerializable]
    public struct SplineContainer : IComponentData
    {
        public NativeList<Spline> Splines;

        public bool IsSetUp;

        public Spline GetSplineByIndex(int splineIndex)
        {
            if (splineIndex >= Splines.Length || splineIndex < 0)
            {
#if UNITY_EDITOR
                Debug.LogError("Spline Container: Spline Index was out of bounds!");
#endif

                return new Spline();
            }

            return Splines[splineIndex];
        }
    }
}
