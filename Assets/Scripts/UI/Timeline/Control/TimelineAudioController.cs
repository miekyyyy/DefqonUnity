using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Control
{
    public class TimelineAudioController : MonoBehaviour
    {
        public static TimelineAudioController Instance { get; private set; }
        public AudioSource audioSource;
        public Button playButton;
        public Button pauseButton;
        public Button stopButton;

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
    }
}
