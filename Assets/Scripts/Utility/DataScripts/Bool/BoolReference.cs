using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class BoolReference
{
    public bool useConstant;
    public bool constantValue;
    public BoolVariable variable;

    public EventHandler ValueChanged;

    private bool _isSubscribed = false;

    public bool value
    {
        get
        {
            if (variable == null && !useConstant)
                Debug.LogError($"{this} has no BoolVariable");

            return useConstant ? constantValue : 
                                 variable.value;
        }
    }

    public void UseEvent()
    {
        if (!useConstant && variable != null)
        {
            _isSubscribed = true;
            variable.ValueChanged += OnVariableValueChanged;
        }
        if (variable == null && !useConstant)
            Debug.LogError($"{this} has no BoolVariable");
    }

    private void OnVariableValueChanged(object sender, EventArgs e)
    {
        if (!useConstant)
            ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetValue(bool value)
    {
        if (useConstant)
            constantValue = value;
        else
        {
            variable.value = value;
        }
    }

    public void OnDestroy()
    {
        if (_isSubscribed && variable != null)
        {
            variable.ValueChanged -= OnVariableValueChanged;
            _isSubscribed = false;
        }
    }
}
