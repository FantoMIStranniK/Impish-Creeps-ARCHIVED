using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [Tooltip("Spline is given to spawned units. Units follow this spline.")]
    [SerializeField] private int SpawnerSplineIndex;
    [Space]
    [Tooltip("Time spend between two units being spawned.")]
    [SerializeField] private float timeBetweenSpawns = 1f;

    private Queue<int> _unitsQueue;

    //private EntitySplinesContainer _container;

    private void Awake()
    {
        //_container = GetComponentInParent<EntitySplinesContainer>();
    }

    public void CreateUnitsQueue(List<int> unitIndexes)
    {
        _unitsQueue.Clear();

        foreach (int unitIndex in unitIndexes)
        {
            _unitsQueue.Enqueue(unitIndex);
        }
    }

    public IEnumerator SpawnWave()
    {
        while(_unitsQueue.Count > 0)
        {
            var unit = UnitsCollection.Instance.Units[_unitsQueue.Dequeue()];

            SpawnUnit(unit);

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        yield return null;
    }

    private void SpawnUnit(GameObject unit)
    {
        //Redo this one for DOTS spawning pls

        //var splineComponent = unit.GetComponent<SplineAnimate>();

        //splineComponent.Container.Spline = _container.Splines[SpawnerSplineIndex].Spline;

        //nstantiate(unit, transform.position, transform.rotation);

    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
