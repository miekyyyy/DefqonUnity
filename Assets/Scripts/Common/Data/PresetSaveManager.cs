using SFB;
using UnityEngine;

namespace DefqonEngine.Common.Data
{
    public class PresetSaveManager : MonoBehaviour
    {
        public void SavePreset()
        {
            //File saving
            StandaloneFileBrowser.SaveFilePanel("Save Preset", "", "preset", "json");
        }

        public void LoadPresets()
        {
            //Files loading
            StandaloneFileBrowser.OpenFilePanel("Load Presets", "", "json", true);
        }

        public void LoadPreset()
        {
            //File Loading
            StandaloneFileBrowser.OpenFilePanel("Load Preset", "", "json", false);
        }

        public void DeletePreset()
        {

        }
    }
}
