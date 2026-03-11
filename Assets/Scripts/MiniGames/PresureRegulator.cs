using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PresureRegulator : MonoBehaviour, IMiniGame, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private List<GameObject> _pipes = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _pipePrefabs = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _brokenPipePrefabs = new List<GameObject>();
    [SerializeField]
    private GameEvent _ValveRotationChanged;
    [SerializeField]
    private GameEvent _openPressureControlUI;
    [SerializeField]
    private GameObject _itemHolder;
    [SerializeField]
    private InputActionReference _closePanel;
    [SerializeField]
    private InputActionReference _leftClickAction;
    [SerializeField]
    private float _mouseSensitivity = 0.25f;
    [SerializeField]
    private bool _invertMouse = false;
    [SerializeField]
    private bool _allowRotateWithoutMiniGame = false; // for testing in editor
    [SerializeField]
    private GameEvent _miniGameFinished;
    
    [Header("AudioVariables")]
    [SerializeField]
    private AudioClip _grabSound;
    [SerializeField]
    private int _valveTurnSpeed = 40;
    [SerializeField]
    private GameObject _trashcanTrigger;
    [SerializeField]
    private List<GameObject> _pipeTriggers = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _PipeSpawnerTriggers = new List<GameObject>();
    [SerializeField]
    private GameEvent _playSteam;

    private SoundManager _soundManager;

    private GameObject _brokenPipe;
    private int _currentBrokenPipeIndex = -1;

    private GameObject _activeValve;
    private GameObject _heldItem;

    private float _valveProgress = 0;
    private bool _valveIsOpen = true;
    private bool _isCarryingPipe = false;
    private bool _itemPlaced = false;
    private bool _valveLocked = false;
    private bool _miniGameStarted = false;
    private bool _pipeRemoved = false;
    private bool _uiIsOpen = false;
    private bool _canOpenPanel = true;

    private int _previousPipe = -1;
    private bool _justOpenedUI;

    private void OnEnable()
    {
        if (GameObject.Find("SoundManager") != null)
            _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        else
            Debug.Log("SoundManager not found");
        if (_leftClickAction != null && _leftClickAction.action != null)
        {
            _leftClickAction.action.started += OnLeftClick;
            _leftClickAction.action.canceled += OnLeftRelease;
        }
    }

    private void OnDisable()
    {
        if (_leftClickAction != null && _leftClickAction.action != null)
        {
            _leftClickAction.action.started -= OnLeftClick;
            _leftClickAction.action.canceled -= OnLeftRelease;
        }
    }

    private Coroutine _holdCoroutine;
    private bool _leftPressed;
    private float _previousMouseAngle;
    private bool _hasPreviousMouseAngle;

    private void Start()
    {
        _soundManager.LoadSoundWithOutPath("grab", _grabSound);
    }

    private void SetRandomBrokenPipe()
    {
        int randomPipe = UnityEngine.Random.Range(0, _pipes.Count);

        while(randomPipe == _previousPipe)
        {
            randomPipe = UnityEngine.Random.Range(0, _pipes.Count);
        }

        _previousPipe = _currentBrokenPipeIndex;
        _currentBrokenPipeIndex = randomPipe;

        for (int i = 0; i < _brokenPipePrefabs.Count; i++)
        {
            if (_brokenPipePrefabs[i].tag != _pipes[randomPipe].tag) continue;
            GameObject brokenPipe = Instantiate(_brokenPipePrefabs[i], _pipes[randomPipe].transform.position, _pipes[randomPipe].transform.rotation);
            _brokenPipe = brokenPipe;
            _pipeTriggers[_currentBrokenPipeIndex].SetActive(true);
            _PipeSpawnerTriggers[_currentBrokenPipeIndex].SetActive(true);
            _trashcanTrigger.SetActive(true);
        }

        Destroy(_pipes[randomPipe]);
        _playSteam.Raise(this, _brokenPipe.transform);
    }

    public void StartMiniGame(Component sender, object obj)
    {
        if (_miniGameStarted) return;
        _miniGameStarted = true;
        SetRandomBrokenPipe();
    }

    public void StartMiniGameTroughDialogue()
    {
        StartMiniGame(this, EventArgs.Empty);
    }

    public void RemovePipe(Component sender, object obj)
    {
        string pipeHolderTag = sender.gameObject.transform.parent.tag;
        if (_isCarryingPipe)
        {
            PlacePipe(pipeHolderTag, sender.transform.parent.gameObject);
            return;
        }

        if (pipeHolderTag != _brokenPipe.tag) return;
        if (_valveIsOpen) return;
        _pipeRemoved = true;
        _isCarryingPipe = true;
        _heldItem = _brokenPipe;
        _brokenPipe.transform.parent = _itemHolder.transform;
        _brokenPipe.transform.localPosition = Vector3.zero;
        _brokenPipe.transform.localEulerAngles = new Vector3(0, 0, 90);

        _soundManager.SetSFXVolume(1);
        _soundManager.PlaySound("grab");
    }

    private void PlacePipe(string tag, GameObject holder)
    {
        if (_heldItem.tag != tag) return;
        if (_brokenPipe != null) return;
        if (!_pipeRemoved) return;

        _heldItem.transform.parent = holder.transform;
        _heldItem.transform.localPosition = Vector3.zero;
        _heldItem.transform.localEulerAngles = new Vector3(0, 0, 0);

        _pipes[_currentBrokenPipeIndex] = _heldItem;
        _heldItem = null;
        _itemPlaced = true;

        _soundManager.StopSound();
    }

    public void GrabItem(Component sender, object obj)
    {
        if (_heldItem != null) return;
        string pipeHolderTag = sender.gameObject.transform.parent.tag;

        _soundManager.PlaySound("grab");

        for(int i = 0; i < _pipePrefabs.Count; i++)
        {
            if (_pipePrefabs[i].tag != pipeHolderTag) continue;
            GameObject go = Instantiate(_pipePrefabs[i]);
            _heldItem = go;
            go.transform.parent = _itemHolder.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        _isCarryingPipe = true;
    }

    public void TrashItem(Component sender, object obj)
    {
        if (sender.transform.parent.gameObject.transform.parent.gameObject != gameObject) return;
        if (_heldItem == null) return;

        _isCarryingPipe = false;
        Destroy(_heldItem.gameObject);
    }

    public void completed()
    {
        _miniGameStarted = false;
        _valveProgress = 0;
        _valveIsOpen = true;
        _isCarryingPipe = false;
        _itemPlaced = false;
        _valveLocked = false;
        _pipeRemoved = false;
        _heldItem = null;
        _activeValve = null;
        _brokenPipe = null;
        _pipeTriggers[_currentBrokenPipeIndex].SetActive(false);
        _PipeSpawnerTriggers[_currentBrokenPipeIndex].SetActive(false);
        _trashcanTrigger.SetActive(false);
        _currentBrokenPipeIndex = -1;
        _miniGameFinished.Raise(this, new MiniGameFinishedEventArgs{ FinishedMiniGame = MiniGame.PipeBroke});
    }

    public void failed()
    {
        _miniGameStarted = false;
        _valveProgress = 0;
        _valveIsOpen = true;
        _isCarryingPipe = false;
        _itemPlaced = false;
        _valveLocked = false;
        _pipeRemoved = false;
        _miniGameStarted = false;
        _heldItem = null;
        _activeValve = null;
        _brokenPipe = null;
        _pipeTriggers[_currentBrokenPipeIndex].SetActive(false);
        _PipeSpawnerTriggers[_currentBrokenPipeIndex].SetActive(false);
        _trashcanTrigger.SetActive(false);
        _currentBrokenPipeIndex = -1;
    }

    public void GetActiveValve(Component sender, object obj)
    {
        _activeValve = obj as GameObject;
    }

    public void OpenPressureControl(Component sender, object obj)
    {
        if (_heldItem != null) return;
        if (_uiIsOpen) return;
        if (!_canOpenPanel) return;
        _canOpenPanel = false;
        _uiIsOpen = true;
        _justOpenedUI = true;
        _openPressureControlUI.Raise(this, true);
        StartCoroutine(CanClosePanel());
        //StartMiniGame(sender, obj);
    }

    public void EnableValve(Component sender, object obj)
    {
        Vector3 rotation = sender.transform.eulerAngles;
        rotation.z = _valveProgress;
        sender.transform.eulerAngles = rotation;
        _activeValve = sender.gameObject;
        _valveLocked = false;
    }

    private void Update()
    {
        if (_closePanel.action.WasPressedThisFrame())
        {
            if (_uiIsOpen && !_justOpenedUI)
            {
                _openPressureControlUI.Raise(this, false);
                _uiIsOpen = false;
                StartCoroutine(CanOpenPanel());
            }
        }
    }

    private IEnumerator CanOpenPanel()
    {
        yield return new WaitForEndOfFrame();
        _canOpenPanel = true;
    }

    private IEnumerator CanClosePanel()
    {
        yield return new WaitForEndOfFrame();
        _justOpenedUI = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // support pointer down similarly to input action start
        _leftPressed = true;
        // if active valve not set via hover event, try to resolve it from the pointer event
        if (_activeValve == null && eventData != null)
        {
            GameObject source = eventData.pointerPress != null ? eventData.pointerPress : eventData.pointerEnter;
            if (source != null)
            {
                // try to find a parent with a color tag (Red/Green/Blue) or a ValveUI component
                Transform t = source.transform;
                ValveUI valveUI = source.GetComponent<ValveUI>();
                if (valveUI != null)
                {
                    // ValveUI raises parent as the valve object, so use parent
                    if (source.transform.parent != null)
                        _activeValve = source.transform.parent.gameObject;
                }
                else
                {
                    while (t != null)
                    {
                        if (t.gameObject.CompareTag("Red") || t.gameObject.CompareTag("Green") || t.gameObject.CompareTag("Blue"))
                        {
                            _activeValve = t.gameObject;
                            break;
                        }
                        t = t.parent;
                    }
                }
            }
        }
        // initialize previous angle so the first delta is zero
        float angle;
        if (TryGetMouseAngle(out angle))
        {
            _previousMouseAngle = angle;
            _hasPreviousMouseAngle = true;
        }
        if (_holdCoroutine == null)
            _holdCoroutine = StartCoroutine(HoldTurn());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _leftPressed = false;
        _hasPreviousMouseAngle = false;
        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
        }
        if (!_miniGameStarted && !_allowRotateWithoutMiniGame) return;
        _valveLocked = false;
    }

    private void OnLeftClick(InputAction.CallbackContext ctx)
    {
        if (!_miniGameStarted) return;
        // called when left mouse button (or bound control) is pressed (started)
        if (ctx.started)
        {
            _leftPressed = true;
            // initialize previous angle so the first delta is zero
            float angle;
            if (TryGetMouseAngle(out angle))
            {
                _previousMouseAngle = angle;
                _hasPreviousMouseAngle = true;
            }
            if (_holdCoroutine == null)
                _holdCoroutine = StartCoroutine(HoldTurn());
        }
    }

    private void OnLeftRelease(InputAction.CallbackContext ctx)
    {
        // called when left mouse button (or bound control) is released (canceled)
        if (ctx.canceled)
        {
            _leftPressed = false;
            _hasPreviousMouseAngle = false;
            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
                _holdCoroutine = null;
            }
            if (!_miniGameStarted) return;
            _valveLocked = false;
        }
    }

    private IEnumerator HoldTurn()
    {
        while (_leftPressed)
        {
            ProcessValveTurn();
            yield return null; // run next frame
        }
        _holdCoroutine = null;
    }

    private void ProcessValveTurn()
    {
        if (!_miniGameStarted) return;
        if (_activeValve == null) return;

        // compute angle of mouse relative to the active valve center and use angle delta
        float currentAngle;
        if (!TryGetMouseAngle(out currentAngle)) return;

        if (!_hasPreviousMouseAngle)
        {
            _previousMouseAngle = currentAngle;
            _hasPreviousMouseAngle = true;
            return;
        }

        float delta = Mathf.DeltaAngle(_previousMouseAngle, currentAngle);
        _previousMouseAngle = currentAngle;
        float move = delta * _mouseSensitivity * (_invertMouse ? -1f : 1f);

        switch (_currentBrokenPipeIndex)
        {
            case 0:
                if (_activeValve.tag != "Red") break;
                if (_valveLocked) break;
                if (!(_valveIsOpen || (!_valveIsOpen && _itemPlaced))) break;
                _valveProgress += move;
                _ValveRotationChanged.Raise(this, new ValveRotationChangedEventArgs { ValveRotation = _valveProgress, Valve = _activeValve });
                break;
            case 1:
                if (_activeValve.tag != "Green") break;
                if (_valveLocked) break;
                if (!(_valveIsOpen || (!_valveIsOpen && _itemPlaced))) break;
                _valveProgress += move;
                _ValveRotationChanged.Raise(this, new ValveRotationChangedEventArgs { ValveRotation = _valveProgress, Valve = _activeValve });
                break;
            case 2:
                if (_activeValve.tag != "Blue") break;
                if (_valveLocked) break;
                if (!(_valveIsOpen || (!_valveIsOpen && _itemPlaced))) break;
                _valveProgress += move;
                _ValveRotationChanged.Raise(this, new ValveRotationChangedEventArgs { ValveRotation = _valveProgress, Valve = _activeValve });
                break;
        }

        if (_valveProgress > 180 && _valveIsOpen)
        {
            _valveProgress = 180;
            _valveIsOpen = false;
            _valveLocked = true;
            _playSteam.Raise(this, _brokenPipe.transform);
        }
        else if(_valveProgress < - 50 && _valveIsOpen)
        {
            StartCoroutine(PopOffValve());
            _valveLocked = true;
            _valveProgress = 0;
        }

        if (_valveProgress < 0 && !_valveIsOpen)
        {
            _valveProgress = 0;
            _valveIsOpen = true;
            _valveLocked = true;

            completed();
        }
        else if (_valveProgress > 230 && !_valveIsOpen)
        {
            StartCoroutine(PopOffValve());
            _valveProgress = 180;
            _valveLocked = true;
        }
    }

    private bool TryGetMouseAngle(out float angle)
    {
        angle = 0f;
        var mouse = Mouse.current;
        if (mouse == null) return false;
        Vector2 mousePos = mouse.position.ReadValue();

        if (_activeValve == null) return false;

        // Compute the screen position of the valve center and use that to get a stable angle
        Vector2 centerScreen;
        var rt = _activeValve.GetComponent<RectTransform>();
        if (rt != null)
        {
            // For UI elements, convert the rect's center to screen space
            Canvas canvas = rt.GetComponentInParent<Canvas>();
            Camera cam = null;
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;

            // Transform the rect center to world space then to screen space — this is more stable than
            // relying on ScreenPointToLocalPointInRectangle for per-frame angle deltas.
            Vector3 worldPos = rt.TransformPoint(rt.rect.center);
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
            centerScreen = screenPoint;
        }
        else
        {
            // For world-space objects, use the main camera to get screen position of the object center
            Camera cam = Camera.main;
            if (cam == null) return false;
            Vector3 screenPoint3 = cam.WorldToScreenPoint(_activeValve.transform.position);
            centerScreen = new Vector2(screenPoint3.x, screenPoint3.y);
        }

        Vector2 dir = mousePos - centerScreen;
        if (dir.sqrMagnitude <= Mathf.Epsilon) return false;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return true;
    }

    private IEnumerator PopOffValve()
    {
        // handle a valve popping off: disable interaction, detach, animate or apply physics and destroy
        if (_activeValve == null) yield break;

        // clear reference so other systems stop updating this valve while it animates
        GameObject valve = _activeValve;
        _activeValve = null;

        // disable raycast/interaction if present
        var canvasGroup = valve.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        var col2d = valve.GetComponent<Collider2D>();
        if (col2d != null)
            col2d.enabled = false;

        var col = valve.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // detach from current parent but keep it under the root Canvas (so it remains visible)
        var parentCanvas = valve.GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            valve.transform.SetParent(parentCanvas.transform, true);
        }
        else
        {
            // fallback to detaching to scene root
            valve.transform.SetParent(null, true);
        }

        // If this is a UI element (RectTransform) animate it in an arch (up-right then down)
        var rt = valve.GetComponent<RectTransform>();
        if (rt != null)
        {
            float duration = 1.0f;
            Vector3 startWorld = rt.position;
            // end point slightly below start to simulate falling back down
            Vector3 endWorld = startWorld + new Vector3(Screen.width * 0.08f, -Screen.height * 0.05f, 0f);
            // control point to create an arch (up and right)
            Vector3 control = startWorld + new Vector3(Screen.width * 0.12f, Screen.height * 0.25f, 0f);

            CanvasGroup cg = valve.GetComponent<CanvasGroup>();
            if (cg == null) cg = valve.AddComponent<CanvasGroup>();

            // store original transform and parent to restore later
            Transform originalParent = valve.transform.parent;
            int originalSibling = valve.transform.GetSiblingIndex();
            Vector3 originalLocalPos = valve.transform.localPosition;
            Vector3 originalLocalEuler = valve.transform.localEulerAngles;

            // ensure any layout or other scripts stop affecting the element while animating
            var layout = valve.GetComponent<UnityEngine.UI.LayoutGroup>();
            if (layout != null) layout.enabled = false;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float u = Mathf.Clamp01(t / duration);
                // quadratic Bezier: B(t) = (1-t)^2 P0 + 2(1-t)t C + t^2 P2
                float it = 1f - u;
                Vector3 pos = it * it * startWorld + 2f * it * u * control + u * u * endWorld;
                rt.position = pos;
                cg.alpha = Mathf.Lerp(1f, 0f, u);
                valve.transform.Rotate(0f, 0f, 600f * Time.deltaTime);
                yield return null;
            }

            // keep valve invisible but present in canvas until UI closes, then restore
            cg.alpha = 0f;
            // keep it on top so it doesn't get occluded
            valve.transform.SetAsLastSibling();

            Destroy(valve);
            yield break;
        }
    }
}