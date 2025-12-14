using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vector2Variable", menuName = "DataScripts / Vector2 Variable")]
public class Vector2Variable : ScriptableObject
{
    public event EventHandler ValueChanged;

    [SerializeField]
    private Vector2 _value;

    public Vector2 value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
