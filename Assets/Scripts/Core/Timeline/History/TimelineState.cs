using DefqonEngine.Sequencing.Data.Events;
using System;
using System.Collections.Generic;

namespace DefqonEngine.Core.Timeline.History
{
    [Serializable]
    public class TimelineState
    {
        public int trackCount;

        public List<TimelineEvent> events = new();

        public List<int> selectedEventIndexes = new();
    }
}
