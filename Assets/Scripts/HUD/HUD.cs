using UnityEngine;

public class HUD : MonoBehaviour
{
    public void ExitGame()
    {
        LevelLoader.Instance.LoadLevel("MainMenu");
    }
}
