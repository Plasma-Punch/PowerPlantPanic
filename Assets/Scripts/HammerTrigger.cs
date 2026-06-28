using UnityEngine;

public class HammerTrigger : MonoBehaviour
{
    [SerializeField]
    private GameEvent _canHitMachine;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Machine") return;
        _canHitMachine.Raise(this, collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Machine") return;
        _canHitMachine.Raise(this, collision.gameObject);
    }
}