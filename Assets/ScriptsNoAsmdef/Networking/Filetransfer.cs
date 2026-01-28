using Renci.SshNet;
using System.IO;
using UnityEngine;

public class Filetransfer : MonoBehaviour
{
    // File name only, stored in Unity's persistent data path
    private string fileName = "myData.json";

    // Pi credentials
    private string host = "192.168.160.54";
    private string username = "stage";
    private string password = "";

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
            using (var sftp = new SftpClient(host, username, password))
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
