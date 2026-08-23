using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

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
    private bool _playSpark = false;

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

    public void StartParticles(Component sender, object obj)
    {
        _playSpark = true;
        StartCoroutine(PlayParticle());
        _smoke.emissionRate = 30;
    }

    public void StopParticles(Component sender, object obj)
    {
        MiniGameFinishedEventArgs args = obj as MiniGameFinishedEventArgs;
        switch (args.FinishedMiniGame) 
        { 
            case MiniGame.FanBlock:
                _playSpark = false;
                _smoke.emissionRate = 0;
                break;
        }
    }

    private void UpdateVisuals()
    {
        float multiplier =  1 - (_machineLvl * 0.1f);
        _animator.SetFloat("Speed", multiplier);
        if (_machineLvl == 0)
        {
            _smoke.emissionRate = 0;
            _playSpark = false;
        }
    }

    private IEnumerator PlayParticle()
    {
        yield return new WaitForSeconds(_speed);
        _sparks.Emit(UnityEngine.Random.Range(3, 8));
        if (_playSpark)
           StartCoroutine(PlayParticle());
    }
}