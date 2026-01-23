using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline
{
    [RequireComponent(typeof(RawImage))]
    public class WaveformDrawer : MonoBehaviour
    {
        [Header("References")]
        public AudioSource audioSource;
        public TimelineView timeline;

        [Header("Settings")]
        public int textureHeight = 60;
        public Color waveformColor = Color.cyan;
        public Color backgroundColor = Color.black;
        public int lineWidth = 2; // aantal pixels breed voor de lijn

        private Texture2D waveformTex;
        private RawImage rawImage;

        private int lastTexWidth = 0;

        void Awake()
        {
            rawImage = GetComponent<RawImage>();
        }

        void Start()
        {
            if (audioSource == null || audioSource.clip == null || timeline == null) return;
            CreateTexture();
            DrawWaveform();
        }

        void LateUpdate()
        {
            if (audioSource == null || audioSource.clip == null || timeline == null || waveformTex == null) return;

            // Zorg dat de texture width matcht panel width
            int panelWidth = Mathf.Max(Mathf.CeilToInt(timeline.Width), 1);
            if (panelWidth != lastTexWidth)
            {
                waveformTex.Reinitialize(panelWidth, textureHeight);
                DrawWaveform();
                lastTexWidth = panelWidth;
            }

            // Scroll waveform met timeline
            float visibleTime = timeline.Width / timeline.pixelsPerSecond;
            float xOffset = timeline.scrollTime / audioSource.clip.length;
            rawImage.uvRect = new Rect(xOffset, 0, visibleTime / audioSource.clip.length, 1);
        }

        void CreateTexture()
        {
            int width = Mathf.Max(Mathf.CeilToInt(timeline.Width), 1);
            waveformTex = new Texture2D(width, textureHeight, TextureFormat.RGBA32, false);
            waveformTex.wrapMode = TextureWrapMode.Clamp;
            waveformTex.filterMode = FilterMode.Point; // voorkomt blur
            rawImage.texture = waveformTex;
            lastTexWidth = width;
        }

        void DrawWaveform()
        {
            var clip = audioSource.clip;
            int samples = clip.samples;
            int channels = clip.channels;
            float[] data = new float[samples * channels];
            clip.GetData(data, 0);

            int texWidth = waveformTex.width;

            // Clear texture
            Color32[] colors = new Color32[texWidth * textureHeight];
            for (int i = 0; i < colors.Length; i++) colors[i] = backgroundColor;
            waveformTex.SetPixels32(colors);

            // Bepaal hoeveel samples per pixel
            int packSize = Mathf.CeilToInt((float)samples / texWidth);

            for (int x = 0; x < texWidth; x++)
            {
                int start = x * packSize;
                int end = Mathf.Min(start + packSize, samples);

                float min = 1f;
                float max = -1f;

                for (int i = start; i < end; i += channels)
                {
                    float val = data[i];
                    if (val < min) min = val;
                    if (val > max) max = val;
                }

                int yMin = Mathf.RoundToInt((min + 1f) * 0.5f * textureHeight);
                int yMax = Mathf.RoundToInt((max + 1f) * 0.5f * textureHeight);

                
                for (int lx = 0; lx < lineWidth; lx++)
                {
                    int px = x + lx;
                    if (px >= texWidth) break;

                    for (int y = yMin; y <= yMax; y++)
                        waveformTex.SetPixel(px, y, waveformColor);
                }
            }

            waveformTex.Apply();
        }
    }
}
