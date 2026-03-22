using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;

public class ControlPanelUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _ui;
    [SerializeField]
    private TextMeshProUGUI _powerEfficiency;
    [SerializeField]
    private TextMeshProUGUI _fanRPM;
    [SerializeField]
    private GameObject _pressureNeedle;
    [SerializeField]
    private GameObject _wasteLight;
    [SerializeField]
    private GameEvent _gameLost;
    [SerializeField]
    private GameEvent _ChangeCanMove;

    [Header("Sound Variables")]
    [SerializeField]
    private AudioClip _explosionSound;


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
        _soundManager.LoadSoundWithOutPath("explosion", _explosionSound);
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

    public void PowerEfficiencyChanged(Component sender, object obj)
    {
        PowerEfficiencyChangedEventArgs args = obj as PowerEfficiencyChangedEventArgs;
        if (args == null) return;

        _powerEfficiency.text = $"{args.PowerEfficiency} %";
    }

    public void FanRPMChanged(Component sender, object obj)
    {
        FanRPMChangedEventArgs args = obj as FanRPMChangedEventArgs;
        if (args == null) return;

        _fanRPM.text = args.FanRPM.ToString();
    }

    public void PipePressureChanged(Component sender, object obj)
    {
        PipePresureEventArgs args = obj as PipePresureEventArgs;
        if (args == null) return;

        float newAngle = (args.PiperPressure / 150f  * 135f - 90f) * -1f;
        _pressureNeedle.transform.eulerAngles = new Vector3(0, 0, newAngle);
    }

    public void WasteTimerChanged(Component sender, object obj)
    {
        WasteTimerChangedEventArgs args = obj as WasteTimerChangedEventArgs;
        if (args == null) return;

        _wasteLight.SetActive(!_wasteLight.activeSelf);

        if (args.WasteTimer == 100) _wasteLight.SetActive(true);
        if (args.WasteTimer == 0) _wasteLight.SetActive(true);
    }

    public void GiveUp()
    {
        _soundManager.PlaySound("explosion");

        _gameLost.Raise(this, EventArgs.Empty);
    }
}
