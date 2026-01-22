using UnityEngine;
using System.Collections.Generic;

public class LampRegistry : MonoBehaviour
{
    public static LampRegistry Instance;

    Dictionary<string, EmissiveLamp> lamps = new();
    List<LampGroup> lampGroups = new();

    void Awake()
    {
        Instance = this;

        foreach (var lamp in Object.FindObjectsByType<EmissiveLamp>(FindObjectsSortMode.None))
        {
            lamps[lamp.lampId] = lamp;
        }
    }

    public EmissiveLamp Get(string id)
    {
        return lamps.TryGetValue(id, out var lamp) ? lamp : null;
    }
    public IEnumerable<EmissiveLamp> Get(IEnumerable<string> lampIds)
    {
        foreach (var lampId in lampIds)
        {
            if (lamps.TryGetValue(lampId, out var lamp))
            {
                yield return lamp;
            }
        }
    }

    public IEnumerable<EmissiveLamp> GetAllLamps()
    {
        return lamps.Values;
    }

    public IEnumerable<EmissiveLamp> GetLampGroup(string groupId)
    {
        foreach (var group in lampGroups)
        {
            if (group.groupId != groupId) continue;
            
            return Get(group.lampIds);
        }
        return new List<EmissiveLamp>();
    }
}
