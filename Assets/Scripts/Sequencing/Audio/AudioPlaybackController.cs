using DefqonEngine.UI.Timeline.Waveform;
using UnityEngine;

namespace DefqonEngine.Core.Timeline.Audio
{
    public class AudioPlaybackController : MonoBehaviour
    {
        public static AudioPlaybackController Instance { get; private set; }
        [SerializeField] AudioSource audioSource;

        private void Awake()
        {
            Instance = this;
        }


        public void TogglePlayPause()
        {
            if (audioSource == null || audioSource.clip == null) return;

            if (audioSource.isPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        public void Play()
        {
            if (audioSource == null || audioSource.clip == null) return;

            //  clamp tijd eerst
            audioSource.time = Mathf.Clamp(audioSource.time, 0f, audioSource.clip.length);

            audioSource.Play();
        }


        public void Pause()
        {
            if (audioSource == null) return;
            audioSource.Pause();
        }

        public void Stop()
        {
            if (audioSource == null) return;
            audioSource.Stop();
        }

        public void SetTime(float time)
        {
            if (audioSource == null || audioSource.clip == null) return;

            // Zorg dat tijd binnen clip boundaries blijft
            float clampedTime = Mathf.Clamp(time, 0f, audioSource.clip.length);

            // Forceer AudioSource initialisatie
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                audioSource.Pause();
            }

            audioSource.time = clampedTime;
        }

        public float GetCurrentTime()
        {
            if (audioSource == null) return 0f;
            return audioSource.time;
        }

        public bool IsPlaying()
        {
            return audioSource != null && audioSource.isPlaying;
        }

        public bool SetAudioClip(AudioClip clip)
        {
            if (audioSource == null) return false;
            audioSource.clip = clip;
            WaveformDrawer.Instance.Refresh();
            return true;
        }

        public AudioClip GetAudioClip()
        {
            if (audioSource == null) return null;
            return audioSource.clip;
        }
    }
}
