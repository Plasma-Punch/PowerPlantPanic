using System;
using UnityEngine;

[Serializable]
public class FloatReference
{
    public bool useConstant;
    public float constantValue;
    public FloatVariable variable;

    public EventHandler ValueChanged;

    private bool _isSubscribed = false;

    public float value
    {
        get
        {
            if (variable == null && !useConstant)
                Debug.LogError($"{this} has no FloatVariable");

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
            Debug.LogError($"{this} has no FloatVariable");
    }

    private void OnVariableValueChanged(object sender, EventArgs e)
    {
        if(!useConstant)
            ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetValue(float value)
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
