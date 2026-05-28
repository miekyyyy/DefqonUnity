using DefqonEngine.Core.Project;
using Renci.SshNet;
using System.IO;
using TMPro;
using UnityEngine;

namespace DefqonEngine.IO.Networking
{

    public class FileTransfer : MonoBehaviour
    {
        [Header("Pi Credentials")]
        [SerializeField] private string host = "192.168.160.54";
        [SerializeField] private string username = "stage";
        private string password = "";

        [Header("UI Elements")]
        [SerializeField] private TMP_InputField hostInput;
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TMP_Text statusText;

        [Header("Settings")]
        [SerializeField] private string remoteFileName = "project.dfqprj";
        [SerializeField] private string remoteDirectory = "/home/stage/defqon_ws/data/projects/";
        [SerializeField] private string musicRemoteDirectory = "/home/stage/defqon_ws/data/projects/audio/";

        public void SetInputFields()
        {
            hostInput.text = host;
            usernameInput.text = username;
            passwordInput.text = password;
        }
        public void SetHost(string hostStr) => host = hostStr;
        public void SetUsername(string usernameStr) => username = usernameStr;
        public void SetPassword(string passwordStr) => password = passwordStr;
        public void test()
        {
            Debug.Log(Application.persistentDataPath);
        }
        /*
        public void UploadFile()
        {

            // Combine Unity's persistent path with your file name
            
            string localFilePath = ProjectManager.Instance.SaveProjectWithReturn();
            string musicFilePath = ProjectManager.Instance.CurrentProject.audioFilePath;
            string remoteFilePath = Path.Combine(remoteDirectory, remoteFileName); // Destination on Pi with single file name
            string remoteMusicFilePath = Path.Combine(musicRemoteDirectory, Path.GetFileName(musicFilePath)); // Destination on Pi for music file
            //string remoteFilePath = Path.Combine(remoteDirectory, Path.GetFileName(localFilePath)); // Destination on Pi with multiple file names

            if (!File.Exists(localFilePath))
            {
                Debug.LogError($"Local file not found: {localFilePath}");
                return;
            }
    
            try
            {
                using (var sftp = new SftpClient(host, username, password))
                {
                    sftp.Connect();
                    using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                    {
                        sftp.UploadFile(fileStream, remoteFilePath);
                    }
                    using (var musicFileStream = new FileStream(musicFilePath, FileMode.Open))
                    {
                        sftp.UploadFile(musicFileStream, remoteMusicFilePath);
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
        */

        public void UploadFile()
        {
            string localFilePath = ProjectManager.Instance.SaveProjectWithReturn();
            string musicFilePath = ProjectManager.Instance.CurrentProject.audioFilePath;

            string remoteFilePath = $"{remoteDirectory}{remoteFileName}";
            string remoteMusicFilePath =
                $"{musicRemoteDirectory}{Path.GetFileName(musicFilePath)}";

            if (!File.Exists(localFilePath))
            {
                Debug.LogError($"Local project file not found: {localFilePath}");
                return;
            }

            try
            {
                using (var sftp = new SftpClient(host, username, password))
                {
                    Debug.Log("Connecting to Pi...");
                    sftp.Connect();

                    if (!sftp.IsConnected)
                    {
                        Debug.LogError("Failed to connect.");
                        return;
                    }

                    // Ensure folders exist
                    if (!sftp.Exists(remoteDirectory))
                        sftp.CreateDirectory(remoteDirectory);

                    if (!sftp.Exists(musicRemoteDirectory))
                        sftp.CreateDirectory(musicRemoteDirectory);

                    // Upload project file
                    using (var fileStream =
                           new FileStream(localFilePath, FileMode.Open))
                    {
                        sftp.UploadFile(fileStream, remoteFilePath, true);
                    }

                    Debug.Log("Project uploaded.");

                    // Upload music if exists
                    if (!string.IsNullOrEmpty(musicFilePath)
                        && File.Exists(musicFilePath))
                    {
                        using (var musicStream =
                               new FileStream(musicFilePath, FileMode.Open))
                        {
                            sftp.UploadFile(
                                musicStream,
                                remoteMusicFilePath,
                                true
                            );
                        }

                        Debug.Log("Music uploaded.");
                    }
                    else
                    {
                        Debug.LogWarning("No music file found.");
                    }

                    sftp.Disconnect();
                }

                Debug.Log("Upload completed successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Upload error: {ex}");
            }
        }

    }

}
