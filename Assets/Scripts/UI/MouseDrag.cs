using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseDrag : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private bool _disableImage;
    [SerializeField]
    private GameEvent _onPlacement;
    [SerializeField]
    private string _tag;
    private bool _onObject, _clicked, _enteredObject;
    private float _timer;

    private void Update()
    {
        if(_onObject && _clicked)
        {
            transform.position = Input.mousePosition;
        }

        if (!_disableImage) return;
        if (_enteredObject && !_clicked)
        {
            _timer += Time.deltaTime;

            if (_timer > 1.5f)
            {
                _timer = 0;
                _enteredObject = false;
                this.gameObject.GetComponent<Image>().enabled = false;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _onObject = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_onObject) return;
        _clicked = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_onObject) return;
        _clicked = false;
        if (_enteredObject)
        {
            if (_onPlacement != null) _onPlacement.Raise(this, EventArgs.Empty);
            _clicked = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != _tag) return;
        _enteredObject = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != this.tag) return;
        _enteredObject = false;
    }

    private void OnEnable()
    {
        _timer = 0;
        _enteredObject = false;
        _onObject = false;
        _clicked = false;
    }
}
