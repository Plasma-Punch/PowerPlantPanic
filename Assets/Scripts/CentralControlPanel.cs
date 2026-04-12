using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CentralControlPanel : MonoBehaviour
{
    [SerializeField]
    private GameEvent _openControlPanel;
    [SerializeField]
    private float _minigamesInterval;
    [SerializeField]
    private List<GameEvent> _enableMiniGame = new List<GameEvent>();
    [SerializeField]
    private List<GameEvent> _disableMiniGame = new List<GameEvent>();
    [SerializeField]
    private GameEvent _powerEfficiencyChanged;
    [SerializeField]
    private GameEvent _fanRPMChanged;
    [SerializeField]
    private GameEvent _pipePressureChanged;
    [SerializeField]
    private GameEvent _wasteTimerChanged;
    [SerializeField]
    private InputActionReference _closePanel;
    [SerializeField]
    private GameEvent _gameLost;
    [SerializeField]
    private int _powerDrainAmount = 1;
    [SerializeField]
    private int _pressureDrainAmount = 10;
    [SerializeField]
    private int _RPMDrainAmount = 1;
    [SerializeField]
    private float _accumulateWasteAmount = 1;
    [SerializeField]
    private float _powerDrainSpeed = 0.5f;
    [SerializeField]
    private float _pressureDrainSpeed = 1;
    [SerializeField]
    private float _RPMDrainSpeed = 1;
    [SerializeField]
    private float _accumulateWasteSpeed = 1;
    [SerializeField]
    private int _maxBrokenMachines = 2;
    [SerializeField]
    private float _decreaseMultiplier = 1.5f;

    [Header("Sound Variables")]
    [SerializeField]
    private AudioClip _alarmSound;
    [SerializeField]
    private GameEvent _playAlarm;

    private SoundManager _soundManager;

    private int _lastEnabledMiniGame = -1;

    private float _powerEfficiency = 100;
    private float _fanRPM = 3600;
    private float _pipePSI = 150;
    private float _wasteTimer = 400;

    private Coroutine _decreaseOutputEfficiency;
    private Coroutine _decreaseFanRPM;
    private Coroutine _decreasePipePressure;
    private Coroutine _accumulateWaste;

    private bool _canDecreasePower = false;
    private bool _canDecreaseFanRPM = false;
    private bool _canDecreasePipePressure = false;
    private bool _canAccumulateWaste = false;

    private List<bool> _isMinigameEnabled = new List<bool>();
    private List<bool> _isMinigameBroken = new List<bool>();

    private int _activeMiniGames;

    private int _allowedActiveMinigames = 1;
    private int _completedMinigames = 0;
    private int _maxCompletedMinigames = 10;
    private int _machinesBroken = 0;

    private bool _canClosePanel = false;

    private void OnEnable()
    {
        if (GameObject.Find("SoundManager") != null)
            _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        else
            Debug.Log("SoundManager not found");

        for(int i = 0; i < 4; i++)
        {
            _isMinigameBroken.Add(false);
        }
    }

    private void Start()
    {
        foreach (GameEvent minigame in _enableMiniGame)
        {
            _isMinigameEnabled.Add(false);
        }

        int index = UnityEngine.Random.Range(0, _enableMiniGame.Count);

        if (_enableMiniGame.Count > 1)
        {
            while (index == _lastEnabledMiniGame)
            {
                index = UnityEngine.Random.Range(0, _enableMiniGame.Count);
            }
        }

        if (index != _lastEnabledMiniGame)
        {
            _isMinigameEnabled[index] = true;
            _enableMiniGame[(int)index].Raise(this, EventArgs.Empty);
            _activeMiniGames += 1;
        }

        _lastEnabledMiniGame = index;

        //Load in the sounds
        _soundManager.LoadSoundWithOutPath("Alarm", _alarmSound);
        _soundManager.SetSFXVolume(0.1f);

        StartCoroutine(SelectRandomMiniGame());
    }

    private IEnumerator SelectRandomMiniGame()
    {
        yield return new WaitForSeconds(_minigamesInterval);

        if (_activeMiniGames < _allowedActiveMinigames)
        {
            int index = UnityEngine.Random.Range(0, _enableMiniGame.Count);

            if(_enableMiniGame.Count > 1)
            {
                while (index == _lastEnabledMiniGame || _isMinigameEnabled[index])
                {
                    index = UnityEngine.Random.Range(0, _enableMiniGame.Count);
                }
            }

            if(index != _lastEnabledMiniGame)
            {
                _isMinigameEnabled[index] = true;
                _enableMiniGame[index].Raise(this, EventArgs.Empty);
                _activeMiniGames += 1;
            }

            _lastEnabledMiniGame = index;
        }

        StartCoroutine(SelectRandomMiniGame());
    }

    private IEnumerator DecreasePowerEfficiency()
    {
        if (!_canDecreasePower) yield break;
        float waitTime = _powerDrainSpeed / _powerDrainAmount;
        yield return new WaitForSeconds(waitTime);

        PlayAlarm();
        float multiplier = 1;
        if (_isMinigameBroken[3]) multiplier = _decreaseMultiplier;
        _powerEfficiency -= _powerDrainAmount * multiplier;

        _powerEfficiencyChanged.Raise(this, new PowerEfficiencyChangedEventArgs { PowerEfficiency = (int)_powerEfficiency, MaxPowerEfficiency = 100});
        _decreaseOutputEfficiency = StartCoroutine(DecreasePowerEfficiency());

        if (_powerEfficiency <= 0)
        {
            FailedMiniGame(new MiniGameFinishedEventArgs { FinishedMiniGame = MiniGame.PowerRegulating });
            _machinesBroken++;
            _isMinigameBroken[0] = true;
            CheckGameOver();
        }
    }

    private IEnumerator DecreaseFanRPM()
    {
        if (!_canDecreaseFanRPM) yield break;
        float waitTime = _RPMDrainSpeed / _RPMDrainAmount;
        yield return new WaitForSeconds(waitTime);

        PlayAlarm();
        float multiplier = 1;
        if (_isMinigameBroken[0]) multiplier = _decreaseMultiplier;
        _fanRPM -= _RPMDrainAmount * multiplier;

        _fanRPMChanged.Raise(this, new FanRPMChangedEventArgs { FanRPM = (int)_fanRPM, MaxFanRPM = 3600});
        _decreaseFanRPM = StartCoroutine(DecreaseFanRPM());

        if (_fanRPM <= 0)
        {
            FailedMiniGame(new MiniGameFinishedEventArgs { FinishedMiniGame = MiniGame.FanBlock });
            _machinesBroken++;
            _isMinigameBroken[1] = true;
            CheckGameOver();
        }
    }

    private IEnumerator DecreasePipePressure()
    {
        if (!_canDecreasePipePressure) yield break;
        float waitTime = _pressureDrainSpeed / _pressureDrainAmount;
        yield return new WaitForSeconds(waitTime);

        PlayAlarm();
        float multiplier = 1;
        if (_isMinigameBroken[1]) multiplier = _decreaseMultiplier;
        _pipePSI -= _pressureDrainAmount * multiplier;

        _pipePressureChanged.Raise(this, new PipePresureEventArgs { PiperPressure = (int)_pipePSI, MaxPiperPressure = 150});
        _decreasePipePressure = StartCoroutine(DecreasePipePressure());

        if (_pipePSI <= 0)
        {
            FailedMiniGame(new MiniGameFinishedEventArgs { FinishedMiniGame = MiniGame.PipeBroke });
            _machinesBroken++;
            _isMinigameBroken[2] = true;
            CheckGameOver();
        }
    }

    private IEnumerator AccumulateWaste()
    {
        if (!_canAccumulateWaste) yield break;
        yield return new WaitForSeconds(_accumulateWasteSpeed);

        PlayAlarm();
        float multiplier = 1;
        if (_isMinigameBroken[2]) multiplier = _decreaseMultiplier;
        _wasteTimer -= _accumulateWasteAmount * multiplier;

        _wasteTimerChanged.Raise(this, new WasteTimerChangedEventArgs { WasteTimer = (int)_wasteTimer, MaxWasteTimer = 400});
        _accumulateWaste = StartCoroutine(AccumulateWaste());

        if (_wasteTimer <= 0)
        {
            FailedMiniGame(new MiniGameFinishedEventArgs { FinishedMiniGame = MiniGame.WasteManagement });
            _machinesBroken++;
            _isMinigameBroken[3] = true;
            CheckGameOver();
        }
    }

    private IEnumerator AllowClose(bool state)
    {
        yield return new WaitForEndOfFrame();
        _canClosePanel = state;
    }

    public void OpenControlPanel(Component sender, object obj)
    {
        if (_canClosePanel) return;
        _openControlPanel.Raise(this, true);
        StartCoroutine(AllowClose(true));
    }

    public void StartOutputMiniGame(Component sender, object obj)
    {
        if (sender != this) return;

        _canDecreasePower = true;
        _decreaseOutputEfficiency = StartCoroutine(DecreasePowerEfficiency());
    }

    public void StartFanBlockMiniGame(Component sender, object obj)
    {
        if (sender != this) return;

        _canDecreaseFanRPM = true;
        _decreaseFanRPM = StartCoroutine(DecreaseFanRPM());
    }

    public void StartPipePresureMiniGame(Component sender, object obj)
    {
        if (sender != this) return;

        _canDecreasePipePressure = true;
        _decreasePipePressure = StartCoroutine(DecreasePipePressure());
    }

    public void StartWasteManagementMiniGame(Component sender, object obj)
    {
        if (sender != this) return;

        _canAccumulateWaste = true;
        _accumulateWaste = StartCoroutine(AccumulateWaste());
    }

    public void FinishedMiniGame(Component sender, object obj)
    {
        MiniGameFinishedEventArgs args = obj as MiniGameFinishedEventArgs;
        switch (args.FinishedMiniGame) 
        {
            case MiniGame.PowerRegulating:
                StopCoroutine(_decreaseOutputEfficiency);
                _canDecreasePower = false;
                _powerEfficiency = 100;
                _isMinigameEnabled[0] = false;
                _powerEfficiencyChanged.Raise(this, new PowerEfficiencyChangedEventArgs { PowerEfficiency = (int)_powerEfficiency, MaxPowerEfficiency = (int)_powerEfficiency });
                _disableMiniGame[0].Raise(this, EventArgs.Empty);
                StopAlarm();
                break;
            case MiniGame.FanBlock:
                StopCoroutine(_decreaseFanRPM);
                _canDecreaseFanRPM = false;
                _fanRPM = 3600;
                _isMinigameEnabled[1] = false;
                _fanRPMChanged.Raise(this, new FanRPMChangedEventArgs { FanRPM = (int)_fanRPM, MaxFanRPM = (int)_fanRPM });
                _disableMiniGame[1].Raise(this, EventArgs.Empty);
                StopAlarm();
                break;
            case MiniGame.PipeBroke:
                StopCoroutine(_decreasePipePressure);
                _canDecreasePipePressure = false;
                _pipePSI = 150;
                _isMinigameEnabled[2] = false;
                _pipePressureChanged.Raise(this, new PipePresureEventArgs { PiperPressure = (int)_pipePSI, MaxPiperPressure = (int)_pipePSI });
                _disableMiniGame[2].Raise(this, EventArgs.Empty);
                StopAlarm();
                break;
            case MiniGame.WasteManagement:
                StopCoroutine(_accumulateWaste);
                _canAccumulateWaste = false;
                _wasteTimer = 100;
                _isMinigameEnabled[3] = false;
                _wasteTimerChanged.Raise(this, new WasteTimerChangedEventArgs { WasteTimer = (int)_wasteTimer, MaxWasteTimer = (int)_wasteTimer });
                _disableMiniGame[3].Raise(this, EventArgs.Empty);
                StopAlarm();
                break;
        }
        _activeMiniGames -= 1;
        _completedMinigames += 1;

        if (_completedMinigames >= _maxCompletedMinigames)
        {
            _allowedActiveMinigames += 1;
            _maxCompletedMinigames = _maxCompletedMinigames * 2;
            _completedMinigames = 0;
        }
    }

    private void Update()
    {
        if (!_closePanel.action.WasPressedThisFrame()) return;
        if (!_canClosePanel) return;

        _openControlPanel.Raise(this, false);
        StartCoroutine(AllowClose(false));
    }

    private void PlayAlarm()
    {
        if (_soundManager.SfxSource.clip != null)
        {
            if (_soundManager.SfxSource.clip.name == "Alarm" && _soundManager.SfxSource.isPlaying) return;
        }
        _soundManager.PlaySound("Alarm");
        _playAlarm.Raise(this, EventArgs.Empty);
    }

    private void StopAlarm()
    {
        _soundManager.StopSound();
    }

    private void CheckGameOver()
    {
        if(_machinesBroken >= _maxBrokenMachines)
            _gameLost.Raise(this, EventArgs.Empty);
    }

    private void FailedMiniGame(MiniGameFinishedEventArgs args)
    {
        switch (args.FinishedMiniGame)
        {
            case MiniGame.PowerRegulating:
                StopCoroutine(_decreaseOutputEfficiency);
                _canDecreasePower = false;
                _powerEfficiency = 0;
                _powerEfficiencyChanged.Raise(this, new PowerEfficiencyChangedEventArgs { PowerEfficiency = (int)_powerEfficiency, MaxPowerEfficiency = (int)_powerEfficiency });
                _disableMiniGame[0].Raise(this, EventArgs.Empty);
                break;
            case MiniGame.FanBlock:
                StopCoroutine(_decreaseFanRPM);
                _canDecreaseFanRPM = false;
                _fanRPM = 0;
                _fanRPMChanged.Raise(this, new FanRPMChangedEventArgs { FanRPM = (int)_fanRPM, MaxFanRPM = (int)_fanRPM, });
                _disableMiniGame[1].Raise(this, EventArgs.Empty);
                break;
            case MiniGame.PipeBroke:
                StopCoroutine(_decreasePipePressure);
                _canDecreasePipePressure = false;
                _pipePSI = 0;
                _pipePressureChanged.Raise(this, new PipePresureEventArgs { PiperPressure = (int)_pipePSI, MaxPiperPressure = (int)_pipePSI });
                _disableMiniGame[2].Raise(this, EventArgs.Empty);
                break;
            case MiniGame.WasteManagement:
                StopCoroutine(_accumulateWaste);
                _canAccumulateWaste = false;
                _wasteTimer = 0;
                _wasteTimerChanged.Raise(this, new WasteTimerChangedEventArgs { WasteTimer = (int)_wasteTimer, MaxWasteTimer = (int)_wasteTimer });
                _disableMiniGame[3].Raise(this, EventArgs.Empty);
                break;
        }
        _activeMiniGames -= 1;
    }
}
