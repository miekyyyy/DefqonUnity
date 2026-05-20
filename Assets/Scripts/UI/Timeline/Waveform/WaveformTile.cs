using UnityEngine;
using UnityEngine.UI;
using static DefqonEngine.UI.Timeline.Waveform.WaveformDrawer;

namespace DefqonEngine.UI.Timeline.Waveform
{
    class WaveformTile
    {
        public Texture2D texture;
        public WaveformPoint[] data;
        public RawImage image;
        public int startSample;
    }
}
