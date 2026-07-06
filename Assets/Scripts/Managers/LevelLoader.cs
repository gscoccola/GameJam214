using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LevelLoader : PersistentSingleton<LevelLoader>
{
    private Image _fadeImage;
    [SerializeField] private float _fadeTime;

    public GameObject[] Areas;
    [SerializeField] private GameObject _currentArea;

    [HideInInspector] public UnityEvent<AreaInfo, int, int> OnAreaChanged;

    protected override void Awake()
    {
        base.Awake();
        _fadeImage = transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        _fadeImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        MusicManager.Instance.SwitchMusic(MusicManager.Instance.MusicMenu);
        if (_currentArea == null) _currentArea = Instantiate(Areas[0], Vector3.zero, Quaternion.identity);
    }

    public void ReloadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().name);
    }

    public void ReloadAreal()
    {
        LoadArea(Array.IndexOf(Areas, _currentArea), 0);
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadLevelCoroutine(sceneName));
    }

    public void LoadArea(int areaNumber, int spawnNumber)
    {
        StartCoroutine(LoadLevelCoroutine(SceneManager.GetActiveScene().name, true, areaNumber, spawnNumber));
    }

    private IEnumerator LoadLevelCoroutine(string sceneName, bool loadArea = false, int areaNumber = 0, int spawnNumber = 0)
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


        if (loadArea)
        {
            if (_currentArea != null)
            {
                LevelGrid.Instance = null;
                Destroy(_currentArea);
            }
            _currentArea = Instantiate(Areas[areaNumber], Vector3.zero, Quaternion.identity);
            OnAreaChanged?.Invoke(_currentArea.GetComponent<AreaInfo>(), spawnNumber, areaNumber);
        }
        else
        {
            DOTween.KillAll();
            SceneManager.LoadScene(sceneName);
        }
        

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