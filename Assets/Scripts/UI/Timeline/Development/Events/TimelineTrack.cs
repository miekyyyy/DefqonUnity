using DefqonEngine.Lighting.Data;
using DefqonEngine.Lighting.Groups;
using DefqonEngine.UI.Timeline.Common;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline.Development.Events
{
    public class TimelineTrack : MonoBehaviour, IPointerClickHandler
    {
        public LampGroup lampGroup;
        public int trackIndex;
        public List<LightEvent> events = new();

        [Header("UI")]
        public RectTransform eventsContainer;
        public TimelineEventView eventPrefab;

        public void AddEvent(LightEvent ev)
        {
            ev.trackIndex = trackIndex;
            events.Add(ev);

            if (eventPrefab != null && eventsContainer != null)
            {
                var view = Instantiate(eventPrefab, eventsContainer);
                view.Initialize(ev, this);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
                return;

            if (TimelineView.Instance == null)
                return;

            RectTransform panel = TimelineView.Instance.panel;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panel,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 local
            );

            float timeUnderMouse = TimelineView.Instance.XToTime(local.x);

            Debug.Log(
                $"[Timeline] Right-clicked on track '{lampGroup.groupName}' at {timeUnderMouse:F2}s"
            );
        }


#if false
        void UpdateRuler()
        {
            float startTime = TimelineView.Instance.scrollTime;
            float endTime = TimelineView.Instance.scrollTime + (TimelineView.Instance.Width / TimelineView.Instance.pixelsPerSecond);


            // Deactiveer oude ticks/labels
            foreach (var t in majorTicks) t.SetActive(false);
            foreach (var t in minorTicks) t.SetActive(false);
            foreach (var l in labels) l.gameObject.SetActive(false);

            int tickIndex = 0;

            for (float t = Mathf.Floor(startTime / tickIntervalSeconds) * tickIntervalSeconds;
                 t <= endTime; t += tickIntervalSeconds)
            {
                float x = TimelineView.Instance.TimeToX(t);


                // Major tick
                GameObject majorTick = GetOrCreate(majorTicks, majorTickPrefab, tickContainer, tickIndex);
                majorTick.SetActive(true);
                RectTransform r = majorTick.GetComponent<RectTransform>();
                r.anchoredPosition = new Vector2(x, 0);
                r.sizeDelta = new Vector2(2, majorTickHeight);

                // Label boven tick
                TMP_Text label = GetOrCreateLabel(labels, labelPrefab, tickContainer, tickIndex);
                label.gameObject.SetActive(true);
                RectTransform lr = label.GetComponent<RectTransform>();
                lr.anchoredPosition = new Vector2(x, majorTickHeight + 2);
                label.text = FormatTime(t);

                // Minor ticks
                for (int i = 1; i < minorTicksPerMajor; i++)
                {
                    float minorTime = t + (tickIntervalSeconds / minorTicksPerMajor) * i;
                    if (minorTime > endTime) break;
                    float minorX = TimelineView.Instance.TimeToX(minorTime);

                    GameObject minorTick = GetOrCreate(minorTicks, minorTickPrefab, tickContainer, tickIndex * minorTicksPerMajor + i);
                    minorTick.SetActive(true);
                    RectTransform mr = minorTick.GetComponent<RectTransform>();
                    mr.anchoredPosition = new Vector2(minorX, 0);
                    mr.sizeDelta = new Vector2(1, minorTickHeight);
                }

                tickIndex++;
            }
        }
#endif
        public string GroupName => lampGroup.groupName; // leesbare naam voor labels
    }
}
