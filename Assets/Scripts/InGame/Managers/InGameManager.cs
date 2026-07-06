using UnityEngine;
using UnityEngine.Events;

public class InGameManager : Singleton<InGameManager>
{
    public bool IsPaused { get; private set; }
    public GameplayType Mode { get; private set; }

    public UnityEvent OnBuildMode = new UnityEvent();
    public UnityEvent OnPlatformingMode = new UnityEvent();

    protected override void Awake()
    {
        base.Awake();
        IsPaused = false;
        Mode = GameplayType.Platforming;
    }

    private void Start()
    {
        GameInput.Actions.Game.Reset.performed += ctx => RestartLevel();
        GameInput.Actions.Game.Pause.performed += ctx => TogglePause();
        GameInput.Actions.Game.ToggleMode.performed += ctx => ToggleMode();

        LevelLoader.Instance.OnAreaChanged.AddListener(ToggleConstruction);
    }

    private void OnDestroy()
    {
        GameInput.Actions.Game.Reset.performed -= ctx => RestartLevel();
        GameInput.Actions.Game.Pause.performed -= ctx => TogglePause();
        GameInput.Actions.Game.ToggleMode.performed -= ctx => ToggleMode();

        LevelLoader.Instance.OnAreaChanged.RemoveListener(ToggleConstruction);
    }


    public void ExitGame()
    {
        LevelLoader.Instance.LoadLevel("MainMenu");
    }

    public void RestartLevel()
    {
        LevelLoader.Instance.ReloadLevel();
    }

    public void TogglePause()
    {
        /*InGameCanvas.Instance.OnTogglePause(!IsPaused);
        Time.timeScale = IsPaused ? 1f : 0f;

        IsPaused = !IsPaused;*/
    }

    public void ToggleMode()
    {
        if (Mode == GameplayType.Construction)
        {
            ToggleConstruction(null, 0, 0);
        }
        else
        {
            //Time.timeScale = 0;
            Mode = GameplayType.Construction;
            OnBuildMode?.Invoke();
        }
    }

    private void ToggleConstruction(AreaInfo info, int spawnNumber, int areaNumber)
    {
        Mode = GameplayType.Platforming;
        OnPlatformingMode?.Invoke();
    }
}


public enum GameplayType
{
    Platforming,
    Construction,
}