using DefqonEngine.Core.Timeline.Audio;
using DefqonEngine.UI.Timeline.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
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
        public float tileDurationSeconds = 10f;

        public int samplesPerPoint = 256;
        public Color waveformColor = Color.cyan;
        public Color backgroundColor = Color.black;
        public int lineWidth = 2;

        public struct WaveformPoint
        {
            public float min;
            public float max;
        }

        void Awake()
        {
            maxTextureSize = SystemInfo.maxTextureSize;
            TileWidth = Mathf.Clamp(TileWidth, 1, maxTextureSize);
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            Refresh();
        }
        void Update()
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
            if (clip == null) return;

            int totalSamples = clip.samples;

            float clipLength = clip.length;
            float sampleRate = clip.frequency;

            float tileDuration = tileDurationSeconds;

            int sampleOffset = 0;

            while (sampleOffset < totalSamples)
            {
                float startTime = sampleOffset / sampleRate;
                float endTime = Mathf.Min(startTime + tileDuration, clipLength);

                int startSample = Mathf.RoundToInt(startTime * sampleRate);
                int endSample = Mathf.RoundToInt(endTime * sampleRate);

                int samplesToRead = endSample - startSample;

                float[] buffer = new float[samplesToRead * clip.channels];
                clip.GetData(buffer, startSample);

                int points = Mathf.CeilToInt((float)samplesToRead / samplesPerPoint);
                WaveformPoint[] data = new WaveformPoint[points];

                for (int i = 0; i < points; i++)
                {
                    int s0 = i * samplesPerPoint;
                    int s1 = Mathf.Min(s0 + samplesPerPoint, samplesToRead);

                    float min = float.MaxValue;
                    float max = float.MinValue;

                    for (int s = s0; s < s1; s++)
                    {
                        float v = 0f;

                        for (int c = 0; c < clip.channels; c++)
                            v += buffer[s * clip.channels + c];

                        v /= clip.channels;

                        min = Mathf.Min(min, v);
                        max = Mathf.Max(max, v);
                    }

                    data[i] = new WaveformPoint { min = min, max = max };
                }

                CreateTile(data, startSample);

                sampleOffset = endSample;
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

            
            rt.sizeDelta = new Vector2(0, height);
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

        Texture2D DrawWaveformTexture(WaveformPoint[] data)
        {
            if(data.Length == 0) return null;

            int texWidth = data.Length;
            int texHeight = Mathf.Max(1, Mathf.RoundToInt(((RectTransform)transform).rect.height));
            Texture2D tex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);

            for (int x = 0; x < texWidth; x++)
            {
                tex.SetPixel(x, y, color);
            }

            return tex;
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

                int center = h / 2;

                int yMin = center + Mathf.RoundToInt(p.min * center);
                int yMax = center + Mathf.RoundToInt(p.max * center);

                // Clamp to texture bounds
                yMin = Mathf.Clamp(yMin, 0, h - 1);
                yMax = Mathf.Clamp(yMax, 0, h - 1);

                // Ensure correct order
                if (yMin > yMax)
                {
                    int temp = yMin;
                    yMin = yMax;
                    yMax = temp;
                }

                for (int y = yMin; y <= yMax; y++)
                {
                    float t = Mathf.InverseLerp(yMin, yMax, y);
                    byte alpha = (byte)(150 + 105 * (1 - Mathf.Abs(t - 0.5f) * 2));

                    Color32 col = waveformColor;
                    col.a = alpha;

                    pixels[y * w + x] = col;
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
        }


        void PositionTiles()
        {
            if (!TimelineView.Instance || !AudioPlaybackController.Instance.GetAudioClip())
                return;

            var clip = AudioPlaybackController.Instance.GetAudioClip();
            float sampleRate = clip.frequency;

            foreach (var tile in tiles)
            {
                float startTime = tile.startSample / sampleRate;
                float endTime = startTime + tile.data.Length * samplesPerPoint / sampleRate;

                float xStart = TimelineView.Instance.TimeToX(startTime);
                float xEnd = TimelineView.Instance.TimeToX(endTime);

                var rt = tile.image.rectTransform;

                rt.anchoredPosition = new Vector2(xStart, 0);
                rt.sizeDelta = new Vector2(xEnd - xStart, rt.sizeDelta.y);
            }
        }

        void UpdateContainerSize()
        {
            var clip = AudioPlaybackController.Instance.GetAudioClip();
            if (clip == null) return;

            float width = TimelineView.Instance.TimeToX(clip.length);

            RectTransform rt = (RectTransform)transform;
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
        }
    }
}
