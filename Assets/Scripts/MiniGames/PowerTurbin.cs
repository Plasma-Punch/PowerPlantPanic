using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class PowerTurbin : MonoBehaviour, IMiniGame
{
    [SerializeField]
    private GameObject turbineScreen;
    [SerializeField]
    private GameEvent turbinCleared;
    [SerializeField]
    private GameEvent _changeCanWalk;


    private List<GameObject> garbagePositions = new List<GameObject>();
    private GameObject[] _garbageList = new GameObject[] { };
    private GarbageCollection _garbadgeCollection;

    private bool _isActive;

    private void Start()
    {
        Transform[] allObjects = turbineScreen.GetComponentsInChildren<Transform>();

        foreach (Transform trans in allObjects)
        {
            if (trans.gameObject.name.StartsWith("Position"))
            {
                garbagePositions.Add(trans.gameObject);
            }
        }

        _garbadgeCollection = turbineScreen.GetComponentInChildren<GarbageCollection>();

        //Subscribe here on the garbage collector event
        _garbadgeCollection.RemoveAllGarbage.AddListener(OnAllGarbadgeRemoved);
    }

    public void OnAllGarbadgeRemoved()
    {
        StartCoroutine(DelayedUIClose());
    }

    public void completed()
    {
        turbinCleared.Raise(this, new MiniGameFinishedEventArgs{FinishedMiniGame = MiniGame.FanBlock});
        turbineScreen.SetActive(false);
        _changeCanWalk.Raise(this, true);
        _isActive = false;
    }

    public void failed()
    {
        Debug.Log("There is no failing, only losing.");
    }

    public void StartMiniGame(Component sender, object obj)
    {
        if (_isActive) return;
        _isActive = true;
        turbineScreen.SetActive(true);
        _changeCanWalk.Raise(this, false);

        //Make garbage visible and put it on the right location
        SetGarabageLocation();
    }

    IEnumerator DelayedUIClose()
    {
        yield return new WaitForSeconds(1f);

        completed();
    }

    private void SetGarabageLocation()
    {
        _garbageList = GameObject.FindGameObjectsWithTag("Garbage");

        //Check if there are enough positions
        if (garbagePositions.Count != _garbageList.Length)
        {
            Debug.Log("Not enough positions for all garbage to be positioned!");
            return;
        }

        int garbageIndex = 0;
        foreach (var garbage in _garbageList)
        {
            garbage.transform.position = garbagePositions[garbageIndex].transform.position;
            garbageIndex++;
            garbage.GetComponent<Image>().enabled = true;
        }
    }
}
