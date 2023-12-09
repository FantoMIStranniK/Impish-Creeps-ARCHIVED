using GC.Units.Movement;
using Unity.Entities;
using UnityEngine.SceneManagement;
using Unity.Collections;
using GC.Map;

namespace GC.SplineMovement
{
    [UpdateBefore(typeof(UnitMovementSystem))]
    public partial class SplineInitializationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var mapCheckSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<MapCheckingSystem>();

            if (mapCheckSystem.CurrentMapType != MapType.Gameplay)
                return;

            RefRW<SplineContainer> splineContainer;

            if (!SystemAPI.TryGetSingletonRW(out splineContainer))
                return;

            if (splineContainer.ValueRW.IsSetUp)
                return;

            InitSplineContainer(splineContainer);
        }

        private void InitSplineContainer(RefRW<SplineContainer> splineContainer)
        {
            NativeList<Spline> splines = new NativeList<Spline>(Allocator.Persistent);

            foreach (Spline spline in SystemAPI.Query<Spline>())
            {
                splines.Add(spline);
            }

            splineContainer.ValueRW.Splines = splines.ToArray(Allocator.Persistent);

            splineContainer.ValueRW.IsSetUp = true;
        }
    }
}
