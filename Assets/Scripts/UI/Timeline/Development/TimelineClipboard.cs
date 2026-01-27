using DefqonEngine.Lighting.Data;

namespace DefqonEngine.UI.Timeline.Development
{
    public static class TimelineClipboard
    {
        public static LightEvent copiedEvent;

        public static void Copy(LightEvent e)
        {
            copiedEvent = new LightEvent()
            {
                time = e.time,
                duration = e.duration,
                color = e.color,
                trackIndex = e.trackIndex,
                targetType = e.targetType,
                targetId = e.targetId
            };
        }

        public static LightEvent Paste()
        {
            if (copiedEvent == null) return null;
            return new LightEvent()
            {
                time = copiedEvent.time,
                duration = copiedEvent.duration,
                color = copiedEvent.color,
                trackIndex = copiedEvent.trackIndex,
                targetType = copiedEvent.targetType,
                targetId = copiedEvent.targetId
            };
        }
    }
}
