using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;

public class PowerRegulator : MonoBehaviour, IMiniGame
{
    [SerializeField]
    private GameObject _miniGameUI;
    [SerializeField]
    private GameObject FuseBoxTrigger;
    [SerializeField]
    private int _numberOfSliders;
    [SerializeField]
    List<GameObject> _sliderObject = new List<GameObject>();
    [SerializeField]
    List<GameObject> _desiredLocations = new List<GameObject>();
    [SerializeField]
    private GameEvent _completedMiniGame;
    [SerializeField]
    private GameEvent _failedMiniGame;
    [SerializeField]
    private GameEvent _disableTrigger;
    [SerializeField]
    private List<Image> _lights;
    [SerializeField]
    private Image _progressBar;
    [SerializeField]
    private float _progressSpeed;
    [SerializeField]
    private GameEvent _changeCanWalk;
    [SerializeField]
    private int _spaceBetween, _topValue, _bottomValue;


    [Header("Sound Variables")]
    [SerializeField]
    private AudioClip _clickSound;

    private SoundManager _soundManager;

    private List<int> _yStartPoints = new List<int>();
    private List<int> _yfinishPoints = new List<int>();

    private Vector2 _mousePos;

    private GameObject _activeSlider;

    private bool _isHoldingSlider;

    private List<GameObject> _completedSliders = new List<GameObject>();

    private float _timer;

    private bool _updateProgress;

    private bool _miniGameFinished;


    private void OnEnable()
    {
        if (GameObject.Find("SoundManager") != null)
            _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        else
            Debug.Log("SoundManager not found");
    }

    private void Start()
    {
        _soundManager.LoadSoundWithOutPath("click", _clickSound);
    }

    public void StartMiniGame(Component sender, object obj)
    {
        if (_updateProgress) return;
        _miniGameFinished = false;
        _updateProgress = true;
        foreach (Image image in _lights)
        {
            image.color = Color.white;
        }
        initializeMiniGame();
    }

    public void completed()
    {
        _miniGameFinished = true;
        _completedSliders.Clear();
        _isHoldingSlider = false;
        _activeSlider = null;
        _timer = 0;
        _completedMiniGame.Raise(this, new MiniGameFinishedEventArgs { FinishedMiniGame = MiniGame.PowerRegulating});
        _updateProgress = false;
        _yStartPoints.Clear();
        _yfinishPoints.Clear();
        foreach (Image image in _lights)
        {
            image.color = Color.green;
        }
        StartCoroutine(DissableUI());
    }

    public void failed()
    {
        FuseBoxTrigger.SetActive(true);
        _completedSliders.Clear();
        _isHoldingSlider = false;
        _activeSlider = null;
        _timer = 0;
        _failedMiniGame.Raise(this, EventArgs.Empty);
        _updateProgress = false;
        _yStartPoints.Clear();
        _yfinishPoints.Clear();
        _disableTrigger.Raise(this, EventArgs.Empty);
        foreach (Image image in _lights)
        {
            image.color = Color.red;
        }
        StartCoroutine(DissableUI());
    }

    private void initializeMiniGame()
    {
        for (int i = 0; i < _numberOfSliders; i++)
        {
            int startHeight = UnityEngine.Random.Range(1, 6);
            int desiredHeight = UnityEngine.Random.Range(1, 6);
            while(startHeight >= desiredHeight - 1 && startHeight <= desiredHeight + 1)
            {
                 startHeight = UnityEngine.Random.Range(1, 6);
            }

            int yStart = _bottomValue + ((startHeight - 1) * _spaceBetween);
            _yStartPoints.Add(yStart);

            int yFinish = _bottomValue + ((desiredHeight - 1) * _spaceBetween);
            _yfinishPoints.Add(yFinish);
        }

        for(int i = 0; i < _yStartPoints.Count; i++)
        {
            Vector3 newPos = _sliderObject[i].transform.localPosition;
            newPos.y = _yStartPoints[i];
            _sliderObject[i].transform.localPosition = newPos;
            newPos.y = _yfinishPoints[i];
            _desiredLocations[i].transform.localPosition = newPos;
        }

        _miniGameUI.SetActive(true);
        _changeCanWalk.Raise(this, false);
    }

