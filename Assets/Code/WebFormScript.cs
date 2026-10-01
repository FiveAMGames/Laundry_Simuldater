using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class WebFormScript : MonoBehaviour
{
    public static WebFormScript instance;

    public bool sendStatistics = true;
    string newGuid;
    private string URL = "https://docs.google.com/forms/d/e/1FAIpQLSfg3A7meeqq7rGKmx7UJAvSou-qG1Tlvf4ggumsQmstVyb4rg/formResponse";


    //"https://docs.google.com/forms/d/e/1FAIpQLScoWyvhcx39nLHWHRaznUFvMnotTnOhDThhpAOzYbhc_s1RuA/formResponse";


    string fieldProjectName = "entry.1468878029";
    string fieldUserID = "entry.7396684";
    string fieldStoryLevel = "entry.1720837152";

    private bool isSubmittingStats = false;
    private bool allowQuit = false;

    private void Awake()
    {
        string id = PlayerPrefs.GetString("UserID", "");
        if (id == null || id=="") CreateGuid();
        else newGuid = id;
        PlayerPrefs.SetString("UserID", newGuid);
        instance = this;
    }

    private void OnEnable()
    {
        Application.wantsToQuit += WantsToQuit;
    }

    private void OnDisable()
    {
        Application.wantsToQuit -= WantsToQuit;
    }

    private bool WantsToQuit()
    {
        if (!sendStatistics || allowQuit)
            return true;

        if (!isSubmittingStats)
        {
            isSubmittingStats = true;
            StartCoroutine(SubmitStatsAndQuit());
        }

        // Cancel/delay this quit attempt.
        return false;
    }

    private IEnumerator SubmitStatsAndQuit()
    {
        yield return SubmitStats();

        allowQuit = true;
        Application.Quit();
    }

    void CreateGuid()
    {
        newGuid = Guid.NewGuid().ToString();
    }

    [ContextMenu("Send")]
    public void SetStats()
    {
        if (sendStatistics) StartCoroutine(SubmitStats());
    }

    IEnumerator SubmitStats()
    {
        newGuid = Guid.NewGuid().ToString();

        WWWForm form = new WWWForm();
        form.AddField(fieldProjectName, "CleanForTheMean");
        form.AddField(fieldUserID, newGuid);
        form.AddField(fieldStoryLevel, "NONE");

        UnityWebRequest WWW = UnityWebRequest.Post(URL, form);
        WWW.timeout = 5;
        yield return WWW.SendWebRequest();
    }
}
