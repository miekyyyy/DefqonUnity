using DefqonEngine.Core.Timeline.Audio;
using DefqonEngine.UI.Timeline.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Waveform
{
    [RequireComponent(typeof(RawImage))]
    public class WaveformDrawer : MonoBehaviour
    {
        public static WaveformDrawer Instance { get; private set; }

        [Header("Settings")]
        public int TileWidth = 1024; // veilig voor WebGL
        int maxTextureSize;
        List<WaveformTile> tiles = new List<WaveformTile>();


        public int samplesPerPoint = 256;
        public Color waveformColor = Color.cyan;
        public Color backgroundColor = Color.black;
        public int lineWidth = 2;

        public struct WaveformPoint
        {
            public float min;
            public float max;
        }

        private WaveformPoint[] waveform;
        private Texture2D texture;
        private RawImage rawImage;

        void Awake()
        {maxTextureSize = SystemInfo.maxTextureSize;
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            maxTextureSize = SystemInfo.maxTextureSize;
        }

        void Start()
        {
            Refresh();
        }
        void LateUpdate()
        {
            PositionTiles();

            if (TimelineView.Instance != null)
                UpdateContainerSize();
        }

        public void Refresh()
        {
            if (!AudioPlaybackController.Instance.GetAudioClip() || !TimelineView.Instance)
                return;

            ClearTiles();
            BuildTiles();

            UpdateContainerSize();
        }

        void ClearTiles()
        {
            foreach (var tile in tiles)
            {
                if (tile.texture != null)
                    Destroy(tile.texture);

                if (tile.image != null)
                    Destroy(tile.image.gameObject);
            }

            tiles.Clear();
        }

        void BuildTiles()
        {
            var clip = AudioPlaybackController.Instance.GetAudioClip();

            int totalSamples = clip.samples;
            int channels = clip.channels;

            int samplesPerTile = samplesPerPoint * TileWidth;

            float[] buffer = new float[samplesPerTile * channels];

            int sampleOffset = 0;

            while (sampleOffset < totalSamples)
            {
                int samplesToRead = Mathf.Min(samplesPerTile, totalSamples - sampleOffset);
                Array.Clear(buffer, 0, buffer.Length);
                clip.GetData(buffer, sampleOffset);

                int points = Mathf.CeilToInt((float)samplesToRead / samplesPerPoint);
                WaveformPoint[] data = new WaveformPoint[points];

                for (int i = 0; i < points; i++)
                {
                    int start = i * samplesPerPoint;
                    int end = Mathf.Min(start + samplesPerPoint, samplesToRead);

                    float min = float.MaxValue;
                    float max = float.MinValue;

                    for (int s = start; s < end; s++)
                    {
                        float v = 0f;
                        for (int c = 0; c < channels; c++)
                        {
                            v += buffer[s * channels + c];
                        }
                        v /= channels;
                        if (v < min) min = v;
                        if (v > max) max = v;
                    }

                    data[i] = new WaveformPoint { min = min, max = max };
                }

                CreateTile(data, sampleOffset);

                sampleOffset += samplesToRead;
            }
        }

        void CreateTile(WaveformPoint[] data, int startSample)
        {
            int height = Mathf.Max(1, Mathf.RoundToInt(((RectTransform)transform).rect.height));

            Texture2D tex = new Texture2D(data.Length, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            DrawToTexture(tex, data);

            GameObject go = new GameObject("WaveTile", typeof(RawImage));
            go.transform.SetParent(transform, false);

            RawImage img = go.GetComponent<RawImage>();
            img.texture = tex;

            RectTransform rt = img.rectTransform;

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.pivot = Vector2.zero;

            
            rt.sizeDelta = new Vector2(100, height);
            //rt.sizeDelta = new Vector2(data.Length, ((RectTransform)transform).rect.height);
            //rt.anchoredPosition = new Vector2(startSample / (float)samplesPerPoint, 0);

            tiles.Add(new WaveformTile
            {
                texture = tex,
                data = data,
                image = img,
                startSample = startSample
            });

            //foreach (var tile in tiles)
            //{

            //    tile.image.rectTransform.anchorMin = Vector2.zero;
            //    tile.image.rectTransform.anchorMax = Vector2.zero;
            //    tile.image.rectTransform.pivot = Vector2.zero;
            //}
        }

        void DrawToTexture(Texture2D texture, WaveformPoint[] data)
        {
            int w = texture.width;
            int h = texture.height;

            Color32[] pixels = new Color32[w * h];

            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = backgroundColor;

            for (int x = 0; x < data.Length; x++)
            {
                var p = data[x];

                int yMin = Mathf.RoundToInt((p.min + 1f) * 0.5f * h);
                int yMax = Mathf.RoundToInt((p.max + 1f) * 0.5f * h);

                yMin = Mathf.Clamp(yMin, 0, h - 1);
                yMax = Mathf.Clamp(yMax, 0, h - 1);

                for (int y = yMin; y <= yMax; y++)
                    pixels[y * w + x] = waveformColor;
            }

            texture.SetPixels32(pixels);
            texture.Apply();
        }


        void PositionTiles()
        {
            if (!TimelineView.Instance || !AudioPlaybackController.Instance.GetAudioClip()) return;

            var clip = AudioPlaybackController.Instance.GetAudioClip();
            float pixelsPerSecond = TimelineView.Instance.pixelsPerSecond;
            float secondsPerSample = 1f / clip.frequency;
            float secondsPerPoint = samplesPerPoint * secondsPerSample;
            float scrollOffset = TimelineView.Instance.scrollTime * pixelsPerSecond;

            foreach (var tile in tiles)
            {
                float startTime = tile.startSample * secondsPerSample;


                var rt = tile.image.rectTransform;

                float x = TimelineView.Instance.TimeToX(startTime);
                rt.anchoredPosition = new Vector2(x, 0);

               
                float width = tile.data.Length * secondsPerPoint * pixelsPerSecond;
                width = Mathf.Ceil(width);

                rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
            }
        }

        void UpdateContainerSize()
        {
            var clip = AudioPlaybackController.Instance.GetAudioClip();

            float pixelsPerSecond = TimelineView.Instance.pixelsPerSecond;
            float width = clip.length * pixelsPerSecond;

            RectTransform rt = (RectTransform)transform;
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
        }
    }
}
