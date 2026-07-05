using UnityEngine;
using System.Collections.Generic;

public class ActiveDraggables : Singleton<ActiveDraggables>
{
    public List<Draggable> Items = new List<Draggable>();

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
