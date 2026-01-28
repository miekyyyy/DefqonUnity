using System.IO;
using UnityEngine;
using DefqonEngine.Lighting.Data;
using System.Collections.Generic;
using DefqonEngine.UI.Timeline.Development.Events;

namespace DefqonEngine.UI.Timeline.Common
{
    [System.Serializable]
    public class TimelineSaveData
    {
        public List<LightEvent> events = new();
    }

    public class TimelineSaveManager : MonoBehaviour
    {

        string SavePath => Path.Combine(Application.persistentDataPath, "timeline.json");

        //public void Save()
        //{
        //    var data = new TimelineSaveData();
        //    foreach (var track in trackManager.Tracks)
        //        data.events.AddRange(track.events);

        //    File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        //    Debug.Log("Timeline saved");
        //}

        //public void Load()
        //{
        //    if (!File.Exists(SavePath))
        //    {
        //        Debug.LogWarning("No save file found");
        //        return;
        //    }

        //    string json = File.ReadAllText(SavePath);
        //    var data = JsonUtility.FromJson<TimelineSaveData>(json);

        //    trackManager.ClearAll();

        //    foreach (var ev in data.events)
        //    {
        //        var track = trackManager.Tracks[ev.trackIndex];
        //        track.AddEvent(ev);
        //    }

        //    Debug.Log("Timeline loaded");
        //}
    }
}
