using System;
using System.Collections.Generic;
using UnityEngine;

public class CentralLightManager : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> _lights = new List<SpriteRenderer>();
    [SerializeField] private GameEvent _Completed;

    private int _lightIndex;

    public void TurnOnLight()
    {
        _lights[_lightIndex].color = Color.green;
        _lightIndex++;
        if (_lightIndex >= 4) _Completed.Raise(this, EventArgs.Empty);
    }
}
