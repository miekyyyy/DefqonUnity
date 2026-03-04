using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Control
{
    public class TimelineAudioController : MonoBehaviour
    {
        public static TimelineAudioController Instance { get; private set; }
        [SerializeField] AudioSource audioSource;
        [SerializeField] Button playButton;
        [SerializeField] Button pauseButton;
        [SerializeField] Button stopButton;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (playButton != null) playButton.onClick.AddListener(Play);
            if (pauseButton != null) pauseButton.onClick.AddListener(Pause);
            if (stopButton != null) stopButton.onClick.AddListener(Stop);
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
    }
}
