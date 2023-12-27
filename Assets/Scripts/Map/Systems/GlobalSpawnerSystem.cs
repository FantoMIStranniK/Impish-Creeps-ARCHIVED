using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Collections;
using GC.SplineFramework;

namespace GC.Map
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GlobalSpawnerSystem : SystemBase
    {
        public static MapPrefab MapPrefab;

        public static Action OnMapDecoCreation;

        private bool FinishedCreatingMap;

        private EntityManager entityManager;

        protected override void OnCreate()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            FinishedCreatingMap = false;

            SceneManager.sceneLoaded += TryCreateMap;
        }

        protected override void OnUpdate(){}

        private void TryCreateMap(Scene scene, LoadSceneMode mode)
        {
            if (!CanStartCreatingMap(scene.name))
                return;

            CreateLevel();
        }

        private bool CanStartCreatingMap(string sceneName)
        {
            var mapCheckSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<MapCheckingSystem>();

            if (mapCheckSystem.CurrentMapType != MapType.Gameplay)
                return false;

            if (FinishedCreatingMap)
                return false;

            if (MapPrefab == null)
                return false;

            return true;
        }

        private void CreateLevel()
        {
            OnMapDecoCreation.Invoke();

            CreateSplines();

            CreateSplineContainer();

            CreateGrid();

            FinishedCreatingMap = true;
        }

        private void CreateSplines()
        {
            if (MapPrefab.Splines == null)
                return;

            var splines = MapPrefab.Splines.GetComponentsInChildren<SplineAuthoring>();

            foreach (var spline in splines)
            {
                CreateSpline(spline.SplineSegments);
            }
        }

        private void CreateSpline(List<SplineSegment> splineSegments)
        {
            SplineFramework.Spline spline = new SplineFramework.Spline();

            spline.Init(new NativeArray<SplineSegment>(splineSegments.ToArray(), Allocator.Persistent));

            Entity entity = CreateEntityFromType(typeof(SplineFramework.Spline));

            entityManager.SetComponentData(entity, spline);
        }

        private void CreateSplineContainer()
        {
            Entity entity = CreateEntityFromType(typeof(SplineContainer));

            entityManager.SetComponentData(entity, new SplineContainer());
        }

        private void CreateGrid()
        {

        }

        private Entity CreateEntityFromType(params ComponentType[] type)
        {
            EntityArchetype archetype = entityManager.CreateArchetype(type);

            return entityManager.CreateEntity(archetype);
        }
    }
}
