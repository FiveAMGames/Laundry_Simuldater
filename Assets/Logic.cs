using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Logic : MonoBehaviour
{
    public enum State { menu, dialogue, scratching, wasching, drying, finale}
    public State currentState;

    public int currentDay;
    int char1Points = 0;
    int char2Points = 0;

    [Header("Dialog stuff")]
    public GameObject DialogObject;
    public Image DialogPortrait;
    public TextMeshProUGUI DialogText;

    [System.Serializable]
    public class DialogResponse 
    {
        public GameObject ResponseGameObject;
        public TextMeshProUGUI ResponseText;
        public int points;
    }
    public List<DialogResponse> Responses;

    int dialogueIndex = 0;
    int variantIndex = 0;
    int textPartIndex = 0;

    bool hasResponses = false;

    public CharacterDialog.DayDialogs currentDialog;
    public CharacterDialog currentCharacter;


    [ContextMenu("Test")]
    public void Test() 
    {
        StartDialogue(currentCharacter);
    }


    public void StartDialogue(CharacterDialog character) 
    {
        currentState = State.dialogue;

        dialogueIndex = 0;
        variantIndex = 0;
        textPartIndex = 0;
        hasResponses = false;
        
        foreach (DialogResponse dr in Responses)
        {
            dr.ResponseGameObject.SetActive(false);
        }
        currentCharacter = character;
        currentDialog = character.AllDialogs[currentDay];
        int points = character.CharIndex == 0 ? char1Points : char2Points;

        for (int i = 0; i < currentDialog.AllDayDialogs.Count; i++) 
        {
            if (currentDialog.AllDayDialogs[i].MinPointsForDialog.x >= points && currentDialog.AllDayDialogs[i].MinPointsForDialog.y <= points)
            {
                variantIndex = i;
                break;
            }
        }


        DialogText.text = currentDialog.AllDayDialogs[dialogueIndex].DialogPart[variantIndex].text;

        if (currentDialog.AllDayDialogs[dialogueIndex].DialogPart[variantIndex].TextPartResponses.Count > 0) 
        {
            hasResponses = true;
            SetUpResponses();
        }

    }

    void SetUpResponses() 
    {
        for (int i = 0; i < currentDialog.AllDayDialogs[dialogueIndex].DialogPart[variantIndex].TextPartResponses.Count; i++) 
        {
            Responses[i].ResponseGameObject.SetActive(true);
            Responses[i].ResponseText.text = currentDialog.AllDayDialogs[dialogueIndex].DialogPart[variantIndex].TextPartResponses[i].text;
            Responses[i].points = currentDialog.AllDayDialogs[dialogueIndex].DialogPart[variantIndex].TextPartResponses[i].ResponcePoints;
        }
        
    }
}
