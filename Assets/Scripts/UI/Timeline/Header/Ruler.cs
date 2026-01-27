using DefqonEngine.UI.Timeline.Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DefqonEngine.UI.Timeline.Header
{
    public class Ruler : MonoBehaviour
    {
        [Header("References")]
        public RectTransform tickContainer;
        public GameObject majorTickPrefab;
        public GameObject minorTickPrefab;
        public GameObject labelPrefab;

        [Header("Settings")]
        public float majorTickHeight = 60f;
        public float minorTickHeight = 30f;
        public float tickIntervalSeconds = 1f; // Major tick
        public int minorTicksPerMajor = 4;      // subdivide

        private List<GameObject> majorTicks = new();
        private List<GameObject> minorTicks = new();
        private List<TMP_Text> labels = new();
        IEnumerator Start()
        {
            yield return null; // wacht 1 frame
            Refresh();
        }

        void Awake()
        {
            TimelineView.Instance.OnViewChanged += Refresh;
        }

        void Refresh()
        {
            if (TimelineView.Instance == null) return;
            UpdateRuler();
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

        GameObject GetOrCreate(List<GameObject> list, GameObject prefab, RectTransform parent, int index)
        {
            if (index < list.Count) return list[index];
            GameObject obj = Instantiate(prefab, parent);
            list.Add(obj);
            return obj;
        }

        TMP_Text GetOrCreateLabel(List<TMP_Text> list, GameObject prefab, RectTransform parent, int index)
        {
            if (index < list.Count) return list[index];
            GameObject obj = Instantiate(prefab, parent);
            TMP_Text text = obj.GetComponent<TMP_Text>();
            if (text == null) Debug.LogError("Label prefab heeft geen TMP_Text!");
            list.Add(text);
            return text;
        }

        string FormatTime(float time)
        {
            int min = Mathf.FloorToInt(time / 60f);
            int sec = Mathf.FloorToInt(time % 60f);
            return $"{min:00}:{sec:00}";
        }
    }
}
