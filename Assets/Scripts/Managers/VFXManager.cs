using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private Volume _globalVolume;
    [SerializeField, Range(0,1)] private float _maxIntensity;
    [SerializeField, Range(0,2)] private float _speedMultiplier;

    [SerializeField] ParticleSystem _steamParticle;


    private Vignette _vignette;
    private bool _vignetteIsPlaying;

    private void Start()
    {
        if (_globalVolume.profile.TryGet(out _vignette))
        {
            // Make sure the override is active
            _vignette.active = true;
        }
    }
    public void PlayAlarm(Component sender, object obj)
    {
        if(!_vignetteIsPlaying)
            StartCoroutine(ChangeVignette());
    }

    public void PlaySteam(Component sender, object obj)
    {
        Transform pos = obj as Transform;
        if (_steamParticle.emissionRate == 30)
            _steamParticle.emissionRate = 0;
        else
        {
            _steamParticle.emissionRate = 30;
            _steamParticle.transform.position = pos.position;
        }
    }

    private IEnumerator ChangeVignette()
    {
        _vignetteIsPlaying = true;
        float intensity = 0;

        while(intensity < _maxIntensity)
        {
            intensity += Time.deltaTime * _speedMultiplier;
            _vignette.intensity.value = intensity;
            yield return null;
        }

        intensity = _maxIntensity;
        _vignette.intensity.value = intensity;

        while (intensity > 0)
        {
            intensity -= Time.deltaTime * _speedMultiplier;
            _vignette.intensity.value = intensity;
            yield return null;
        }
        intensity = 0;
        _vignette.intensity.value = intensity;
        _vignetteIsPlaying = false;
    }
}
