using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetFloatReference : MonoBehaviour
{
    [SerializeField]
    private FloatReference[] _floatReferences;

    private void Awake()
    {
        foreach (FloatReference f in _floatReferences)
        {
            f.variable.value = f.constantValue;
        }
    }
}
