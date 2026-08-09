using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class WasteVisuals : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> _machineVisual = new();
    [SerializeField]
    private List<SpriteRenderer> _machineObjects = new();

    private int _machineLvl = 0;

    public void RaiseMachineLvl(Component sender, object obj)
    {
        string name = obj as string;
        if (name != gameObject.name) return;
        if (_machineVisual.Count < _machineLvl) return;
        _machineLvl++;
        _machineObjects[_machineLvl - 1].sprite = _machineVisual[_machineLvl - 1];
        GameObject pool = _machineObjects[_machineLvl - 1].gameObject;
        StartCoroutine(ScaleUpSprite(pool));
    }

    public void LowerMachineLvl(Component sender, object obj)
    {
        string name = obj as string;
        if (name != gameObject.name) return;
        if (0 >= _machineLvl) return;
        _machineLvl--;
        _machineObjects[_machineLvl].sprite = _machineVisual[_machineLvl];
        GameObject pool = _machineObjects[_machineLvl].gameObject;
        StartCoroutine(ScaleDownSprite(pool));
    }

    private IEnumerator ScaleUpSprite(GameObject obj)
    {
        float scale = 0;

        while (scale < 1)
        {
            scale += Time.deltaTime;
            obj.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        scale = 1;
        obj.transform.localScale = new Vector3(scale, scale, scale);
    }
    private IEnumerator ScaleDownSprite(GameObject obj)
    {
        float scale = 0;

        while (scale < 1)
        {
            scale += Time.deltaTime;
            obj.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        scale = 1;
        obj.transform.localScale = new Vector3(scale, scale, scale);
    }
}
