using UnityEngine;

public class Builder : Singleton<Builder>
{
    public bool IsActive { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        IsActive = false;
    }

    private void Start()
    {
        InGameManager.Instance.OnBuildMode.AddListener(() => Activate());
        InGameManager.Instance.OnPlatformingMode.AddListener(() => Deactivate());
    }

    private void OnDestroy()
    {
        //GameInput.Actions.Construction
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

}
