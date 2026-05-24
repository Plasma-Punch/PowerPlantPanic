using UnityEngine;

public class Hammer: MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Machine") return;

    }
}
