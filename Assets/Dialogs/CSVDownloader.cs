using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CSVDownloader : MonoBehaviour
{
    public static string urlToDownload = "1SoMOAZoocNsoVti5SPa9xLP_SPG04T0XNCmt4tA-FQ4";
    public static string path;

    internal static IEnumerator DownloadData(System.Action<string, string> onCompleted)
    {
        yield return new WaitForEndOfFrame();
        string urlToCSV ="https://docs.google.com/spreadsheets/d/" + urlToDownload + "/export?format=csv";
        string downloadData = null;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(urlToCSV))
        {
            Debug.Log("Starting Download...");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("...Download Error: " + webRequest.error);
            }
            else 
            {
                
                downloadData = webRequest.downloadHandler.text.Substring(0);
                Debug.Log("...Downloaded");
            }

        }
        onCompleted(downloadData, path);
    }

}
