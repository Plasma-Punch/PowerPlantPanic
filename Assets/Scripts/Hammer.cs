using UnityEngine;

public class HammerHitHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Machine") return;

    }
}
