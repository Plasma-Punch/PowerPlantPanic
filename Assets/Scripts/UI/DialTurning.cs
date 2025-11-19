using UnityEngine;

public class DialTurning : MonoBehaviour
{
    [SerializeField]
    private GameObject Dial01, Dial02;

    private Vector3 _dial01RotationSpeed, _dial02Rotation;

    private int _index;

    // Update is called once per frame
    void Update()
    {
        _index = Random.Range(0,2);

        if (_dial01RotationSpeed != null)
        {
            if (_index == 1)
                _dial01RotationSpeed = new Vector3(0, 0, 90);
            else
                _dial01RotationSpeed = new Vector3(0, 0, -45);


            Dial01.transform.Rotate(_dial01RotationSpeed * Time.deltaTime);
        }

        if (_dial02Rotation != null)
        {
            if (_index == 1)
                _dial02Rotation *= 5;
            else
                _dial02Rotation *= -5;


            Dial01.transform.rotation *= Quaternion.Euler(_dial02Rotation);
        }

    }
}
