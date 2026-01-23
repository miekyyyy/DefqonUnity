namespace DefqonEngine.Lighting.Data
{
    public enum SnapMode
    {
        None,
        Seconds,
        BPM
    }

    [System.Serializable]
    public class TimelineSettings
    {
        public SnapMode snapMode = SnapMode.None;

        public float secondSnapInterval = 0.25f;

        public float bpm = 120f;
        public float beatDivision = 1f;
    }
}
