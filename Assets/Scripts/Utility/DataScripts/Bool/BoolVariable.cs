using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolVariable", menuName = "DataScripts / Bool Variable")]
public class BoolVariable : ScriptableObject
{
    public event EventHandler ValueChanged;

    [SerializeField]
    private bool _value;

    public bool value
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