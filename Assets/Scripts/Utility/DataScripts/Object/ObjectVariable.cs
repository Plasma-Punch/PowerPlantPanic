using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectVariable", menuName = "DataScripts / Object Variable")]
public class ObjectVariable : ScriptableObject
{
    public event EventHandler ValueChanged;

    [SerializeField]
    private object _value;

    public object value
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