using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Waveform
{
    public class WaveformTile
    {
        public Texture2D texture;
        public WaveformPoint[] data;
        public RawImage image;
        public int startSample;
        public int endSample;
    }
}
