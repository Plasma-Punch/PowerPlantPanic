using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "DataScripts / Float Variable")]
public class FloatVariable : ScriptableObject
{
    public event EventHandler ValueChanged;
    
    [SerializeField]
    private float _value;

    public float value
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
