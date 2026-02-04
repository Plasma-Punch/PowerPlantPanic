using UnityEngine;

public class Light : MonoBehaviour
{
    [SerializeField]
    private Sprite _litLight;
    private SpriteRenderer _light;
    public void EnableLight()
    {
        _light = GetComponent<SpriteRenderer>();
        _light.sprite = _litLight;
    }
}
