using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Collections;
using GC.SceneManagement;
using GC.Gameplay.SplineFramework.Model;
using GC.Gameplay.SplineFramework;

namespace GC.Map
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GlobalSpawnerSystem : SystemBase
    {
        public static MapPrefab MapPrefab;

        public static Action OnMapDecoCreation;
        public static Action OnMapCreationFinished;

        private EntityManager _entityManager;

        private bool _finishedCreatingMap;

        protected override void OnCreate()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _finishedCreatingMap = false;

            SceneManager.sceneLoaded += TryCreateMap;
        }

        protected override void OnUpdate() {}

        private void TryCreateMap(Scene scene, LoadSceneMode mode)
        {
            if (!CanStartCreatingMap())
                return;

            CreateLevel();
        }

        private bool CanStartCreatingMap()
        {
            if (!IsSuitableMap(MapType.Gameplay))
                return false;

            if (_finishedCreatingMap)
                return false;

            if (MapPrefab == null)
                return false;

            return true;
        }

        public bool IsSuitableMap(MapType desiredMapType)
        {
            var mapCheckSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<MapCheckingSystem>();

            if (mapCheckSystem.CurrentMapType == desiredMapType)
                return true;

            return false;
        }

        private void CreateLevel()
        {
            OnMapDecoCreation.Invoke();

            CreateSplines();

            CreateSplineContainer();

            CreateGrid();

            _finishedCreatingMap = true;

            OnMapCreationFinished.Invoke();
        }

        private void CreateSplines()
        {
            if (MapPrefab.MapContainer.Splines == null)
                return;

            var splines = MapPrefab.MapContainer.Splines.GetComponentsInChildren<SplineAuthoring>();

            foreach (var spline in splines)
            {
                CreateSpline(spline.SplineSegments);
            }
        }

        private void CreateSpline(List<SplineSegment> splineSegments)
        {
            Spline spline = new Spline();

            spline.Init(new NativeArray<SplineSegment>(splineSegments.ToArray(), Allocator.Persistent));

            Entity entity = CreateEntityFromType(typeof(Spline));

            _entityManager.SetComponentData(entity, spline);
        }

        private void CreateSplineContainer()
        {
            Entity entity = CreateEntityFromType(typeof(SplineContainer));

            _entityManager.SetComponentData(entity, new SplineContainer
            {
                IsSetUp = false
            });
        }

        private void CreateGrid()
        {

        }

        private Entity CreateEntityFromType(params ComponentType[] type)
        {
            EntityArchetype archetype = _entityManager.CreateArchetype(type);

            return _entityManager.CreateEntity(archetype);
        }
    }
}
