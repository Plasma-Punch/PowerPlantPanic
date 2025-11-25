using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FuseBoxManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _cableSpots = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _cableHolder = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _movealbeCables = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _cablePrefabs = new List<GameObject>();
    [SerializeField]
    private GameObject _UI;
    [SerializeField]
    private GameObject _powerTrigger;
    [SerializeField]
    private GameObject _fuseBoxTrigger;

    private List<GameObject> _spawnedCables = new List<GameObject>();
    private List<int> _spawnedIndex = new List<int>();

    private int _cablesToPlace;
    private int _cablesPlaced;

    [ContextMenu("initialize")]
    public void InitializeFuseBox(Component sender, object obj)
    {
        _UI.transform.parent.gameObject.SetActive(true);
        for(int i = 0; i < _cableHolder.Count; i++)
        {
            _movealbeCables[i].transform.parent = _cableHolder[i].transform.parent.transform;
            _movealbeCables[i].transform.localPosition = _cableHolder[i].transform.localPosition;
        }
        foreach (GameObject cableSpot in _cableSpots)
        {
            cableSpot.SetActive(true);
        }
        DestroySpawnedCables();
        _spawnedIndex.Clear();
        _cablesPlaced = 0;
        int cablesFixed = UnityEngine.Random.Range(0, 3);
        _cablesToPlace = 3 - cablesFixed;

        for (int i = 0; i < cablesFixed; i++)
        {
            int cableIndex = UnityEngine.Random.Range(0, _cableSpots.Count);

            while (_spawnedIndex.Contains(cableIndex))
            {
                cableIndex = UnityEngine.Random.Range(0, _cableSpots.Count);
            }

            GameObject cable = Instantiate(_cablePrefabs[cableIndex], _cableSpots[cableIndex].transform.position, Quaternion.identity);
            cable.transform.parent = _UI.transform;
            _cableSpots[cableIndex].SetActive(false);
            _spawnedCables.Add(cable);
            _spawnedIndex.Add(cableIndex);
        }
    }

    public void PlacedCableCorrectly(Component sender, object obj)
    {
        _cablesPlaced++;
        if (_cablesPlaced >= _cablesToPlace) FuseBoxFixed();
    }

    private void FuseBoxFixed()
    {
        StartCoroutine(CloseFuseBoxWithDelay());
    }

    private void DestroySpawnedCables()
    {
        foreach (GameObject cable in _spawnedCables)
        {
            Destroy(cable);
        }
        _spawnedCables.Clear();
    }

    private IEnumerator CloseFuseBoxWithDelay()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < _cableHolder.Count; i++)
        {
            _movealbeCables[i].transform.parent = _cableHolder[i].transform.parent.transform;
            _movealbeCables[i].transform.localPosition = _cableHolder[i].transform.localPosition;
        }
        DestroySpawnedCables();
        _powerTrigger.SetActive(true);
        _fuseBoxTrigger.SetActive(false);
        _UI.transform.parent.gameObject.SetActive(false);
    }
}
