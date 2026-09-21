using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorConsoleUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _doorUi;
    [SerializeField]
    private GameObject _cameraPivot;
    [SerializeField]
    private GameObject _ConePivot;

    [SerializeField]
    private GameObject _coneSprite;
    [SerializeField]
    private GameObject _cameraSprite;
    [SerializeField]
    private AnimationCurve _scaleCurve;

    private int _correctAngle;
    private float _correctScale;

    private bool _isMiniGameActive;
    private bool _isHoldingSlider;

    private GameObject _activeSlider;

    private int _topValue = 260, _bottomValue = -260;

    private float normelizedAngle;
    private float scaleSliderValue;
    private float normelizedScale;
    private float maxScale;
    private float scale;
    private float angle;

    [ContextMenu("Run My Function")]
    public void DoorBroken()
    {
        if (_isMiniGameActive) return;
        _isMiniGameActive = true;
        int coneAngle = GetRandomAngle();
        int cameraAngle = GetRandomAngle();

        while (coneAngle <= cameraAngle + 20 && coneAngle >= cameraAngle - 20)
        {
            coneAngle = GetRandomAngle();
        }

        _ConePivot.transform.localEulerAngles = new Vector3(0, 0, coneAngle);
        _cameraPivot.transform.localEulerAngles = new Vector3(0, 0, cameraAngle);
        _correctAngle = coneAngle;

        float cameraScale = GetRandomScale(cameraAngle);
        float coneScale = GetRandomScale(coneAngle);

        _cameraSprite.transform.localScale = new Vector3(_cameraSprite.transform.localScale.x, cameraScale, 1);
        _coneSprite.transform.localScale = new Vector3(_coneSprite.transform.localScale.x, coneScale, 1);
        _correctScale = coneScale;
    }

    public void SetActivatedSlider(Component sender, object obj)
    {
        if (_isHoldingSlider) return;
        _activeSlider = obj as GameObject;
    }

    private int GetRandomAngle()
    {
        int max = 90;
        int min = 0;

        return Random.Range(min, max + 1);
    }

    private float GetRandomScale(int angle)
    {
        float normelizedAngle = (float)angle / 90f;
        float normelizedScale = _scaleCurve.Evaluate(normelizedAngle);
        float maxScale = normelizedScale * 2;

        float scale = Random.Range(maxScale / 2f, maxScale);
        return scale;
    }

    private void MoveSlider(Vector3 newPos)
    {
        if (!_isMiniGameActive) return;
        _activeSlider.transform.localPosition = newPos;

        float yPos = newPos.y;
        yPos += 260f;
        float normedYPos = yPos / 520f;

        switch (_activeSlider.name)
        {
            case "Slider_A":
                angle = (int)(normedYPos * 90f);
                _cameraPivot.transform.localEulerAngles = new Vector3(0, 0, angle);
                SetScale(scaleSliderValue);
                break;

            case "Slider_S":
                scaleSliderValue = normedYPos;
                SetScale(scaleSliderValue);
                break;
        }
    }

    private void SetScale(float yPos)
    {
        normelizedAngle = (float)angle / 90f;
        normelizedScale = _scaleCurve.Evaluate(normelizedAngle);
        maxScale = normelizedScale * 2;
        scale = yPos * 2;
        scale = Mathf.Clamp(scale, 0.20f, maxScale);
        _cameraSprite.transform.localScale = new Vector3(_cameraSprite.transform.localScale.x, scale, 1);
    }

    private void CheckSolution()
    {
        if (_ConePivot.transform.localEulerAngles.z + 2 < _cameraPivot.transform.localEulerAngles.z &&
            _ConePivot.transform.localEulerAngles.z - 2 > _cameraPivot.transform.localEulerAngles.z) return;
        Debug.Log("within Angle");

        if (Mathf.Abs(_coneSprite.transform.localScale.y - _cameraSprite.transform.localScale.y) > 0.1f) return;
        Debug.Log("Fixed");
    }

    private void Update()
    {
        if (!_doorUi.activeSelf) return;

        if (Mouse.current.leftButton.isPressed)
        {
            if (_activeSlider == null) return;
            _isHoldingSlider = true;

            var sliderRect = (RectTransform)_activeSlider.transform;

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

            CheckSolution();

            _isHoldingSlider = false;
            _activeSlider = null;
        }
    }
}
