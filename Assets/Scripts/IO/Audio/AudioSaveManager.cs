using DefqonEngine.Sequencing.Audio;
using SFB;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace DefqonEngine.IO.Audio
{
    public class AudioSaveManager : MonoBehaviour
    {
        public void LoadAudio()
        {
            string path = LoadAudioDialog();
            LoadAudio(path);
        }

        public void LoadAudio(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            StartCoroutine(Load(path));
        }

        public IEnumerator Load(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("No audio clip found, returning null");
                yield break;
            }

            string url = new System.Uri(path).AbsoluteUri;

            AudioType audioType = GetAudioType(path);

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
            {
                var handler = (DownloadHandlerAudioClip)www.downloadHandler;
                handler.streamAudio = false;

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load audio: {www.error}");
                    yield break;
                }

                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                if (clip == null)
                {
                    Debug.LogError("Clip is null after loading.");
                    yield break;
                }

                AudioPlaybackController.Instance.SetAudioClip(clip);
            }
        }

        private AudioType GetAudioType(string path)
        {
            string extension = Path.GetExtension(path).ToLower();

            switch (extension)
            {
                case ".mp3": return AudioType.MPEG;
                case ".wav": return AudioType.WAV;
                case ".ogg": return AudioType.OGGVORBIS;
                case ".aif":
                case ".aiff": return AudioType.AIFF;
                default:
                    Debug.LogWarning("Unsupported format, using AudioType.UNKNOWN");
                    return AudioType.UNKNOWN;
            }
        }

        public string LoadAudioDialog()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Load Audio", "", new[] { new ExtensionFilter("Sound Files", "mp3", "wav", "ogg", "aif", "aiff") }, false);
            if (paths.Length == 0)
                return null;
            return paths[0];
        }
    }
}
