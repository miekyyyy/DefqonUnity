using DefqonEngine.UI.Timeline.Waveform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveformAnalyzer : MonoBehaviour
{
    public struct TileData
    {
        public int startSample;
        public int endSample;
        public WaveformPoint[] points;
    }

    public List<TileData> GenerateTiles(
        AudioClip clip,
        float tileDurationSeconds,
        int samplesPerPoint)
    {
        List<TileData> result = new();

        int totalSamples = clip.samples;
        float sampleRate = clip.frequency;

        int sampleOffset = 0;

        while (sampleOffset < totalSamples)
        {
            float startTime = sampleOffset / sampleRate;

            float endTime = Mathf.Min(
                startTime + tileDurationSeconds,
                clip.length);

            int startSample =
                Mathf.RoundToInt(startTime * sampleRate);

            int endSample =
                Mathf.RoundToInt(endTime * sampleRate);

            int samplesToRead =
                endSample - startSample;

            float[] buffer =
                new float[samplesToRead * clip.channels];

            clip.GetData(buffer, startSample);

            int points =
                Mathf.CeilToInt(
                    (float)samplesToRead / samplesPerPoint);

            var waveform =
                GenerateWaveformPoints(
                    buffer,
                    samplesToRead,
                    clip.channels,
                    samplesPerPoint,
                    points);

            result.Add(new TileData
            {
                startSample = startSample,
                endSample = endSample,
                points = waveform
            });

            sampleOffset = endSample;
        }

        return result;
    }

    public IEnumerator GenerateTilesAsync(
    AudioClip clip,
    float tileDurationSeconds,
    int samplesPerPoint,
    System.Action<List<TileData>> onDone,
    System.Action<float> onProgress = null)
    {
        List<TileData> result = new();

        int totalSamples = clip.samples;
        int channels = clip.channels;
        float sampleRate = clip.frequency;

        int samplesPerTile = Mathf.FloorToInt(tileDurationSeconds * sampleRate);

        float[] buffer = new float[samplesPerTile * channels];

        int startSample = 0;

        while (startSample < totalSamples)
        {
            int endSample = Mathf.Min(startSample + samplesPerTile, totalSamples);
            int samplesToRead = endSample - startSample;

            clip.GetData(buffer, startSample);

            var waveform = GenerateWaveformPoints(
                buffer,
                samplesToRead,
                channels,
                samplesPerPoint,
                Mathf.CeilToInt((float)samplesToRead / samplesPerPoint)
            );

            result.Add(new TileData
            {
                startSample = startSample,
                endSample = endSample,
                points = waveform
            });

            startSample = endSample;

            // Report progress
            float progress = (float)startSample / totalSamples;
            onProgress?.Invoke(progress);

            yield return null;
        }

        onDone?.Invoke(result);
    }

    WaveformPoint[]
        GenerateWaveformPoints(
        float[] buffer,
        int samplesToRead,
        int channels,
        int samplesPerPoint,
        int points)
    {
        var data =
            new WaveformPoint[points];

        for (int i = 0; i < points; i++)
        {
            int s0 = i * samplesPerPoint;
            int s1 = Mathf.Min(
                s0 + samplesPerPoint,
                samplesToRead);

            float min = float.MaxValue;
            float max = float.MinValue;

            for (int s = s0; s < s1; s++)
            {
                float v = 0f;

                for (int c = 0; c < channels; c++)
                    v += buffer[s * channels + c];

                v /= channels;

                min = Mathf.Min(min, v);
                max = Mathf.Max(max, v);
            }

            data[i] = new WaveformPoint
            {
                min = min,
                max = max
            };
        }

        return data;
    }
}