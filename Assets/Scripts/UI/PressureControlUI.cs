using System.Collections.Generic;
using UnityEngine;
using System;

public class PressureControlUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _ui;

    [Header("Audio Variable")]
    [SerializeField]
    private AudioClip _valveTurning;

    [SerializeField]
    private GameEvent _ChangeCanMove;

    private SoundManager _soundManager;

    private void OnEnable()
    {
        if (GameObject.Find("SoundManager") != null)
            _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        else
            Debug.Log("SoundManager not found");
    }

    private void Start()
    {
        _soundManager.LoadSoundWithOutPath("turning", _valveTurning);
    }


    public void EnableUi(Component sender, object obj)

    {
        bool? setActive = obj as bool?;
        if ((bool)setActive)
        {
            _ui.SetActive(true);
            _ChangeCanMove.Raise(this, false);
        }
        else
        {
            _ui.SetActive(false);
            _ChangeCanMove.Raise(this, true);
        }
    }

    public void ValveRotationChanged(Component sender, object obj)
    {
        ValveRotationChangedEventArgs args = obj as ValveRotationChangedEventArgs;

        if (args == null) return;

        if(!_soundManager.SfxSource.isPlaying)
            _soundManager.PlaySound("turning");

        args.Valve.transform.eulerAngles = new Vector3(0, 0, args.ValveRotation * -1);
    }
}
