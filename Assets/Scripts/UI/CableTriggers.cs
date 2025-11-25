using UnityEngine;
using System;

public class CableTriggers : MonoBehaviour
{
    [SerializeField]
    private GameEvent _placedCorrectly;
    private bool _isFilled;
    private Collider2D _collided;

    private bool _hasTriggered;

    private void OnEnable()
    {
        _isFilled = false;
        _hasTriggered = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != this.tag) return;
        _isFilled = true;
        _collided = other;
    }

    public void CheckCorrectness(Component sender, object obj)
    {
        if (!_isFilled) return;
        if (_hasTriggered) return;
        _collided.transform.parent = this.transform;
        _collided.transform.localPosition = Vector3.zero;
        _placedCorrectly.Raise(this, EventArgs.Empty);
        _hasTriggered = true;
    }
}
