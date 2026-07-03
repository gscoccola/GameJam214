using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private GameObject _creditsPanel;

    private void Start()
    {
        _startPanel.SetActive(true);
        _creditsPanel.SetActive(false);
    }

    public void ToggleCredits()
    {
        _creditsPanel.SetActive(!_creditsPanel.activeSelf);
        _startPanel.SetActive(!_creditsPanel.activeSelf);
    }


    public void StartGame()
    {
        LevelLoader.Instance.LoadLevel("Level1");
    }
}
