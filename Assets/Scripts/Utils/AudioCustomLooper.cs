using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioCustomLooper : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Loop Settings (in Seconds)")]
    [SerializeField] private float loopStartSeconds;
    [SerializeField] private float loopEndSeconds;

    private int loopStartSamples;
    private int loopEndSamples;
    private int loopLengthSamples;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Turn OFF the default loop; our script handles the looping logic manually
        audioSource.loop = false;

        // Convert seconds to exact audio samples based on file frequency
        loopStartSamples = (int)(loopStartSeconds * audioSource.clip.frequency);
        loopEndSamples = (int)(loopEndSeconds * audioSource.clip.frequency);
        loopLengthSamples = loopEndSamples - loopStartSamples;

        audioSource.Play();
    }

    void Update()
    {
        // Check if the current playback sample has passed the loop end point
        if (audioSource.timeSamples >= loopEndSamples)
        {
            // Rewind by exactly the loop length to maintain seamless rhythm
            audioSource.timeSamples -= loopLengthSamples;
        }
    }
}