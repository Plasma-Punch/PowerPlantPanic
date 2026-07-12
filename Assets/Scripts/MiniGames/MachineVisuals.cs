using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class MachineVisuals : MonoBehaviour
{
    [SerializeField]
    private List<LevelVisials> _machineVisual = new();
    [SerializeField]
    private List<SpriteRenderer> _machineObjects = new();

    private int _machineLvl = 0;

    public void RaiseMachineLvl(Component sender, object obj)
    {
        string name = obj as string;
        if(name != gameObject.name) return;
        if (_machineVisual.Count - 1 <= _machineLvl) return;
            _machineLvl ++;
        UpdateVisuals();
    }

    public void LowerMachineLvl(Component sender, object obj)
    {
        string name = obj as string;
        if (name != gameObject.name) return;
        if (_machineVisual.Count - 1 >= _machineLvl) return;
        _machineLvl --;
        UpdateVisuals();
    }

    //[ContextMenu("Raise Machine Level")]
    private void UpdateVisuals()
    {
        for(int i = 0; i < _machineObjects.Count; i++)
        {
            _machineObjects[i].sprite = _machineVisual[_machineLvl].LevelVisial[i];
        }
    }
}

[Serializable]
public class LevelVisials
{
    public List<Sprite> LevelVisial = new();
}
