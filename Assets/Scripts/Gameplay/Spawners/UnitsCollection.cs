using System.Collections.Generic;
using UnityEngine;

public enum UnitName
{
    Basic = 1,
    Scout = 2,
    Tank = 3,
}
public class UnitsCollection : MonoBehaviour
{
    public static UnitsCollection Instance { get; private set; }

    [field: SerializeField] public List<GameObject> Units;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogWarning("Units Collection instance already exists!");
    }
}
