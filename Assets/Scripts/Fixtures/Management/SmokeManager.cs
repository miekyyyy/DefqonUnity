using DefqonEngine.Lighting.Groups;
using System.Collections.Generic;
using UnityEngine;


namespace DefqonEngine
{
    public class SmokeManager : MonoBehaviour
    {
        public static SmokeManager Instance { get; private set; }
        [System.Serializable]
        public class SmokeMachineEntry
        {
            public int id;
            public SmokeFixture smokeMachine;
        }

        [System.Serializable]
        public class SmokeGroupEntry
        {
            public int Id => group.id;
            public SmokeGroup group;
        }

        [Header("Assign all individual smoke machines here")]
        public List<SmokeMachineEntry> smokeList = new();

        [Header("Assign all groups here")]
        public List<SmokeGroupEntry> groupList = new();

        [HideInInspector]
        public Dictionary<int, SmokeFixture> smokeMachines = new();

        [HideInInspector]
        public Dictionary<int, SmokeGroup> groups = new();


        void Awake()
        {
            Instance = this;

            // Fill smoke machine dictionary
            smokeMachines.Clear();
            foreach (var entry in smokeList)
            {
                if (entry.smokeMachine != null)
                    smokeMachines[entry.id] = entry.smokeMachine;
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
