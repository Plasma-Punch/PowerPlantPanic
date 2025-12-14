using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField]
    private FloatReference _timer;
    [SerializeField]
    private TextMeshProUGUI _timerText;

    void Start()
    {
        _timer.variable.ValueChanged += Timer_Value_Changed;
    }

    private void Timer_Value_Changed(object sender, EventArgs e)
    {
        int minutes = (int)_timer.value / 60;
        int seconds = (int)_timer.value % 60;
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
