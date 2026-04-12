using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private List<Image> _uiIcons = new List<Image>();
    [SerializeField]
    private List<TextMeshProUGUI> _uiText = new List<TextMeshProUGUI>();

    [Header("Sound Variables")]
    [SerializeField]
    private AudioClip _explosionSound;


    private SoundManager _soundManager;
    private Color _redColor = new Color(115, 12, 12, 255);

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

    private void ChangeIconColor(int maxValue, int value, Color startcolor, Color endcolor, Image icon = null, TextMeshProUGUI text = null)
    {
        float t = (float)value / (float)maxValue;
        Debug.Log($"{value} / {maxValue} = {t}");
        if(t == 1)
        {
            StartCoroutine(ChangeColor(startcolor / 255f, startcolor / 255f, icon, text));
        }

        if(t < 0.76f && t > 0.74f)
        {
            StartCoroutine(ChangeColor(startcolor / 255f, Color.yellow, icon, text));
        }
        if (t < 0.51f && t > 0.49f)
        {
            StartCoroutine(ChangeColor(Color.yellow, Color.orange, icon, text));
        }
        if (t < 0.26f && t > 0.24f)
        {
            StartCoroutine(ChangeColor(Color.orange, Color.red, icon, text));
        }
        if (t < 0.1f)
        {
            StartCoroutine(ChangeColor(Color.red, Color.darkRed, icon, text));
        }
    }

    private IEnumerator ChangeColor(Color start, Color end, Image icon = null, TextMeshProUGUI text = null)
    {
        float t = 0;
        if(icon!= null)
        {
            while (icon.color != end)
            {
                t += Time.deltaTime / 2;
                Color newColor = Color.Lerp(start, end, t);
                icon.color = newColor;
                yield return null;
            }
            icon.color = end;
            yield return null;
        }

        if (text != null)
        {
            while (text.color != end)
            {
                t += Time.deltaTime / 2;
                Color newColor = Color.Lerp(start, end, t);
                text.color = newColor;
                yield return null;
            }
            text.color = end;
            yield return null;
        }
        yield return null;
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
        ChangeIconColor(args.MaxPowerEfficiency, args.PowerEfficiency, new Color(155, 222, 136, 255), _redColor, null, _uiText[0]);
        ChangeIconColor(args.MaxPowerEfficiency, args.PowerEfficiency, new Color(155, 222, 136, 255), _redColor, _uiIcons[0], null);
    }

    public void FanRPMChanged(Component sender, object obj)
    {
        FanRPMChangedEventArgs args = obj as FanRPMChangedEventArgs;
        if (args == null) return;

        _fanRPM.text = args.FanRPM.ToString();
        ChangeIconColor(args.MaxFanRPM, args.FanRPM, new Color(155, 222, 136, 255), _redColor, null, _uiText[1]);
        ChangeIconColor(args.MaxFanRPM, args.FanRPM, new Color(155, 222, 136, 255), _redColor, _uiIcons[1], null);
    }

    public void PipePressureChanged(Component sender, object obj)
    {
        PipePresureEventArgs args = obj as PipePresureEventArgs;
        if (args == null) return;

        float newAngle = (args.PiperPressure / 150f  * 135f - 90f) * -1f;
        _pressureNeedle.transform.eulerAngles = new Vector3(0, 0, newAngle);
        ChangeIconColor(args.MaxPiperPressure, args.PiperPressure, new Color(155, 222, 136, 255), _redColor, _uiIcons[2], null);
        ChangeIconColor(args.MaxPiperPressure, args.PiperPressure, new Color(155, 222, 136, 255), _redColor, null, _uiText[2]);
        ChangeIconColor(args.MaxPiperPressure, args.PiperPressure, new Color(155, 222, 136, 255), _redColor, null, _uiText[3]);
        ChangeIconColor(args.MaxPiperPressure, args.PiperPressure, new Color(155, 222, 136, 255), _redColor, null, _uiText[4]);
        ChangeIconColor(args.MaxPiperPressure, args.PiperPressure, new Color(155, 222, 136, 255), _redColor, null, _uiText[5]);
    }

    public void WasteTimerChanged(Component sender, object obj)
    {
        WasteTimerChangedEventArgs args = obj as WasteTimerChangedEventArgs;
        if (args == null) return;

        _wasteLight.SetActive(!_wasteLight.activeSelf);
        ChangeIconColor(args.MaxWasteTimer, args.WasteTimer, new Color(155, 222, 136, 255), _redColor, _uiIcons[3], null);
        
        if (args.WasteTimer == 100) _wasteLight.SetActive(true);
        if (args.WasteTimer == 0) _wasteLight.SetActive(true);
    }

    public void GiveUp()
    {
        _soundManager.PlaySound("explosion");

        _gameLost.Raise(this, EventArgs.Empty);
    }
}
