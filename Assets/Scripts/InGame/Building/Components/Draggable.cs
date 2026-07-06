using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [ReadOnly] public bool IsSelected;
    [ReadOnly] public bool IsSelectable = true;
    [ReadOnly] public bool Locked;

    private Camera _camera;
    private Rigidbody2D _rb;

    [HideInInspector] public UnityEvent OnSelected;
    [HideInInspector] public UnityEvent OnDeSelected;

    private Component _component;
    private BoxCollider2D _coll;
    private DraggableState _state = DraggableState.OnItemBar;
    private SpriteRenderer _spriteRenderer;

    #region Setup
    private void Awake()
    {
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _camera = FindAnyObjectByType<Camera>();
        _rb = GetComponent<Rigidbody2D>();
        _component = _rb.GetComponent<Component>();
        _coll = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        InventoryBar.Instance.AddDraggable(this);
        SetHidden(true);
    }

    private void Update()
    {
        if (!IsSelected) return;
        if (!IsSelectable) Deselect();
        Vector3 mouseWorldPos = GetMouseWorldPos();
        Vector3 newPos = LevelGrid.Instance.SnapToGrid(mouseWorldPos);
        if (newPos != transform.position) MoveToPos(newPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Deselect();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
       if (!IsSelectable || InGameManager.Instance.Mode != GameplayType.Construction) return;
       Select();
    }

    #endregion

    private Vector2 GetMouseWorldPos()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 newPos = new Vector3(Mathf.Clamp(mouseScreenPos.x, 0, Screen.width), Mathf.Clamp(mouseScreenPos.y, 0, Screen.height), 0f);
        return _camera.ScreenToWorldPoint(newPos);
    }

    private void MoveToPos(Vector3 pos)
    {
        _rb.MovePosition(pos);
        if (_coll.IsTouchingLayers(LayerMask.GetMask("InventoryBar"))) _state = DraggableState.OnItemBar;
        else if (LevelGrid.Instance.IsPosValid(_component, pos)) _state = DraggableState.OnValidPos;
        else
        {
            _state = DraggableState.OnInvalidPos;
            _spriteRenderer.color = new Color(1f, 0f, 0f, 0.7f);
            return;
        }
        _spriteRenderer.color = new Color(1f, 1f, 1f, 0.7f);
    }

    private void Deselect()
    {
        IsSelected = false;
        OnDeSelected?.Invoke();
        _spriteRenderer.color = Color.white;
        if (_state == DraggableState.OnValidPos) Activate();
        else if (_state == DraggableState.OnInvalidPos) _spriteRenderer.color = Color.red;
    }

    private void Select()
    {
        OnSelected?.Invoke();
        IsSelected = true;
        if (_state == DraggableState.OnItemBar) _spriteRenderer.color = new Color(1f, 1f, 1f, 0.7f);
        else if (_state == DraggableState.OnValidPos) _spriteRenderer.color = new Color(1f, 1f, 1f, 0.7f);
        else if (_state == DraggableState.OnInvalidPos) _spriteRenderer.color = new Color(1f, 0f, 0f, 0.7f);
        if (_component.IsActive) Deactivate();
    }

    private void Activate()
    {
        _component.Activate();
        ActiveDraggables.Instance.AddDraggable(this);
        InventoryBar.Instance.RemoveDraggable(this);
    }

    private void Deactivate()
    {
        _component.Deactivate();
        InventoryBar.Instance.AddDraggable(this);
        ActiveDraggables.Instance.RemoveDraggable(this);
    }

    public void SetHidden(bool hidden)
    {
        if (!hidden) _spriteRenderer.color = Color.white;
        _spriteRenderer.enabled = !hidden;
        _coll.enabled = !hidden;
        if (hidden) IsSelectable = false;
        else if (!Locked) IsSelectable = true;
    }

    public void Lock()
    {
        Locked = true;
        IsSelectable = false;
    }
}

public enum DraggableState
{
    OnItemBar,
    OnValidPos,
    OnInvalidPos,
}