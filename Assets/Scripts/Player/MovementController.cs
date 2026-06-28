using DialogueEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rigidbody2D;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rb;
    [SerializeField]
    private float _movingSpeed = 5, _rotatingSpeed = 1;
    [SerializeField]
    private Animator _bodyAnim, _handsAnim;
    [SerializeField] 
    private InputAction _moveDirection;
    [SerializeField]
    private AudioClip _runningSound;

  
    private SoundManager _soundManager;
    private Vector2 _lastPosition, _velocity;


    private SlideMovement _sMove;

    private bool _canMove = true;

    private bool _isInDialogue;

    private void OnEnable()
    {
        _moveDirection.Enable();

        if (GameObject.Find("SoundManager") != null)
            _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        else
            Debug.Log("SoundManager not found");
    }

    private void OnDisable()
    {
        _moveDirection.Disable();
    }

    private void Start()
    {
        _sMove.startPosition = new Vector2(0,0);
        _sMove.maxIterations = 20;

        _lastPosition = transform.position;

        _soundManager.LoadSoundWithOutPath("walking", _runningSound);
    }

    private void Update()
    {
        if (!_canMove) return;
        if(_moveDirection.ReadValue<Vector2>() == Vector2.zero)
        {
            //Stop the moving animations
            _bodyAnim.StartPlayback();
            _handsAnim.StartPlayback();
            //Don't Move
            return;
        }
        else
        {
            //Play the animation
            _bodyAnim.StopPlayback();
            _handsAnim.StopPlayback();
            //Move the player
            _rb.Slide(_moveDirection.ReadValue<Vector2>(), _movingSpeed * Time.deltaTime, _sMove);
        }

        Vector2 direction = new Vector2(_moveDirection.ReadValue<Vector2>().x, _moveDirection.ReadValue<Vector2>().y);

        if (direction != Vector2.zero)
        {
            float currentAngle = transform.rotation.eulerAngles.z;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(currentAngle, targetAngle, _rotatingSpeed * Time.deltaTime);
            
            transform.rotation = Quaternion.Euler(0,0, angle);
        }

        //Small velocity calculation -- Nothing to worry about
        Vector2  currentPosition = transform.position;
        _velocity = (currentPosition - _lastPosition) / Time.deltaTime;
        _lastPosition = currentPosition;

        PlayerSound();
    }


    private void PlayerSound()
    {
        if (_velocity == Vector2.zero)
        {
            _soundManager.StopSound();
        }
        if (_velocity != Vector2.zero)
        {
           _soundManager.SetSFXVolume(1);
           _soundManager.PlaySound("walking");
        }
    }

    public void ChangeCanMove(Component sender, object obj)
    {
        bool? canMove = obj as bool?;

        if(sender is ConversationManager && !(bool)canMove)
        {
            _isInDialogue = true;
            _canMove = false;
        }
        else if(sender is ConversationManager && (bool)canMove)
        {
            _isInDialogue = false;
            _canMove = true;
        }

        if(!_isInDialogue)
            _canMove = (bool)canMove;

        if (!_canMove)
        {
            _bodyAnim.StartPlayback();
            _handsAnim.StartPlayback();
        }
    }
}
