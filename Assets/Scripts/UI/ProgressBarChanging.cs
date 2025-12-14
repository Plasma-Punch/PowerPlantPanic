using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarChanging : MonoBehaviour
{
    [SerializeField]
    private Image ProgressBar;

    [SerializeField]
    private float ThreshHold;

    private float _newBarValue, _oldBarValue, _timerChangeValue = 0f;

    private Coroutine _fillingBar;

    private void Start()
    {
        _oldBarValue = ProgressBar.fillAmount;
    }

    void Update()
    {
        _timerChangeValue += Time.deltaTime;

        if (_timerChangeValue >= ThreshHold)
        {
            if (_fillingBar != null)
            {
                StopCoroutine(_fillingBar);
                _fillingBar = null;
            }
            else
            {
                _fillingBar = StartCoroutine(MoveProgressBar());
                _timerChangeValue = 0f;
            }    
        }  
    }

    private IEnumerator MoveProgressBar()
    {
        _newBarValue = Random.Range(1, 101);
        _newBarValue /= 100;

        float duration = 4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            ProgressBar.fillAmount = Mathf.Lerp(_oldBarValue, _newBarValue, t);

            yield return null;
        }

        ProgressBar.fillAmount = _newBarValue;
        _oldBarValue = _newBarValue;

        
    }
}
