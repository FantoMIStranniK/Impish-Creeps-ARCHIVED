using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;
using GC.Map;

public enum MapType
{
    None,
    Base,
    Gameplay,
    SubScene,
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(GlobalSpawnerSystem))]
public partial class MapCheckingSystem : SystemBase
{
    public MapType CurrentMapType = MapType.None;

    private Dictionary<string, MapType> Maps;

    protected override void OnCreate()
    {
        CreateMapDictionary();

        CurrentMapType = MapType.Base;

        SceneManager.sceneLoaded += OnLevelChange;
    }

    protected override void OnUpdate(){}

    private void CreateMapDictionary()
    {
        Maps = new();

        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for(int i = 0; i < sceneCount; i++)
        {
            SceneManager.LoadScene(i);

            if (!TryGetMapType(out Scene scene, out MapType mapType, i))
                continue;

            if(i != 0)
                SceneManager.UnloadSceneAsync(i);

            Maps.Add(scene.name, mapType);
        }

        SceneManager.LoadScene(0);
    }

    private bool TryGetMapType(out Scene scene, out MapType mapType, int sceneIndex)
    {
        scene = SceneManager.GetSceneByBuildIndex(sceneIndex);

        string scenePrefix = scene.name.Split('_')[0];

        if (!Enum.TryParse(scenePrefix, out mapType))
        {
            Debug.LogWarning($"WARNING: Possible invalid scene name prefix! Scene name: {scene.name}, scene index: {sceneIndex}");

            return false;
        }

        return true;
    }

    private void OnLevelChange(Scene scene, LoadSceneMode mode)
    {
        if (Maps[scene.name] == MapType.SubScene)
            return;

        if (scene.isSubScene)
            return;

        if(!Maps.ContainsKey(scene.name))
        {
            Debug.LogError($"ERROR: Nonexistent map name as key! Scene name: {scene.name}");

            return;
        }

        CurrentMapType = Maps[scene.name];
    }

    public void ResetSystem()
    {
        throw new NotImplementedException();
    }
}
