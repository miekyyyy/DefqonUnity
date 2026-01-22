using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlaybackController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public float Time => audioSource.time;
    public bool IsPlaying => audioSource.isPlaying;
    public float Volume => audioSource.volume;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }


    public void Play()
    {
        audioSource.Play();
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void SetTime(float time)
    {
        audioSource.time = time;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void SetClip(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    public void SetLoop(bool loop)
    {
        audioSource.loop = loop;
    }
}
