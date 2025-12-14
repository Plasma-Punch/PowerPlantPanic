using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PresureRegulator : MonoBehaviour, IMiniGame
{
    [SerializeField]
    private List<GameObject> _pipes = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _pipePrefabs = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _brokenPipePrefabs = new List<GameObject>();
    [SerializeField]
    private GameEvent _ValveRotationChanged;
    [SerializeField]
    private GameEvent _openPressureControlUI;
    [SerializeField]
    private GameObject _itemHolder;
    [SerializeField]
    private InputActionReference _closePanel;
    [SerializeField]
    private GameEvent _miniGameFinished;
    
    [Header("AudioVariables")]
    [SerializeField]
    private AudioClip _grabSound;
    [SerializeField]
    private int _valveTurnSpeed = 40;
    [SerializeField]
    private GameObject _trashcanTrigger;
    [SerializeField]
    private List<GameObject> _pipeTriggers = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _PipeSpawnerTriggers = new List<GameObject>();

    private SoundManager _soundManager;

    private GameObject _brokenPipe;
    private int _currentBrokenPipeIndex;

    private GameObject _activeValve;
    private GameObject _heldItem;

    private float _valveProgress = 0;
    private bool _valveIsOpen = true;
    private bool _isCarryingPipe = false;
    private bool _itemPlaced = false;
    private bool _valveLocked = false;
    private bool _miniGameStarted = false;
    private bool _pipeRemoved = false;
    private bool _uiIsOpen = false;
    private bool _canOpenPanel = true;

    private int _previousPipe;

    private void OnEnable()
    {
        if (GameObject.Find("SoundManager") != null)
            _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        else
            Debug.Log("SoundManager not found");
    }

    private void Start()
    {
        _soundManager.LoadSoundWithOutPath("grab", _grabSound);
    }

    private void SetRandomBrokenPipe()
    {
        int randomPipe = UnityEngine.Random.Range(0, _pipes.Count);

        while(randomPipe == _previousPipe)
        {
            randomPipe = UnityEngine.Random.Range(0, _pipes.Count);
        }

        _currentBrokenPipeIndex = randomPipe;

        for (int i = 0; i < _brokenPipePrefabs.Count; i++)
        {
            if (_brokenPipePrefabs[i].tag != _pipes[randomPipe].tag) continue;
            GameObject brokenPipe = Instantiate(_brokenPipePrefabs[i], _pipes[randomPipe].transform.position, _pipes[randomPipe].transform.rotation);
            _brokenPipe = brokenPipe;
            _pipeTriggers[_currentBrokenPipeIndex].SetActive(true);
            _PipeSpawnerTriggers[_currentBrokenPipeIndex].SetActive(true);
            _trashcanTrigger.SetActive(true);
        }

        Destroy(_pipes[randomPipe]);
    }

    public void StartMiniGame(Component sender, object obj)
    {
        if (_miniGameStarted) return;
        _miniGameStarted = true;
        SetRandomBrokenPipe();
    }

    public void RemovePipe(Component sender, object obj)
    {
        string pipeHolderTag = sender.gameObject.transform.parent.tag;
        if (_isCarryingPipe)
        {
            PlacePipe(pipeHolderTag, sender.transform.parent.gameObject);
            return;
        }

        if (pipeHolderTag != _brokenPipe.tag) return;
        if (_valveIsOpen) return;
        _pipeRemoved = true;
        _isCarryingPipe = true;
        _heldItem = _brokenPipe;
        _brokenPipe.transform.parent = _itemHolder.transform;
        _brokenPipe.transform.localPosition = Vector3.zero;
        _brokenPipe.transform.localEulerAngles = new Vector3(0, 0, 90);

        _soundManager.SetSFXVolume(1);
        _soundManager.PlaySound("grab");
    }

    private void PlacePipe(string tag, GameObject holder)
    {
        if (_heldItem.tag != tag) return;
        if (_brokenPipe != null) return;
        if (!_pipeRemoved) return;

        _heldItem.transform.parent = holder.transform;
        _heldItem.transform.localPosition = Vector3.zero;
        _heldItem.transform.localEulerAngles = new Vector3(0, 0, 0);

        _pipes[_currentBrokenPipeIndex] = _heldItem;
        _heldItem = null;
        _itemPlaced = true;

        _soundManager.StopSound();
    }

    public void GrabItem(Component sender, object obj)
    {
        if (_heldItem != null) return;
        string pipeHolderTag = sender.gameObject.transform.parent.tag;

        _soundManager.PlaySound("grab");

        for(int i = 0; i < _pipePrefabs.Count; i++)
        {
            if (_pipePrefabs[i].tag != pipeHolderTag) continue;
            GameObject go = Instantiate(_pipePrefabs[i]);
            _heldItem = go;
            go.transform.parent = _itemHolder.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        _isCarryingPipe = true;
    }

    public void TrashItem(Component sender, object obj)
    {
        if (sender.transform.parent.gameObject.transform.parent.gameObject != gameObject) return;
        if (_heldItem == null) return;

        _isCarryingPipe = false;
        Destroy(_heldItem.gameObject);
    }

    public void completed()
    {
        _miniGameStarted = false;
        _valveProgress = 0;
        _valveIsOpen = true;
        _isCarryingPipe = false;
        _itemPlaced = false;
        _valveLocked = false;
        _pipeRemoved = false;
        _heldItem = null;
        _activeValve = null;
        _brokenPipe = null;
        _pipeTriggers[_currentBrokenPipeIndex].SetActive(false);
        _PipeSpawnerTriggers[_currentBrokenPipeIndex].SetActive(false);
        _trashcanTrigger.SetActive(false);
        _currentBrokenPipeIndex = -1;
        _miniGameFinished.Raise(this, new MiniGameFinishedEventArgs{ FinishedMiniGame = MiniGame.PipeBroke});
    }

    public void failed()
    {
        _miniGameStarted = false;
        _valveProgress = 0;
        _valveIsOpen = true;
        _isCarryingPipe = false;
        _itemPlaced = false;
        _valveLocked = false;
        _pipeRemoved = false;
        _miniGameStarted = false;
        _heldItem = null;
        _activeValve = null;
        _brokenPipe = null;
        _pipeTriggers[_currentBrokenPipeIndex].SetActive(false);
        _PipeSpawnerTriggers[_currentBrokenPipeIndex].SetActive(false);
        _trashcanTrigger.SetActive(false);
        _currentBrokenPipeIndex = -1;
    }

    public void GetActiveValve(Component sender, object obj)
    {
        _activeValve = obj as GameObject;
    }

    public void OpenPressureControl(Component sender, object obj)
    {
        if (_heldItem != null) return;
        if (_uiIsOpen) return;
        if (!_canOpenPanel) return;
        _canOpenPanel = false;
        _uiIsOpen = true;
        _openPressureControlUI.Raise(this, true);
        StartMiniGame(sender, obj);
    }

    private void Update()
    {
        if (_closePanel.action.WasPressedThisFrame())
        {
            if (_uiIsOpen)
            {
                _openPressureControlUI.Raise(this, false);
                _uiIsOpen = false;
                StartCoroutine(CanOpenPanel());
            }
        }

        if (!_miniGameStarted) return;
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _valveLocked = false;
        }
        if (!Mouse.current.leftButton.IsPressed()) return;
        if (_activeValve == null) return;

        switch (_currentBrokenPipeIndex)
        {
            case 0:
                if (_activeValve.tag != "Red") break;
                if (_valveLocked) break;
                if (_valveIsOpen) _valveProgress += _valveTurnSpeed * Time.deltaTime;
                else if (!_valveIsOpen && _itemPlaced) _valveProgress -= _valveTurnSpeed * Time.deltaTime;
                _ValveRotationChanged.Raise(this, new ValveRotationChangedEventArgs { ValveRotation = _valveProgress, Valve = _activeValve });
                break;
            case 1:
                if (_activeValve.tag != "Green") break;
                if (_valveLocked) break;
                if (_valveIsOpen) _valveProgress += _valveTurnSpeed * Time.deltaTime;
                else if (!_valveIsOpen && _itemPlaced) _valveProgress -= _valveTurnSpeed * Time.deltaTime;
                _ValveRotationChanged.Raise(this, new ValveRotationChangedEventArgs { ValveRotation = _valveProgress, Valve = _activeValve });
                break;
            case 2:
                if (_activeValve.tag != "Blue") break;
                if (_valveLocked) break;
                if (_valveIsOpen) _valveProgress += _valveTurnSpeed * Time.deltaTime;
                else if (!_valveIsOpen && _itemPlaced) _valveProgress -= _valveTurnSpeed * Time.deltaTime;
                _ValveRotationChanged.Raise(this, new ValveRotationChangedEventArgs { ValveRotation = _valveProgress, Valve = _activeValve });
                break;
        }

        if (_valveProgress > 180)
        {
            _valveProgress = 180;
            _valveIsOpen = false;
            _valveLocked = true;
        }


        if (_valveProgress < 0)
        {
            _valveProgress = 0;
            _valveIsOpen = true;
            _valveLocked = true;

            completed();
        }
    }

    private IEnumerator CanOpenPanel()
    {
        yield return new WaitForEndOfFrame();
        _canOpenPanel = true;
    }
}
