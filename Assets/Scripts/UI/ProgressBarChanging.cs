using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarChanging : MonoBehaviour
{
    [SerializeField]
    private Image[] ProgressBars;

    private float _newBarValue, _oldBarValue;

    private void Start()
    {
        _oldBarValue = 0f;
    }

    void Update()
    {
        StartCoroutine(MoveProgressBar());
    }

    private IEnumerator MoveProgressBar()
    {
        _newBarValue = Random.Range(0, 1);

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            ProgressBars[0].fillAmount = Mathf.Lerp(_oldBarValue, _newBarValue, t);

            yield return null;
        }

        ProgressBars[0].fillAmount = _newBarValue;
        _oldBarValue = _newBarValue;

        
    }
}
