using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool IsSelected;
    public bool IsSelectable => InGameManager.Instance.Mode == GameplayType.Construction;

    //[SerializeField] private float _force = 5f;

    private Camera _camera;
    private Rigidbody2D _rb;
    //private Vector2 _grabbedLocalPos;

    public UnityEvent OnSelected;
    public UnityEvent OnDeSelected;

    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        if (!IsSelected) return;
        if (!IsSelectable) Deselect();
        Vector3 mouseWorldPos = GetMouseWorldPos();
        /*Vector3 mouseRelativePos = (mouseWorldPos - transform.position);
        Vector3 forceVector = mouseRelativePos.normalized * Mathf.Min(mouseRelativePos.magnitude, 0.4f);
        Vector2 grabbedWorldPos = transform.TransformPoint(_grabbedLocalPos);*/
        _rb.position = LevelGrid.Instance.SnapToGrid(mouseWorldPos);

        /*_rb.AddForceAtPosition(_force * Time.fixedDeltaTime * forceVector.Flatten(), grabbedWorldPos,
        ForceMode2D.Impulse);*/
    }

    private void OnMouseAction(bool mouseDown)
    {
        IsSelected = mouseDown;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Deselect();
    }

    private void Deselect()
    {
        OnMouseAction(false);
        OnDeSelected?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsSelectable) return;
       // _grabbedLocalPos = transform.InverseTransformPoint(GetMouseWorldPos());
        OnSelected?.Invoke();
        OnMouseAction(true);
    }

    private Vector2 GetMouseWorldPos()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        // Clamp the new position to the bounds of the screen
        Vector3 newPos = new Vector3(Mathf.Clamp(mouseScreenPos.x, 0, Screen.width), Mathf.Clamp(mouseScreenPos.y, 0, Screen.height), 0f);
        return _camera.ScreenToWorldPoint(newPos);
    }
}