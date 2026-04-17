using DefqonEngine.Lighting.Groups;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.Lighting.Runtime
{
    [System.Serializable]
    public class LampEntry
    {
        public int id;
        public LightFixture lamp;
    }

    [System.Serializable]
    public class LampGroupEntry
    {
        public int Id => group.id;
        public LampGroup group;
    }

    public class LightManager : MonoBehaviour
    {
        public static LightManager Instance;

        [Header("Assign all individual lamps here")]
        public List<LampEntry> lampList = new();

        [Header("Assign all groups here")]
        public List<LampGroupEntry> groupList = new();

        [HideInInspector]
        public Dictionary<int, LightFixture> lamps = new();

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
                    groups[entry.Id] = entry.group;
            }
        }
    }
}
