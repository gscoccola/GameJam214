using UnityEngine;

public class Component : MonoBehaviour
{
    [ReadOnly] public bool IsActive;
    [SerializeField] public bool LockOnActivate;

    public ComponentType Type;
    public TileType[] ValidTiles;
    private Draggable _draggable;

    private void Awake()
    {
        _draggable = GetComponent<Draggable>();
    }

    public void Activate()
    {
        IsActive = true;
        if (LockOnActivate)_draggable.Lock();
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}


public enum ComponentType
{
    Generic,
}