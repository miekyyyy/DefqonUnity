using DefqonEngine.Stage.Fixtures.Management;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.Stage.Fixtures.Light
{
    [System.Serializable]
    public class LampGroup
    {
        public int id;
        public string groupName;
        public List<int> lampIds;
        public int priority = 0;

        public void SetColor(Color color)
        {
            foreach (int lampId in lampIds)
            {
                if (LightManager.Instance.lamps.TryGetValue(lampId, out var lamp))
                    lamp.SetColor(color);
            }
        }
    }
}
