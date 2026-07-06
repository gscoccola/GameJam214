using UnityEngine;
using System.Collections.Generic;

public class InventoryBar : Singleton<InventoryBar>
{
    public List<Draggable> Items = new List<Draggable>();
    private SpriteRenderer _spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Deactivate();
    }


    public void Activate()
    {
        _spriteRenderer.enabled = true;
        ReorderItems();
        foreach (var item in Items)
        {
            item.SetHidden(false);
        }
    }

    public void Deactivate()
    {
        _spriteRenderer.enabled = false;
        foreach (var item in Items)
        {
            item.SetHidden(true);
        }
    }

    private void ReorderItems()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].transform.localPosition = new Vector3(0f, 4f - i * 2f, 0);
        }
    }

    public void AddDraggable(Draggable draggable)
    {
        Items.Add(draggable);
        draggable.transform.SetParent(transform);
    }

    public void RemoveDraggable(Draggable draggable)
    {
        Items.Remove(draggable);
    }
}
