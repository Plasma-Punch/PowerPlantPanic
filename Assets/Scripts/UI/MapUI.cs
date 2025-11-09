using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _mapUIImage;
    [SerializeField]
    private InputActionReference _closeMap;
    [SerializeField]
    private GameEvent _changeCanMove;

    private bool _canOpenPanel = true;

    private void Start()
    {
        _closeMap.action.performed += Close_Map_Performed;
    }

    private void Close_Map_Performed(InputAction.CallbackContext obj)
    {
        if (!_mapUIImage.activeSelf) return;
        CloseUI();
    }

    public void OpenMapUI(Component sender, object obj)
    {
        if (!_canOpenPanel) return;
        _changeCanMove.Raise(this, false);
        _mapUIImage.SetActive(true);
        _canOpenPanel = false;
    }

    public void CloseUI()
    {
        if (_canOpenPanel) return;
        _changeCanMove.Raise(this, true);
        _mapUIImage.SetActive(false);
        StartCoroutine(AllowOpen());
    }

    private IEnumerator AllowOpen()
    {
        yield return new WaitForEndOfFrame();
        _canOpenPanel = true;
    }
}
