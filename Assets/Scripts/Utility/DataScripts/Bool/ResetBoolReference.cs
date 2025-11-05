using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBoolReference : MonoBehaviour
{
    [SerializeField]
    private BoolReference[] _boolReferences;

    private void Awake()
    {
        foreach (BoolReference f in _boolReferences)
        {
            f.variable.value = f.constantValue;
        }
    }
}
