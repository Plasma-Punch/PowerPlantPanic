using UnityEngine;

public class DialTurning : MonoBehaviour
{
    [SerializeField]
    private GameObject Dial01, Dial02;

    private int _angleIncrease = 45, _angleDecrease = -45;

    private Vector3 _dial01RotationSpeed, _dial02RotationSpeed;

    private int _indexDial01, _indexDial02;

    void Update()
    {
        _indexDial01 = Random.Range(0,2);
        _indexDial02 = Random.Range(0, 2);
        _angleIncrease = Random.Range(1,91);
        _angleDecrease = -Random.Range(1, 91);

        if (_dial01RotationSpeed != null)
        {
            if (_indexDial01 == 1)
                _dial01RotationSpeed = new Vector3(0, 0, _angleIncrease);
            else
                _dial01RotationSpeed = new Vector3(0, 0, _angleDecrease);


            Dial01.transform.Rotate(_dial01RotationSpeed * Time.deltaTime);
        }

        if (_dial02RotationSpeed != null)
        {
            if (_indexDial02 == 1)
                _dial02RotationSpeed = new Vector3(0, 0, _angleIncrease);
            else
                _dial02RotationSpeed = new Vector3(0, 0, _angleDecrease);


            Dial02.transform.Rotate(_dial02RotationSpeed * Time.deltaTime);
        }

    }
}
