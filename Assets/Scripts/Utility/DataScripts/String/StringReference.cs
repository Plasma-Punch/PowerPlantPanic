using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StringReference
{
    public bool useConstant;
    public string constantValue;
    public StringVariable variable;

    public EventHandler ValueChanged;

    private bool _isSubscribed = false;

    public string value
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

    public void SetValue(string value)
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