    private void MoveRandomSlider(GameObject activeSlider)
    {
        int hitChance = UnityEngine.Random.Range(1, 3);
        if (hitChance != 2) return;
        int index = UnityEngine.Random.Range(0, 3);
        GameObject randomSlider = _sliderObject[index];

        while(randomSlider == activeSlider)
        {
            index = UnityEngine.Random.Range(0, 3);
            randomSlider = _sliderObject[index];
        }

        int direction = UnityEngine.Random.Range(1, 3);

        Vector3 newpos = randomSlider.transform.localPosition;
        if (direction == 1 && randomSlider.transform.localPosition.y > _bottomValue) newpos.y -= _spaceBetween;
        else if (direction == 2 && randomSlider.transform.localPosition.y < _topValue) newpos.y += _spaceBetween;

        randomSlider.transform.localPosition = newpos;

        if (_completedSliders.Contains(randomSlider))
        {
            _completedSliders.Remove(randomSlider);
        }

        //Click Sound Implementation
        _soundManager.SetSFXVolume(0.1f);
        _soundManager.PlaySound("click");
    }

    private void CheckSolution()
    {
        for(int i = 0; i < _sliderObject.Count; i++)
        {
            if (_sliderObject[i].transform.position.y == _desiredLocations[i].transform.position.y)
            {
                if (!_completedSliders.Contains(_sliderObject[i]))
                    _completedSliders.Add(_sliderObject[i]);
            }
        }

        if (_completedSliders.Count == 3) completed();
    }

    private void MoveSlider(Vector3 newPos)
    {
        if (_miniGameFinished) return;
        Debug.Log(newPos);
        _activeSlider.transform.localPosition = newPos;

        CheckSolution();
    }

    private void UpdateProgressBar(float progress)
    {
        if (progress >= 100)
        {
            failed();
            return;
        }
        float fillAmount = progress / 100;
        _progressBar.fillAmount = fillAmount;
    }

    private void Update()
    {
        if (!_miniGameUI.activeSelf) return;

        if (_updateProgress)
        {
            _timer += Time.deltaTime * _progressSpeed;

            UpdateProgressBar(_timer);
        }

        if (Mouse.current.leftButton.isPressed)
        {
            if (_activeSlider == null) return;
            _isHoldingSlider = true;

            var sliderRect = (RectTransform)_activeSlider.transform;

            // screen -> local (panel) space
            Vector2 localMouse;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _activeSlider.transform.parent.GetComponent<RectTransform>(), Mouse.current.position.ReadValue(), null, out localMouse); // null if Screen Space Overlay [web:34]

            Vector2 newPos = sliderRect.localPosition;
            newPos.y = Mathf.Clamp(localMouse.y, _bottomValue, _topValue);
            MoveSlider(newPos);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (_activeSlider == null) return;

            int index = _sliderObject.IndexOf(_activeSlider);

            // use local positions and snap distance in local units
            var sliderRect = (RectTransform)_activeSlider.transform;
            var targetRect = (RectTransform)_desiredLocations[index].transform;

            if (Mathf.Abs(sliderRect.localPosition.y - targetRect.localPosition.y) < 40)
            {
                Vector2 newPos = sliderRect.localPosition;
                newPos.y = targetRect.localPosition.y;
                MoveSlider(newPos);
                MoveRandomSlider(_activeSlider);
            }

            _isHoldingSlider = false;
            _activeSlider = null;
        }
    }

    public void SetActivatedSlider(Component sender, object obj)
    {
        if (_isHoldingSlider) return;
        _activeSlider = obj as GameObject;
    }

    private IEnumerator DissableUI()
    {
        yield return new WaitForSeconds(0.75f);
        _miniGameUI.SetActive(false);
        _changeCanWalk.Raise(this, true);
    }
}
