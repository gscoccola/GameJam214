using UnityEngine;
using System.Collections;

public class MusicManager : PersistentSingleton<MusicManager>
{
    private AudioSource _audioSource;

    private AudioClip _playingClip;
    private AudioCustomLooper _audioCustomLooper;

    [SerializeField] private AudioClip[] _musicClips;
    [SerializeField] private float[] _loopingTimes;


    private void Start()
    {
        _audioCustomLooper = GetComponent<AudioCustomLooper>();
        _audioSource = GetComponent<AudioSource>();

        _audioSource.clip = _musicClips[0];
        _playingClip = _audioSource.clip;
        _audioSource.Play();
        //_audioCustomLooper.
    }

    public void SwitchMusic(AudioSource music)
    {
        /*if (music == null)
        {
            Debug.LogWarning($"Music source {music} is null.");
            return;
        }
            
        if (music.isPlaying) return;
        StartCoroutine(Switch(music));*/
    }

    /*private IEnumerator Switch(AudioSource music)
    {
        float startVolume = _playingSource.volume;
        while (_playingSource.volume > 0)
        {
            _playingSource.volume -= startVolume * Time.deltaTime / 1f; // Fade out over 1 second
            yield return null;
        }
        _playingSource.Stop();
        _playingSource.volume = startVolume; // Reset volume for next time

        _playingSource = music;
        music.Play();
    }*/
}
