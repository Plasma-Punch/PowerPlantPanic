using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringVariable", menuName = "DataScripts / String Variable")]
public class StringVariable : ScriptableObject
{
    public event EventHandler ValueChanged;

    [SerializeField]
    private string _value;

    public string value
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
