using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private FloatReference _timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DoTimer());
    }

    private IEnumerator DoTimer()
    {
        while (true)
        {
            _timer.variable.value += 1 * Time.deltaTime;
            yield return null;
        }
    }
}
