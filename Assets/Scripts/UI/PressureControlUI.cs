using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using NUnit.Framework.Internal;

public class PressureControlUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _ui;
    [SerializeField]
    private GameObject _glass;
    [SerializeField]
    private List<GameObject> _spawnPos = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _prefabs = new List<GameObject>();
    [SerializeField]
    private List<Transform> _valvePos = new List<Transform>();

    [Header("Audio Variable")]
    [SerializeField]
    private AudioClip _valveTurning;

    [SerializeField]
    private GameEvent _changeCanMove;
    [SerializeField]
    private GameEvent _enableValve;

    private SoundManager _soundManager;

    private List<GameObject> _spawnedValves = new List<GameObject>();

    private void OnEnable()
    {
        //_glass.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        if (GameObject.Find("SoundManager") != null)
            _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        else
            Debug.Log("SoundManager not found");
    }

    private void Start()
    {
        _soundManager.LoadSoundWithOutPath("turning", _valveTurning);
    }

    private void InitializeSpares()
    {
        foreach(GameObject obj in _spawnedValves)
        {
            Destroy(obj);
        }

        for(int i = 0; i < _prefabs.Count; i++)
        {
            GameObject valve = Instantiate(_prefabs[i], _spawnPos[i].transform);
            valve.transform.parent = _spawnPos[i].transform;
            valve.GetComponentInChildren<ValveUI>().enabled = false;
            valve.GetComponent<MouseDrag>().enabled = true;
            _spawnedValves.Add(valve);
        }
    }


    public void EnableUi(Component sender, object obj)
    {
        bool? setActive = obj as bool?;
        if ((bool)setActive)
        {
            InitializeSpares();
            _ui.SetActive(true);
            _changeCanMove.Raise(this, false);
        }
        else
        {
            _ui.SetActive(false);
            _changeCanMove.Raise(this, true);
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

    public void BreakGlass(Component sender, object obj)
    {
        _glass.GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    public void PlaceValve(Component sender, object obj)
    {
        if (obj == null) return;
        sender.gameObject.GetComponent<MouseDrag>().enabled = false;
        sender.gameObject.GetComponentInChildren<ValveUI>().enabled = true;
        _spawnedValves.Remove(sender.gameObject);
        _enableValve.Raise(sender, true);
    }
}
