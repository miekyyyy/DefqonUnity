using System.Collections.Generic;
using UnityEngine;
using DefqonEngine.Lighting.Groups;
using DefqonEngine.Lighting.Data;
using DefqonEngine.Lighting.Runtime;

namespace DefqonEngine.Lighting.Runtime
{
    [System.Serializable]
    public class LampEntry
    {
        public int id;
        public EmissiveLamp lamp;
    }

    [System.Serializable]
    public class GroupEntry
    {
        public int id;
        public LampGroup group;
    }

    public class LightManager : MonoBehaviour
    {
        public static LightManager Instance;

        [Header("Assign all individual lamps here")]
        public List<LampEntry> lampList = new();

        [Header("Assign all groups here")]
        public List<GroupEntry> groupList = new();

        [HideInInspector]
        public Dictionary<int, EmissiveLamp> lamps = new();

        [HideInInspector]
        public Dictionary<int, LampGroup> groups = new();

        void Awake()
        {
            Instance = this;

            // Fill lamp dictionary
            lamps.Clear();
            foreach (var entry in lampList)
            {
                if (entry.lamp != null)
                    lamps[entry.id] = entry.lamp;
            }

            // Fill group dictionary
            groups.Clear();
            foreach (var entry in groupList)
            {
                if (entry.group != null)
                    groups[entry.id] = entry.group;
            }
        }

        public void ApplyEvent(LightEvent e)
        {
            switch (e.targetType)
            {
                case TargetType.Lamp:
                    if (lamps.TryGetValue(e.targetId, out var lamp))
                        lamp.SetColor(e.color);
                    break;

                case TargetType.Group:
                    if (groups.TryGetValue(e.targetId, out var group))
                        group.SetColor(e.color);
                    break;
            }
        }

        public void RegisterLamp(int id, EmissiveLamp lamp)
        {
            lamps[id] = lamp;
        }

        public void RegisterGroup(int id, LampGroup group)
        {
            groups[id] = group;
        }
    }
}
