using UnityEngine;

public class Component : MonoBehaviour
{
    [ReadOnly] public bool IsActive;
    [ReadOnly] private SpriteRenderer _spriteRenderer;
    public ComponentType Type;
    public TileType[] ValidTiles;

    private void Awake()
    {
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        IsActive = true;
        //_spriteRenderer.color = Color.white;
    }

    public void Deactivate()
    {
        IsActive = false;
        //_spriteRenderer.color = Color.red;
    }
}


public enum ComponentType
{
    Generic,
}