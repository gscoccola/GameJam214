using UnityEngine;

public class HUD : Singleton<HUD>
{
    public void ExitGame()
    {
        LevelLoader.Instance.LoadLevel("MainMenu");
    }

    public void RestartLevel()
    {
        LevelLoader.Instance.ReloadLevel();
    }
}
