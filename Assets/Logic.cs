using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Logic : MonoBehaviour
{
    public static Logic Instance;
    public enum State { menu, dialogue, scratching, wasching, drying, finale}
    public State currentState;

    public int currentDay;
    int char1Points = 0;
    int char2Points = 0;

    public GameObject Intro;

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

    public CharacterDialog char1;
    public CharacterDialog char2;

    [System.Serializable]
    public class Outfit 
    {
        public Sprite Sprite;
        public Sprite Stain1;
        public Sprite Stain1Bad;
        public Sprite Stain2;
        public Sprite Stain2Bad;
        public enum Vorwaeasche {acid, soap, salt }
        public Vorwaeasche vorwaescheType;

        public enum Temperatur {thirty, forty, sixty}
        public Temperatur temperatureType;

        public enum Waesche {farbe, sport, wolle, weiss}
        public Waesche waescheType;
    }

    public List<Outfit> char1Outfits;
    public List<Outfit> char2Outfits;

    [Header("Scratching")]
    public GameObject ScratchingGameObject;
    public SpriteRenderer OutfitImage;

    public SpriteRenderer Stain1Object;
    public SpriteRenderer Stain1BadWashingObject;

    public SpriteRenderer Stain2Object;
    public SpriteRenderer Stain2BadWashingObject;

    public GameObject ScratchMoreObject;
    bool vorwaescheselected = false;

    public RenderTextureColorCheck TextureCheckScript;

    [ContextMenu("Test")]
    public void Test() 
    {
        StartDialogue(currentCharacter);
    }

    private void Start()
    {
        Instance = this;
        currentState = State.menu;
        Intro.SetActive(true);
        DialogObject.SetActive(false);
        ScratchingGameObject.SetActive(false);
        
    }

    private void Update()
    {
        if (currentState == State.menu && Input.anyKeyDown) 
        {
            StartDialogue(char1);
        }
    }


    public void StartDialogue(CharacterDialog character) 
    {
        currentState = State.dialogue;

        Intro.SetActive(false);
        DialogObject.SetActive(true);

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
                OpenScratching();
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

    public void OpenScratching() 
    {
        currentState = State.scratching;
        DialogObject.SetActive(false);
        ScratchingGameObject.SetActive(true);
        vorwaescheselected = false;
        Stain1BadWashingObject.gameObject.SetActive(true);
        Stain2BadWashingObject.gameObject.SetActive(true);


        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];
        OutfitImage.sprite = o.Sprite;
        Stain1Object.sprite = o.Stain1;
        Stain2Object.gameObject.SetActive(false);

        Stain1BadWashingObject.sprite = o.Stain1Bad;
        if(o.Stain2 != null) 
        {
            Stain2Object.gameObject.SetActive(true);
            Stain2Object.sprite = o.Stain2;
            Stain2BadWashingObject.sprite = o.Stain2Bad;

        }
    }

    public void SelectVorwaesche(Outfit.Vorwaeasche type) 
    {
        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];       
        if (o.vorwaescheType == type) 
        {
            if (currentCharacter.CharIndex == 0) char1Points++;
            else char2Points++;

            Stain1BadWashingObject.gameObject.SetActive(false);
            Stain2BadWashingObject.gameObject.SetActive(false);
        }
        else 
        {
            if (currentCharacter.CharIndex == 0) char1Points--;
            else char2Points--;
        }
        vorwaescheselected = true;
    }

    public void ClickDoneVorwaesche() 
    {
        if (!vorwaescheselected) 
        {
            if (currentCharacter.CharIndex == 0) char1Points--;
            else char2Points--;
        }
        bool enough = TextureCheckScript.SamplePanorama();
        if (enough) 
        {
        
        }
        else ScratchMoreObject.SetActive(true);
    }

    public void CloseScratchMore() 
    {
        ScratchMoreObject.SetActive(false);
    }
}
