using DefqonEngine.Core.Timeline.Audio;
using DefqonEngine.UI.Timeline.Common;
using DefqonEngine.UI.Timeline.Waveform;
using System.Collections.Generic;
using UnityEngine;

public class WaveformPositioner : MonoBehaviour
{
    public void PositionTiles(
        List<WaveformTile> tiles)
    {
        var clip =
            AudioPlaybackController
            .Instance
            .GetAudioClip();

        if (clip == null ||
            TimelineView.Instance == null)
            return;

        float sampleRate =
            clip.frequency;

        foreach (var tile in tiles)
        {
            float startTime =
                tile.startSample / sampleRate;

            float endTime =
                tile.endSample / sampleRate;

            float xStart =
                TimelineView
                .Instance
                .TimeToX(startTime);

            float xEnd =
                TimelineView
                .Instance
                .TimeToX(endTime);

            var rt =
                tile.image.rectTransform;

            rt.anchoredPosition =
                new Vector2(xStart, 0);

            rt.sizeDelta =
                new Vector2(
                    xEnd - xStart,
                    rt.sizeDelta.y);
        }
    }

    public void UpdateContainerSize(
        Transform transform)
    {
        var clip =
            AudioPlaybackController
            .Instance
            .GetAudioClip();

        if (clip == null)
            return;

        float width =
            TimelineView
            .Instance
            .TimeToX(clip.length);

        RectTransform rt =
            (RectTransform)transform;

        rt.sizeDelta =
            new Vector2(
                width,
                rt.sizeDelta.y);
    }
}