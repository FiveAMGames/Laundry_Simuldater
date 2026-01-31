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

    public GameObject ButtonNext;

    [System.Serializable]
    public class DialogResponse 
    {
        public GameObject ResponseGameObject;
        public TextMeshProUGUI ResponseText;
        public int points;
    }
    public List<DialogResponse> Responses;

    int dayPartIndex = 0;
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

        dayPartIndex = 0;
        variantIndex = 0;
        textPartIndex = 0;
        hasResponses = false;
        
        
        currentCharacter = character;
        currentDialog = character.AllDialogs[currentDay];
        int points = character.CharIndex == 0 ? char1Points : char2Points;

        for (int i = 0; i < currentDialog.DialogParts[dayPartIndex].VariantDialogs.Count; i++) 
        {
            if (currentDialog.DialogParts[dayPartIndex].VariantDialogs[i].MinPointsForDialog.x >= points &&
                currentDialog.DialogParts[dayPartIndex].VariantDialogs[i].MinPointsForDialog.y < points)
            {
                variantIndex = i;
                break;
            }
        }
        UpdateDialogue();
    }

    void SetUpResponses() 
    {
        ButtonNext.SetActive(false);
        for (int i = 0; i < currentDialog.DialogParts[dayPartIndex].VariantDialogs[variantIndex].DialogPart[textPartIndex].TextPartResponses.Count; i++) 
        {
            Responses[i].ResponseGameObject.SetActive(true);
            Responses[i].ResponseText.text = currentDialog.DialogParts[dayPartIndex].VariantDialogs[variantIndex].DialogPart[textPartIndex].TextPartResponses[i].text;
            Responses[i].points = currentDialog.DialogParts[dayPartIndex].VariantDialogs[variantIndex].DialogPart[textPartIndex].TextPartResponses[i].ResponcePoints;
        }
        
    }

    public void UpdateDialogue() 
    {
        ButtonNext.SetActive(true);
        foreach (DialogResponse dr in Responses)
        {
            dr.ResponseGameObject.SetActive(false);
            dr.points = 0;
        }
        Debug.Log(dayPartIndex + " " + variantIndex + " " + textPartIndex);
        if (currentDialog.DialogParts[dayPartIndex].VariantDialogs[variantIndex].DialogPart.Count > textPartIndex)
        {

            DialogText.text = currentDialog.DialogParts[dayPartIndex].VariantDialogs[variantIndex].DialogPart[textPartIndex].text;

            if (currentDialog.DialogParts[dayPartIndex].VariantDialogs[variantIndex].DialogPart[textPartIndex].TextPartResponses.Count > 0)
            {
                hasResponses = true;
                SetUpResponses();
            }
        }
        else 
        {
            textPartIndex = 0;
            dayPartIndex++;
            if (currentDialog.DialogParts.Count > dayPartIndex) 
            {
                int points = currentCharacter.CharIndex == 0 ? char1Points : char2Points;
                Debug.Log("points are " + points);
                for (int i = 0; i < currentDialog.DialogParts[dayPartIndex].VariantDialogs.Count; i++)
                {
                    
                    if (currentDialog.DialogParts[dayPartIndex].VariantDialogs[i].MinPointsForDialog.x <= points &&
                        currentDialog.DialogParts[dayPartIndex].VariantDialogs[i].MinPointsForDialog.y > points)
                    {
                        variantIndex = i;
                        Debug.Log(currentDialog.DialogParts[dayPartIndex].VariantDialogs[i].MinPointsForDialog);

                    }
                    
                }
                UpdateDialogue();
            }
            else 
            {
                Debug.Log("End of day dialog");           
            }
        }
    }


    public void DialogClickNext() 
    {
        if (!hasResponses) 
        {
            textPartIndex++;
            UpdateDialogue();
        }        
    }

    public void ClickOnResponse(int index) 
    {
        if (hasResponses) 
        {
            bool char1 = currentCharacter.CharIndex == 0;
            if (char1) char1Points += Responses[index].points;
            else char2Points += Responses[index].points;

            hasResponses = false;
            textPartIndex++;
            UpdateDialogue();
        }
    
    }
}
