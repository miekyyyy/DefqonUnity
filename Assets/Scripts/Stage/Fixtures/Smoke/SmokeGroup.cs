using DefqonEngine.Stage.Fixtures.Management;
using System.Collections.Generic;

namespace DefqonEngine.Stage.Fixtures.Smoke
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
