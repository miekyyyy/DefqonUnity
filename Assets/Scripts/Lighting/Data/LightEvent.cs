namespace DefqonEngine.Lighting.Data
{
    [System.Serializable]
    public class LightEvent
    {
        public float time;
        public float duration;
        public UnityEngine.Color color;

        public int trackIndex;
        public TargetType targetType = TargetType.Lamp;
        public int targetId;
    }
    public enum TargetType
    {
        Lamp,
        Group
    }

}
