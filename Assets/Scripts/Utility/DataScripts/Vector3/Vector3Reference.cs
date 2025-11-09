using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vector3Reference 
{
    public bool useConstant;
    public Vector3 constantValue;
    public Vector3Variable variable;

    public Vector3 Value
    {
        get
        {
            return useConstant ? constantValue :
                                 variable.value;
        }
    }

    public void SetValue(Vector3 value)
    {
        variable.value = value;
    }
}
