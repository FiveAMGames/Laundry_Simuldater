using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using UnityEngine.Diagnostics;
using UnityEditor;
using System.IO;
using System.Reflection;

[ExecuteInEditMode]
public class CSVLoader : MonoBehaviour
{

    private int progress = 0;
    List<string> languages = new List<string>();
    public CSVReader CSVReader;


    bool onDialogueDownload = false;


    [ContextMenu("Load Text")]
    public void LoadDialogueTexts()
    {
        onDialogueDownload = true;
        Load();
    }
    [ContextMenu("Populate")]
    public void Populate() 
    {
        CSVReader.PopulateCharDialogs(CSVReader.char1);
        CSVReader.PopulateCharDialogs(CSVReader.char2);
    }

    [ContextMenu("Set Up Texts")]
    public void SetUpTexts()
    {
        CSVReader.SetUpText();
    }


    public void Load()
    {
        Debug.Log("Loading dialogue text...");
        StartCoroutine(CSVDownloader.DownloadData(AfterDownload));
    }

    public void AfterDownload(string data, string path)
    {
        if (null == data)
        {
            Debug.LogError("Was not able to download data or retrieve stale data.");
            // TODO: Display a notification that this is likely due to poor internet connectivity
            //       Maybe ask them about if they want to report a bug over this, though if there's no internet I guess they can't
        }
        else
        {
            Debug.Log("data is here! ");
            File.WriteAllText(Application.dataPath + "/Dialogs/Text.csv", data);

            FileInfo file = new FileInfo(Application.dataPath + "/Dialogs/Text.csv");
            AwaitFile(file);
        }
    }

    private void AfterProcessData(string errorMessage)
    {
        if (null != errorMessage)
        {
            Debug.LogError("Was not able to process data: " + errorMessage);
            // TODO: 

            
        }
        else
        {

        }
    }

    void AwaitFile( FileInfo file)
    {
        //While File is not accesable because of writing process
        while (IsFileLocked(file)) { Debug.Log("file is being written..."); }
        Debug.Log("file is available now");
        Debug.Log(file.Name);
     
        //File is available here
    }

    /// <summary>
    /// Code by ChrisW -> http://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
    /// </summary>
    protected virtual bool IsFileLocked(FileInfo file)
    {
        FileStream stream = null;

        try
        {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            return true;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }

        //file is not locked
        return false;
    }
}