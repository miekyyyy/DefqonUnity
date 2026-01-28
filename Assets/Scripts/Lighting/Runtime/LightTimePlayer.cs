using DefqonEngine.Lighting.Data;
using System;
using TMPro;
using UnityEngine;

namespace DefqonEngine.Lighting.Runtime
{
    public class LightTimelinePlayer : MonoBehaviour
    {
        public AudioSource audioSource;
        [SerializeField] TextMeshProUGUI timeDisplay;

        void Update()
        {
            if (audioSource == null) return;

            float t = audioSource.time;

            TimeSpan time = TimeSpan.FromSeconds(t);
            timeDisplay.text = time.ToString("mm':'ss':'ff");

            
        }
    }
}
