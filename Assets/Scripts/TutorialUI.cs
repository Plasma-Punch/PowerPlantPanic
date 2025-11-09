using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _tutorialScreen;

    [SerializeField]
    private InputActionReference _closeTutorial;
    [SerializeField]
    private GameEvent _changeCanMove;

    private bool _canOpenPanel = true;

    private void Start()
    {
        _closeTutorial.action.performed += Close_Map_Performed;
    }

    private void Close_Map_Performed(InputAction.CallbackContext obj)
    {
        if (!_tutorialScreen.activeSelf) return;
        CloseUI();
    }

    public void OpenTutorialUI(Component sender, object obj)
    {
        if(!_canOpenPanel) return;
        _changeCanMove.Raise(this, false);
        _tutorialScreen.SetActive(true);
        _canOpenPanel = false;
    }

    public void CloseUI()
    {
        if (_canOpenPanel) return;
        _changeCanMove.Raise(this, true);
        _tutorialScreen.SetActive(false);
        StartCoroutine(AllowOpen());
    }

    private IEnumerator AllowOpen()
    {
        yield return new WaitForEndOfFrame();
        _canOpenPanel = true;
    }
}
