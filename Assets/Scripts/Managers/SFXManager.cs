using UnityEngine;
using System.Collections.Generic;

public class SFXManager : PersistentSingleton<SFXManager>
{
    private List<AudioSource> _sfxSources = new();
    private Dictionary<string, ContinuousSFX> _continuousSFXs = new();

    [Header("UI")]
    public AudioClip UIClick;
    public AudioClip UIHover;
    public AudioClip MenuOpen;
    public AudioClip MenuClose;

    [SerializeField]
    public AudioClip[] humantalk;

    [SerializeField]
    public AudioClip[] robottalk;

    [SerializeField]
    public AudioClip steps;

    [SerializeField]
    public AudioClip jump;

    public AudioClip GetRandomTalk(AudioClip[] fromwhat)
    {
        int outof = fromwhat.Length;
        int i = outof*Random.Range(0, outof);
        return fromwhat[i];
    }

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < 10; i++)
        {
            GameObject sfxSource = new GameObject("SFXSource" + i);
            sfxSource.transform.SetParent(transform);
            sfxSource.AddComponent<AudioSource>();
            _sfxSources.Add(sfxSource.GetComponent<AudioSource>());
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in _sfxSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        Debug.LogWarning("All SFX sources are currently playing. Consider increasing the pool size.");
        return null;
    }

    public void PlaySFX(AudioClip[] clip, bool randomizePitch = false, float pitch = 1f)
    {
        if (clip == null || clip.Length == 0)
        {
            Debug.LogWarning($"SFX source {clip} is null.");
            return;
        }

        int index = Random.Range(0, clip.Length);

        PlaySFX(clip[index], randomizePitch, pitch);
    }

    public void PlaySFX(AudioClip clip, bool randomizePitch = false, float pitch = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning($"SFX source {clip.name} is null.");
            return;
        }

        AudioSource source = GetAvailableAudioSource();
        if (source == null)
        {
            Debug.LogWarning("No available audio source for continuous SFX.");
            return;
        }
        source.pitch = pitch;
        if (randomizePitch) source.pitch = pitch * Random.Range(0.9f, 1.1f);
        source.clip = clip;
        source.Play();
    }

    public void PlayContinuousSFX(AudioClip clip, string Id)
    {
        if (clip == null)
        {
            Debug.LogWarning($"SFX source {clip.name} is null.");
            return;
        }

        AudioSource source = GetAvailableAudioSource();
        if (source == null)
        {
            Debug.LogWarning("No available audio source for continuous SFX.");
            return;
        }

        source.clip = clip;
        source.loop = true;
        

        _continuousSFXs[Id] = new ContinuousSFX(source, Id);
        source.Play( );
    }
    public void StopContinuousSFX(string Id)
    {
        
        if (_continuousSFXs.TryGetValue(Id, out ContinuousSFX continuousSFX))
        {
            continuousSFX.Kill();
            _continuousSFXs.Remove(Id);
        }
        else
        {
            Debug.LogWarning($"No continuous SFX found with ID: {Id}");
        }
    }
}

public class ContinuousSFX
{
    public string Id;
    private AudioSource _source;
    public ContinuousSFX(AudioSource source, string id)
    {
        _source = source;
        Id = id;
    }
    public void Kill()
    {
        _source.loop = false;
        _source.Stop();
    }
}
