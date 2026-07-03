using UnityEngine;
using System.Collections;

public class MusicManager : PersistentSingleton<MusicManager>
{
    public AudioSource MusicMenu;
    public AudioSource MusicGame;
    private AudioSource _playingSource;

    private void Start()
    {
        _playingSource = MusicMenu;
        if (MusicMenu == null)
        {
            Debug.LogWarning($"Music source {MusicMenu} is null.");
            return;
        }
        MusicMenu.Play();
    }

    public void SwitchMusic(AudioSource music)
    {
        if (music == null)
        {
            Debug.LogWarning($"Music source {music} is null.");
            return;
        }
            
        if (music.isPlaying) return;
        StartCoroutine(Switch(music));
    }

    private IEnumerator Switch(AudioSource music)
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
    }
}
