using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : PersistentSingleton<LevelLoader>
{
    private Image _fadeImage;
    [SerializeField] private float _fadeTime;
    protected override void Awake()
    {
        base.Awake();
        _fadeImage = transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        _fadeImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        MusicManager.Instance.SwitchMusic(MusicManager.Instance.MusicMenu);
    }

    public void ReloadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadLevelCoroutine(sceneName));
    }

    private IEnumerator LoadLevelCoroutine(string sceneName)
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.color = new Color(0, 0, 0, 0);
        float fadeDuration = _fadeTime;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            _fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        DOTween.KillAll();
        SceneManager.LoadScene(sceneName);

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01((fadeDuration - elapsedTime) / fadeDuration);
            _fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        _fadeImage.gameObject.SetActive(false);
    }
}
