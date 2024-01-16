using GC.SplineFramework;
using GC.SplineMovement;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            //CreateGrid();

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

        /*private void CreateGrid()
        {
            if (MapPrefab.MapContainer.Grid == null)
                return;

            var grid = MapPrefab.MapContainer.Grid.GetComponent<TowerGrid>();

            CreateTiles(grid);

            CreateGridContainer(grid);
        }

        private void CreateTiles(TowerGrid grid)
        {
            int j = 0;
            int i = 0;

            int flatArrayIndex = GetFlatArrayIndex(i, j, grid.RowWidth);

            while (flatArrayIndex < grid.Tiles.Count)
            {
                SetUpTile(grid.Tiles[flatArrayIndex], new int2(i, j));

                i++;

                if(i >= grid.RowWidth)
                {
                    i = 0;
                    j++;
                }

                flatArrayIndex = GetFlatArrayIndex(i, j, grid.RowWidth);
            }
        }

        private int GetFlatArrayIndex(int x, int y, int rowWidth)
            => x + y * rowWidth;

        private void SetUpTile(GameObject tile, int2 indexPosition)
        {
            RequireForUpdate<TileDeckComponent>();

            var tileComponent = tile.GetComponent<Tile>();

            var deck = SystemAPI.GetSingletonBuffer<TileDeckElement>(true);

            Entity entity = deck[(int)tileComponent.State].Tile;

            EntityManager.SetComponentData(entity, new TileDataComponent
            {
                State = tileComponent.State,
                IndexPosition = indexPosition
            });

            EntityManager.Instantiate(entity);
        }

        private void CreateGridContainer(TowerGrid grid)
        {
            Entity entity = CreateEntityFromType(typeof(GridComponent));

            _entityManager.SetComponentData(entity, new GridComponent
            {
                IsSetUp = false,
                RowWidth = grid.RowWidth
            });
        }*/

        private Entity CreateEntityFromType(params ComponentType[] type)
        {
            EntityArchetype archetype = _entityManager.CreateArchetype(type);

            return _entityManager.CreateEntity(archetype);
        }
    }
}
