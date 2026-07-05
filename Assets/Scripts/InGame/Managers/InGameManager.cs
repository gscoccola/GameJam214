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
    }

    private void OnDestroy()
    {
        GameInput.Actions.Game.Reset.performed -= ctx => RestartLevel();
        GameInput.Actions.Game.Pause.performed -= ctx => TogglePause();
        GameInput.Actions.Game.ToggleMode.performed -= ctx => ToggleMode();
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
            //Time.timeScale = 1.0f;
            Mode = GameplayType.Platforming;
            OnPlatformingMode?.Invoke();
        }
        else
        {
            //Time.timeScale = 0;
            Mode = GameplayType.Construction;
            OnBuildMode?.Invoke();
        }
    }
}


public enum GameplayType
{
    Platforming,
    Construction,
}