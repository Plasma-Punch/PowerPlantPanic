using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hammer: MonoBehaviour
{
    [SerializeField]
    private InputAction _dropHammer;
    [SerializeField]
    private GameObject _hitbox;
    [SerializeField]
    private GameObject _itemHolder;
    [SerializeField]
    private InputAction _hitAction;
    [SerializeField]
    private GameEvent _fixMachine;

    private GameObject _target;
    private bool _isEquipped;
    private bool _isInOtherTrigger;

    private void Start()
    {
        _hitAction.performed += _hitAction_performed;
        _hitAction.Enable();
        _dropHammer.performed += _dropHammer_performed;
        _dropHammer.Enable();
    }

    private void Update()
    {
        if(!_isEquipped) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mousePos);
        mouseWorld.z = 0;
        _hitbox.transform.position = mouseWorld;
    }

    private void _dropHammer_performed(InputAction.CallbackContext context)
    {
        if(_isInOtherTrigger) return;
        if(!_isEquipped) return;
        transform.parent = null;
        _isEquipped = false;
    }

    private void _hitAction_performed(InputAction.CallbackContext obj)
    {
        if (_target == null) return;
        _fixMachine.Raise(this, _target);
    }

    public void EquipHammer(Component sender, object obj)
    {
        if (_isEquipped) return;
        transform.parent = _itemHolder.transform;
        transform.localPosition = Vector3.zero;
        _isEquipped = true;
    }

    public void SetTarget(Component sender, object obj)
    {
        _target = (GameObject)obj;
    }

    public void SetTrigger(Component sender, object obj)
    {
        if (sender.transform.parent.gameObject != transform.gameObject) return;
        _isInOtherTrigger = (bool)obj;
    }
}
