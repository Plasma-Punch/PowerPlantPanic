using System;
using UnityEngine;

[CreateAssetMenu(fileName = "IntVariable", menuName = "DataScripts / Int Variable")]
public class IntVariable : ScriptableObject
{
    public event EventHandler ValueChanged;

    [SerializeField]
    private int _value;

    public int value
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