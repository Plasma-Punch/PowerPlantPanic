using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoors : MonoBehaviour
{
    [SerializeField]
    private GameObject doorLeft, doorRight;
    [SerializeField]
    private Transform doorLeftPosition, doorRightPosition;
    [SerializeField]
    private float doorSpeed = 1;
    [SerializeField]
    private GameObject _brokenCollider;
    [SerializeField]
    private ParticleSystem _sparks;
    [SerializeField]
    private List<GameObject> _consoleTriggers = new List<GameObject>();

    private Vector2 _leftClosedPosition, _rightOpenPosition;

    private bool _Works = true;

    private void Start()
    {
        _leftClosedPosition = doorLeft.transform.position;
        _rightOpenPosition = doorRight.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != 3f) return;
        if (!_Works) return;

        StopAllCoroutines();
        StartCoroutine(OpenDoors());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != 3f) return;
        if (!_Works) return;

        StopAllCoroutines();
        StartCoroutine(CloseDoors());
    }

    public void OpenDoor()
    {
        StartCoroutine(OpenDoors());
    }

    public void BreakDoor(Component sender, object obj)
    {
        GameObject door = obj as GameObject;
        if (door.name != gameObject.name) return;
        _sparks.gameObject.SetActive(true);
        _brokenCollider.SetActive(true);
        _Works = false;
        foreach (GameObject consoleTrigger in _consoleTriggers)
        {
            consoleTrigger.SetActive(true);
        }
        StopAllCoroutines();
        StartCoroutine(BrokenDoors());
    }

    IEnumerator OpenDoors() 
    {
        Vector3 leftDoorTargetPos = doorLeftPosition.position - transform.right * ((doorLeft.GetComponent<SpriteRenderer>().size.x / 2) * doorLeft.transform.localScale.x);
        Vector3 currentLeftDoorPos = doorLeft.transform.position;

        while(Vector2.Distance(currentLeftDoorPos, leftDoorTargetPos) > 0.01f)
        {
            doorLeft.transform.position = Vector2.MoveTowards(doorLeft.transform.position, doorLeftPosition.position - transform.right * ((doorLeft.GetComponent<SpriteRenderer>().size.x / 2) * doorLeft.transform.localScale.x), doorSpeed * Time.deltaTime);
            doorRight.transform.position = Vector2.MoveTowards(doorRight.transform.position, doorRightPosition.position + transform.right * ((doorRight.GetComponent<SpriteRenderer>().size.x / 2) * doorRight.transform.localScale.x), doorSpeed * Time.deltaTime);
            currentLeftDoorPos = doorLeft.transform.position;
            yield return null;
        }

        doorLeft.transform.position = doorLeftPosition.position;
        doorRight.transform.position = doorRightPosition.position;
    }

    IEnumerator CloseDoors()
    {
        Vector2 leftDoorTargetPos = _leftClosedPosition;
        Vector2 currentLeftDoorPos = doorLeft.transform.position;

        while (Vector2.Distance(currentLeftDoorPos, leftDoorTargetPos) > 0.01f)
        {
            doorLeft.transform.position = Vector2.MoveTowards(doorLeft.transform.position, _leftClosedPosition, doorSpeed * Time.deltaTime);
            doorRight.transform.position = Vector2.MoveTowards(doorRight.transform.position, _rightOpenPosition, doorSpeed * Time.deltaTime);
            currentLeftDoorPos = doorLeft.transform.position;
            yield return null;
        }

        doorLeft.transform.position = _leftClosedPosition;
        doorRight.transform.position = _rightOpenPosition;
    }

    IEnumerator BrokenDoors()
    {
        Vector3 leftDoorTargetPos = doorLeftPosition.position - transform.right * ((doorLeft.GetComponent<SpriteRenderer>().size.x / 2) * doorLeft.transform.localScale.x);
        Vector3 currentLeftDoorPos = doorLeft.transform.position;

        while (Vector2.Distance(currentLeftDoorPos, leftDoorTargetPos) > 0.01f)
        {
            doorLeft.transform.position = Vector2.MoveTowards(doorLeft.transform.position, doorLeftPosition.position - transform.right * ((doorLeft.GetComponent<SpriteRenderer>().size.x / 2) * doorLeft.transform.localScale.x), doorSpeed * 7 * Time.deltaTime);
            doorRight.transform.position = Vector2.MoveTowards(doorRight.transform.position, doorRightPosition.position + transform.right * ((doorRight.GetComponent<SpriteRenderer>().size.x / 2) * doorRight.transform.localScale.x), doorSpeed * 7 * Time.deltaTime);
            currentLeftDoorPos = doorLeft.transform.position;
            yield return null;
        }

        doorLeft.transform.position = doorLeftPosition.position;
        doorRight.transform.position = doorRightPosition.position;
        yield return null;
        leftDoorTargetPos = _leftClosedPosition;
        currentLeftDoorPos = doorLeft.transform.position;

        while (Vector2.Distance(currentLeftDoorPos, leftDoorTargetPos) > 0.01f)
        {
            doorLeft.transform.position = Vector2.MoveTowards(doorLeft.transform.position, _leftClosedPosition, doorSpeed * 7 * Time.deltaTime);
            doorRight.transform.position = Vector2.MoveTowards(doorRight.transform.position, _rightOpenPosition, doorSpeed * 7 * Time.deltaTime);
            currentLeftDoorPos = doorLeft.transform.position;
            PlayParticle();
            yield return null;
        }

        doorLeft.transform.position = _leftClosedPosition;
        doorRight.transform.position = _rightOpenPosition;

        if (!_Works) StartCoroutine(BrokenDoors());
    }

    private void PlayParticle()
    {
        _sparks.Emit(UnityEngine.Random.Range(1, 4));
    }
}
