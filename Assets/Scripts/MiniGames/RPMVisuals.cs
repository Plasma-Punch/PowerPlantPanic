using System;
using System.Collections;
using UnityEngine;

public class RPMVisuals : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private ParticleSystem _sparks;
    [SerializeField]
    private ParticleSystem _smoke;
    private int _machineLvl = 0;
    [SerializeField, Range(0.1f, 1)]
    private float _speed = 1f;

    public void RaiseMachineLvl(Component sender, object obj)
    {
        string name = obj as string;
        if (name != gameObject.name) return;
        _machineLvl++;
        UpdateVisuals();
    }

    public void LowerMachineLvl(Component sender, object obj)
    {
        string name = obj as string;
        if (name != gameObject.name) return;
        if (0 >= _machineLvl) return;
        _machineLvl--;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        float multiplier =  1 - (_machineLvl * 0.1f);
        _animator.SetFloat("Speed", multiplier);
        if(_machineLvl != 0)
        {
            StartCoroutine(PlayParticle());
            _smoke.enableEmission = true;
        }
        else _smoke.enableEmission = false;
    }

    private IEnumerator PlayParticle()
    {
        yield return new WaitForSeconds(_speed);
        _sparks.Emit(UnityEngine.Random.Range(3, 8));
        if (_machineLvl != 0)
           StartCoroutine(PlayParticle());
    }
}