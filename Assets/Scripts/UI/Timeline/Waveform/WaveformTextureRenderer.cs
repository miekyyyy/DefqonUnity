using DefqonEngine.UI.Timeline.Waveform;
using UnityEngine;

namespace DefqonEngine.UI.Timeline.Waveform {
    
    public class WaveformTextureRenderer : MonoBehaviour
    {
        public Color waveformColor = Color.cyan;
        public Color backgroundColor = Color.black;
    
        public Texture2D GenerateTexture(
            WaveformPoint[] data,
            int height)
        {
            Texture2D texture =
                new Texture2D(
                    data.Length,
                    height,
                    TextureFormat.RGBA32,
                    false);
    
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
    
            Draw(texture, data);
    
            return texture;
        }
    
        void Draw(
            Texture2D texture,
            WaveformPoint[] data)
        {
            int w = texture.width;
            int h = texture.height;
    
            Color32[] pixels =
                new Color32[w * h];
    
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = backgroundColor;
    
            for (int x = 0; x < data.Length; x++)
            {
                var p = data[x];
    
                int center = h / 2;
    
                int yMin =
                    center +
                    Mathf.RoundToInt(
                        p.min * center);
    
                int yMax =
                    center +
                    Mathf.RoundToInt(
                        p.max * center);
    
                yMin = Mathf.Clamp(yMin, 0, h - 1);
                yMax = Mathf.Clamp(yMax, 0, h - 1);
    
                if (yMin > yMax)
                    (yMin, yMax) = (yMax, yMin);
    
                for (int y = yMin; y <= yMax; y++)
                {
                    pixels[y * w + x] =
                        waveformColor;
                }
            }
    
            texture.SetPixels32(pixels);
            texture.Apply();
        }
    }
}