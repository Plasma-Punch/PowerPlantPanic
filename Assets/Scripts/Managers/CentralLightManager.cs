using System;
using System.Collections.Generic;
using UnityEngine;

public class CentralLightManager : MonoBehaviour
{
    [SerializeField] 
    private List<SpriteRenderer> _lights = new List<SpriteRenderer>();
    [SerializeField]
    private Sprite _litLight;
    [SerializeField] 
    private GameEvent _Completed;

    private int _lightIndex;

    public void TurnOnLight()
    {
        if(_lightIndex <= 3)_lights[_lightIndex].sprite = _litLight;
        _lightIndex++;
        if (_lightIndex >= 4) _Completed.Raise(this, EventArgs.Empty);
    }
}
