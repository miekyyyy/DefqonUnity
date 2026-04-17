using DefqonEngine.UI.Timeline.Common;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Header
{
    [RequireComponent(typeof(RawImage))]
    public class WaveformDrawer : MonoBehaviour
    {
        [Header("References")]
        public AudioSource audioSource;

        [Header("Settings")]
        public int samplesPerPoint = 256;
        public Color waveformColor = Color.cyan;
        public Color backgroundColor = Color.black;
        public int lineWidth = 2;

        struct WaveformPoint
        {
            public float min;
            public float max;
        }

        private WaveformPoint[] waveform;
        private Texture2D texture;
        private RawImage rawImage;

        void Awake()
        {
            rawImage = GetComponent<RawImage>();
            rawImage.rectTransform.anchorMin = Vector2.zero;
            rawImage.rectTransform.anchorMax = Vector2.zero;
            rawImage.rectTransform.pivot = Vector2.zero;
        }

        void Start()
        {
            if (!audioSource || !audioSource.clip || !TimelineView.Instance) return;

            BuildWaveformData();
            CreateTexture();
            DrawWaveform();
        }

        void BuildWaveformData()
        {
            var clip = audioSource.clip;
            int totalSamples = clip.samples;
            int channels = clip.channels;

            float[] data = new float[totalSamples * channels];
            clip.GetData(data, 0);

            int points = Mathf.CeilToInt((float)totalSamples / samplesPerPoint);
            waveform = new WaveformPoint[points];

            for (int i = 0; i < points; i++)
            {
                int start = i * samplesPerPoint;
                int end = Mathf.Min(start + samplesPerPoint, totalSamples);

                float min = 0f;
                float max = 0f;

                for (int s = start; s < end; s++)
                {
                    float v = data[s * channels];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }

                waveform[i] = new WaveformPoint { min = min, max = max };
            }
        }

        void CreateTexture()
        {
            int height = Mathf.Max(1, Mathf.RoundToInt(((RectTransform)transform).rect.height));
            texture = new Texture2D(waveform.Length, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            rawImage.texture = texture;
        }

        void DrawWaveform()
        {
            int w = texture.width;
            int h = texture.height;

            Color32[] pixels = new Color32[w * h];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = backgroundColor;

            for (int x = 0; x < waveform.Length; x++)
            {
                var p = waveform[x];

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

        void LateUpdate()
        {
            if (!TimelineView.Instance || !audioSource || !audioSource.clip) return;

            float pixelsPerSecond = TimelineView.Instance.pixelsPerSecond;
            float secondsPerPoint = (float)samplesPerPoint / audioSource.clip.frequency;

            float widthPx = waveform.Length * secondsPerPoint * pixelsPerSecond;
            rawImage.rectTransform.sizeDelta = new Vector2(widthPx, rawImage.rectTransform.sizeDelta.y);

            float x = -TimelineView.Instance.scrollTime * pixelsPerSecond;
            rawImage.rectTransform.anchoredPosition = new Vector2(x, 0);
        }
    }
}
