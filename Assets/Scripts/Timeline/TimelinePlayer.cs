using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TimelinePlayer : MonoBehaviour
{
    //public AudioPlaybackController clock;
    //public LightShowController controller;
    //public List<LightEvent> events;

    //int index;

    //void Start()
    //{
    //    events.Sort((a, b) => a.time.CompareTo(b.time));
    //}

    //void Update()
    //{
    //    if (!clock.IsPlaying || index >= events.Count)
    //        return;

    //    while (index < events.Count && clock.Time >= events[index].time)
    //    {
    //        controller.PlayEvent(events[index]);
    //        index++;
    //    }
    //}

    //public void Restart()
    //{
    //    index = 0;
    //    clock.audioSource.time = 0f;
    //    clock.audioSource.Play();
    ////}
}
