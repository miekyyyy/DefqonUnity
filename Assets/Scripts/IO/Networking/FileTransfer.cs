using Renci.SshNet;
using System.IO;
using UnityEngine;

namespace DefqonEngine.ScriptsNoAsmdef.Networking{
    
    public class FileTransfer : MonoBehaviour
    {
        // File name only, stored in Unity's persistent data path
        private string fileName = "myData.json";
    
        // Pi credentials
        [field: SerializeField] public string Host { private get; set; } = "192.168.160.54";
        [field: SerializeField] public string Username { private get; set; } = "stage";
        [field: SerializeField] public string Password { private get; set; } = "";
    
        // Call this to send the file
        public void test()
        {
            Debug.Log(Application.persistentDataPath);
        }
        public void UploadFile()
        {
            // Combine Unity's persistent path with your file name
            string localFilePath = Path.Combine(Application.persistentDataPath, "Jsons/" + fileName);
            string remoteFilePath = "/home/stage/defqon_ws/" + fileName; // Destination on Pi
    
            if (!File.Exists(localFilePath))
            {
                Debug.LogError($"Local file not found: {localFilePath}");
                return;
            }
    
            try
            {
                using (var sftp = new SftpClient(Host, Username, Password))
                {
                    sftp.Connect();
                    using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                    {
                        sftp.UploadFile(fileStream, remoteFilePath);
                    }
                    sftp.Disconnect();
                }
    
                Debug.Log($"File uploaded successfully from {localFilePath} to {remoteFilePath}!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error uploading file: {ex.Message}");
            }
        }
    
    }
    
}
