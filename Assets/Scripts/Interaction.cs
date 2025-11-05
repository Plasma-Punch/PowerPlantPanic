using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [SerializeField]
    private LayerMask _playerMask;
    [SerializeField]
    private GameEvent _changeInteractionUI;
    [SerializeField]
    private GameEvent _interact;
    [SerializeField]
    private MonoBehaviour _miniGame;
    [SerializeField]
    private InputActionReference _interactInput;

    private bool _isInTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsLayerInMask(_playerMask, collision.gameObject.layer)) return;

        _changeInteractionUI.Raise(this, true);
        _isInTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsLayerInMask(_playerMask, collision.gameObject.layer)) return;

        if (_changeInteractionUI != null)
            _changeInteractionUI.Raise(this, false);

        _isInTrigger = false;
    }
    private bool IsLayerInMask(LayerMask mask, int layer)
    {
        return (mask & (1 << layer)) != 0;
    }

    private void Update()
    {
        if (!_interactInput.action.WasPressedThisFrame()) return;
        if (!_isInTrigger) return;

        if (_miniGame == null)
        {
            _interact.Raise(this, EventArgs.Empty);
            _changeInteractionUI.Raise(this, false);
        }
        else
        {
            _interact.Raise(this, _miniGame as IMiniGame);
            _changeInteractionUI.Raise(this, false);
        }
    }
}
