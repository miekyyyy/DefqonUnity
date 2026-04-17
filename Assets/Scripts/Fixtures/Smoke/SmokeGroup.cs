using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.Lighting.Groups
{
    [System.Serializable]
    public class SmokeGroup
    {
        public int id;
        public string groupName;
        public List<int> smokeIds;
        public int priority = 0;

        public void Trigger(bool active)
        {
            foreach (int smokeId in smokeIds)
            {
                if (SmokeManager.Instance.smokeMachines.TryGetValue(smokeId, out var smokeMachine))
                    smokeMachine.Trigger(active);
            }
        }
    }
}
