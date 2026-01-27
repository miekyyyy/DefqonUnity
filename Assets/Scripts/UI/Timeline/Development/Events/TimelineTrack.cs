using DefqonEngine.Lighting.Data;
using DefqonEngine.Lighting.Groups;
using DefqonEngine.UI.Timeline.Common;
using System.Collections.Generic;
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

            float startTime = TimelineView.Instance.scrollTime;
            float endTime = TimelineView.Instance.scrollTime + (TimelineView.Instance.Width / TimelineView.Instance.pixelsPerSecond);






            //Oude code
           

            float time = 0f;

            if (TimelineView.Instance != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    TimelineView.Instance.panel,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 local
                );
                time = TimelineView.Instance.XToTime(local.x);
            }

            LightEvent newEvent = new LightEvent
            {
                time = Mathf.Max(0f, time),
                duration = 1f,
                color = Color.white,
                trackIndex = trackIndex,
                targetType = TargetType.Group,
                targetId = lampGroup.id
            };

            AddEvent(newEvent);

            Debug.Log($"[Timeline] Added event to track '{lampGroup.groupName}' at {newEvent.time:F2}s");
        }
#if false

        public void Zoom(float factor, float mouseX)
        {
            if (pixelsPerSecond <= 0f) return;

            float timeUnderMouse = XToTime(mouseX);

            // Nieuwe pixelsPerSecond
            float newPPS = pixelsPerSecond * factor;

            // Clamp: niet verder uitzoomen dan de clip
            if (audioSource != null && audioSource.clip != null)
            {
                float minPPS = Width / audioSource.clip.length;
                newPPS = Mathf.Max(newPPS, minPPS);
            }

            pixelsPerSecond = Mathf.Clamp(newPPS, 20f, 600f); // optioneel max zoom in

            // Pas scrollTime aan zodat de tijd onder de muis blijft
            float newScrollTime = timeUnderMouse - (mouseX / pixelsPerSecond);
            SetScrollTime(newScrollTime);
        }


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
