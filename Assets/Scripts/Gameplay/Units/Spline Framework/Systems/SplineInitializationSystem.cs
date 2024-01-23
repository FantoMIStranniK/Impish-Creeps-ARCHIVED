using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using GC.Map;

namespace GC.Gameplay.SplineFramework
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(GlobalSpawnerSystem))]
    public partial class SplineInitializationSystem : SystemBase
    {
        protected override void OnCreate()
        {
            GlobalSpawnerSystem.OnMapCreationFinished += InitSplineContainer;
        }

        protected override void OnUpdate() {}

        private void InitSplineContainer()
        {
            RefRW<SplineContainer> splineContainer;

            if (!SystemAPI.TryGetSingletonRW(out splineContainer))
            {
                Debug.LogError("ERROR: Failed to fetch SplineContainer");

                return;
            }

            NativeList<Spline> splines = new NativeList<Spline>(Allocator.Persistent);

            foreach (Spline spline in SystemAPI.Query<Spline>())
            {
                splines.Add(spline);
            }

            splineContainer.ValueRW.Splines = splines.AsArray();

            splineContainer.ValueRW.IsSetUp = true;
        }
    }
}
