using UnityEngine;

public class InGameCanvas : Singleton<InGameCanvas>
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _buildOverlay;

    protected override void Awake()
    {
        base.Awake();
        _pausePanel.SetActive(false);
        _buildOverlay.SetActive(false);
    }

    private void Start()
    {
        /*InGameManager.Instance.OnBuildMode.AddListener(() => ToggleBuildOverlay(true));
        InGameManager.Instance.OnPlatformingMode.AddListener(() => ToggleBuildOverlay(false));*/
    }

    public void ExitGame()
    {
        InGameManager.Instance.ExitGame();
    }

    public void RestartLevel()
    {
        InGameManager.Instance.RestartLevel();
    }

    public void OnTogglePause(bool isPaused)
    {
        _pausePanel.SetActive(isPaused);
    }

    public void ToggleBuildOverlay(bool isActive)
    {
        _buildOverlay.SetActive(isActive);
    }
}
