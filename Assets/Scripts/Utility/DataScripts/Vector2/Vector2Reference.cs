using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vector2Reference 
{
    public bool useConstant;
    public Vector2 constantValue;
    public Vector2Variable variable;

    public Vector3 Value
    {
        get
        {
            return useConstant ? constantValue :
                                 variable.value;
        }
    }

    public void SetValue(Vector2 value)
    {
        variable.value = value;
    }
}
