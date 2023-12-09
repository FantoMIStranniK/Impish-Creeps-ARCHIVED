using UnityEngine.SceneManagement;
using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

public enum MapType
{
    None,
    Base,
    Gameplay,
}
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class MapCheckingSystem : SystemBase
{
    public MapType CurrentMapType;

    private Dictionary<string, MapType> Maps;

    protected override void OnCreate()
    {
        CurrentMapType = MapType.Base;
        
        CreateMapDictionary();

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

            var scene = SceneManager.GetSceneByBuildIndex(i);

            string scenePrefix = scene.name.Split('_')[0];

            if (!Enum.TryParse(scenePrefix, out MapType mapType))
            {
                Debug.LogWarning($"WARNING: Possible invalid scene name prefix! Scene name: {scene.name}, scene index: {i}");

                continue;
            }

            if (i != 0)
                SceneManager.UnloadSceneAsync(i);

            Maps.Add(scene.name, mapType);
        }

        SceneManager.LoadScene(0);
    }

    private void OnLevelChange(Scene scene, LoadSceneMode mode)
    {
        if(!Maps.ContainsKey(scene.name))
        {
            Debug.LogError($"ERROR: Nonexistent map name as key! Scene name: {scene.name}");

            return;
        }

        CurrentMapType = Maps[scene.name];
    }
}
