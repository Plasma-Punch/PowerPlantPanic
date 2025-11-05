using System;
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

    private void Start()
    {
        _closeMap.action.performed += Close_Map_Performed;
    }

    private void Close_Map_Performed(InputAction.CallbackContext obj)
    {
        OpenMapUI(this, EventArgs.Empty);
    }

    public void OpenMapUI(Component sender, object obj)
    {
        _changeCanMove.Raise(this, _mapUIImage.activeSelf);
        _mapUIImage.SetActive(!_mapUIImage.activeSelf);
    }
}
