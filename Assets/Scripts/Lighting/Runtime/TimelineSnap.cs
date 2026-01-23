using DefqonEngine.Lighting.Data;
using UnityEngine;

namespace DefqonEngine.Lighting.Runtime
{
    public static class TimelineSnap
    {
        public static float SnapTime(float time, TimelineSettings settings)
        {
            switch (settings.snapMode)
            {
                case SnapMode.Seconds:
                    return Mathf.Round(time / settings.secondSnapInterval) * settings.secondSnapInterval;

                case SnapMode.BPM:
                    float secondsPerBeat = 60f / settings.bpm;
                    float snapInterval = secondsPerBeat * settings.beatDivision;
                    return Mathf.Round(time / snapInterval) * snapInterval;

                case SnapMode.None:
                default:
                    return time;
            }
        }
    }
}
