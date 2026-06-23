using System;
using System.Collections;
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
    [SerializeField]
    private GameEvent _pickUpItem;

    private GameObject _target;
    private GameObject _equipedItem;
    private bool _isInOtherTrigger;
    private bool _canPickUpItem = true;

    private void Start()
    {
        _hitAction.performed += _hitAction_performed;
        _hitAction.Enable();
        _dropHammer.performed += _dropHammer_performed;
        _dropHammer.Enable();
    }

    private void Update()
    {
        if(_equipedItem != this.gameObject) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mousePos);
        mouseWorld.z = 0;
        _hitbox.transform.position = mouseWorld;
    }

    private void _dropHammer_performed(InputAction.CallbackContext context)
    {
        if(_isInOtherTrigger) return;
        if(_equipedItem != this.gameObject) return;
        _canPickUpItem = false;
        transform.parent = null;
        _hitbox.transform.localPosition = Vector3.zero;
        _equipedItem = null;
        _pickUpItem.Raise(this, _equipedItem);
        StartCoroutine(CanPickUp());
    }

    private void _hitAction_performed(InputAction.CallbackContext obj)
    {
        if (_target == null) return;
        _fixMachine.Raise(this, _target.name);
    }

    private IEnumerator CanPickUp()
    {
        yield return new WaitForEndOfFrame();
        _canPickUpItem = true;
    }

    public void EquipHammer(Component sender, object obj)
    {
        if (!_canPickUpItem) return;
        if (_equipedItem != null) return;
        transform.parent = _itemHolder.transform;
        transform.localPosition = Vector3.zero;
        _equipedItem = this.gameObject;
        _pickUpItem.Raise(this, _equipedItem);
    }

    public void SetTarget(Component sender, object obj)
    {
        _target = (GameObject)obj;
    }

    public void SetTrigger(Component sender, object obj)
    {
        if (sender.transform.parent.gameObject.tag != "Machine") return;
        _isInOtherTrigger = (bool)obj;
    }
}
