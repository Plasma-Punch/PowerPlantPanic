using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlidingDoors : MonoBehaviour
{
    [SerializeField]
    private GameObject doorLeft, doorRight;
    [SerializeField]
    private Transform doorLeftPosition, doorRightPosition;
    [SerializeField]
    private float doorSpeed = 1;

    private Vector2 _leftClosedPosition, _rightOpenPosition;

    private void Start()
    {
        _leftClosedPosition = doorLeft.transform.position;
        _rightOpenPosition = doorRight.transform.position;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != 3f) return;

        StopAllCoroutines();
        StartCoroutine(OpenDoors());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != 3f) return;

        StopAllCoroutines();
        StartCoroutine(CloseDoors());
    }

    public void OpenDoor()
    {
        StartCoroutine(OpenDoors());
    }

    IEnumerator OpenDoors() 
    {
        Vector3 leftDoorTargetPos = doorLeftPosition.position - transform.right * ((doorLeft.GetComponent<SpriteRenderer>().size.x / 2) * doorLeft.transform.localScale.x);
        Vector3 currentLeftDoorPos = doorLeft.transform.position;

        while(Vector2.Distance(currentLeftDoorPos, leftDoorTargetPos) > 0.01f)
        {
            doorLeft.transform.position = Vector2.MoveTowards(doorLeft.transform.position, doorLeftPosition.position - transform.right * ((doorLeft.GetComponent<SpriteRenderer>().size.x / 2) * doorLeft.transform.localScale.x), doorSpeed * Time.deltaTime);
            doorRight.transform.position = Vector2.MoveTowards(doorRight.transform.position, doorRightPosition.position + transform.right * ((doorRight.GetComponent<SpriteRenderer>().size.x / 2) * doorRight.transform.localScale.x), doorSpeed * Time.deltaTime);

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

            yield return null;
        }

        doorLeft.transform.position = _leftClosedPosition;
        doorRight.transform.position = _rightOpenPosition;
    }
}
