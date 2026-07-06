using UnityEngine;
using System.Collections.Generic;

public class ActiveDraggables : Singleton<ActiveDraggables>
{
    public List<Draggable> AreaItems;
    private Dictionary<int, List<Draggable>> _areaDraggables = new Dictionary<int, List<Draggable>>();


    private void Start()
    {
        for (int i = 0; i < LevelLoader.Instance.Areas.Length; i++)
        {
            _areaDraggables[i] = new List<Draggable>();
        }
        AreaItems = _areaDraggables[0];
        LevelLoader.Instance.OnAreaChanged.AddListener(OnAreaChange);
    }

    private void OnDisable()
    {
        LevelLoader.Instance.OnAreaChanged.RemoveListener(OnAreaChange);
    }

    private void OnAreaChange(AreaInfo info, int spawnNumber, int areaNumber)
    {
        foreach (var item in AreaItems) item.gameObject.SetActive(false);
        AreaItems = _areaDraggables[areaNumber];
        foreach (var item in AreaItems) item.gameObject.SetActive(true);
    }


    public void AddDraggable(Draggable draggable)
    {
        AreaItems.Add(draggable);
        draggable.transform.SetParent(transform);
    }

    public void RemoveDraggable(Draggable draggable)
    {
        AreaItems.Remove(draggable);
    }
}
