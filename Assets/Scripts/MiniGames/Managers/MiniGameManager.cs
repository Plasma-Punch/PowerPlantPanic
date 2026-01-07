using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _miniGameTriggers = new List<GameObject>();
    private IMiniGame _currentMiniGame;

    public void EnableOutputTrigger(Component sender, object obj)
    {
        foreach (GameObject go in _miniGameTriggers)
        {
            if (go.name != "OutputTrigger") continue;
            go.SetActive(!go.activeSelf);
            return;
        }
    }

    public void EnableTurbineTrigger(Component sender, object obj)
    {
        foreach (GameObject go in _miniGameTriggers)
        {
            if (go.name != "TurbineTrigger") continue;
            go.SetActive(!go.activeSelf);
            return;
            
        }
    }

    public void EnablePressureControlTrigger(Component sender, object obj)
    {
        foreach (GameObject go in _miniGameTriggers)
        {
            if (go.name != "PressureControlTrigger") continue;

            go.SetActive(!go.activeSelf);
            return;
        }
    }

    public void EnableWasteControlTrigger(Component sender, object obj)
    {
        foreach (GameObject go in _miniGameTriggers)
        {
            if (go.name != "WasteConsoleTrigger") continue;
            go.SetActive(!go.activeSelf);
            return;
        }
    }

    public void DisableOutputTrigger(Component sender, object obj)
    {
        foreach (GameObject go in _miniGameTriggers)
        {
            if (go.name != "OutputTrigger") continue;
            go.SetActive(false);
            return;
        }
    }

    public void DisableTurbineTrigger(Component sender, object obj)
    {
        foreach (GameObject go in _miniGameTriggers)
        {
            if (go.name != "TurbineTrigger") continue;
            go.SetActive(false);
            return;

        }
    }

    public void DisablePressureControlTrigger(Component sender, object obj)
    {
        foreach (GameObject go in _miniGameTriggers)
        {
            if (go.name != "PressureControlTrigger") continue;

            go.SetActive(false);
            return;
        }
    }

    public void DisableWasteControlTrigger(Component sender, object obj)
    {
       foreach (GameObject go in _miniGameTriggers)
       {
            if (go.name != "WasteConsoleTrigger") continue;
            go.SetActive(false);
            return;
       }
    }

    public void StartMiniGame(Component sender, object obj)
    {
        _currentMiniGame = obj as IMiniGame;
        if (_currentMiniGame == null) return;
        _currentMiniGame.StartMiniGame(this, EventArgs.Empty);
    }
}
