using UnityEngine;

public class Light : MonoBehaviour
{
    private SpriteRenderer _light;
    public void EnableLight()
    {
        _light = GetComponent<SpriteRenderer>();
        _light.color = Color.green;
    }
}
