using System.Collections.Generic;
using UnityEngine;

public class BrokenDoorManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _doors = new List<GameObject>();
    [SerializeField]
    private GameEvent _breakDoor;

    private int _minigamesStarted = 0;

    private List<bool> _isDoorBroken = new List<bool>();

    private void Start()
    {
        for (int i = 0; i < _doors.Count; i++)
        {
            _isDoorBroken.Add(false);
        }
    }

    public void BreakDoor(Component sender, object obj)
    {
        _minigamesStarted++;

        if(_minigamesStarted < 3) return;
        int randNum = Random.Range(0, 4);

        if (randNum != 2) return;
        SelectrandomDoor();
    }

    private void SelectrandomDoor()
    {
        int randNum = Random.Range(0, _doors.Count);

        while (_isDoorBroken[randNum])
        {
            randNum = Random.Range(0, _doors.Count);
        }

        _isDoorBroken[randNum] = true;
        _breakDoor.Raise(this, _doors[randNum]);
    }
}
