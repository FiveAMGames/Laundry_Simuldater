using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Logic : MonoBehaviour
{
    public static Logic Instance;
    public enum State { menu, dialogue, scratching, wasching, abgabe, finale}
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
        public Vorwaeasche vorwaescheType1;
        public Vorwaeasche vorwaescheType2;

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
    bool vorwaesche1selected = false;
    bool vorwaesche2selected = false;
    bool badVorwaesche = false;

    public RenderTextureColorCheck TextureCheckScript;
    public Image TemprtureTag;
    public Image WolleSportTag;

    public Sprite Degree30;
    public Sprite Degree40;
    public Sprite Degree60;
    public Sprite Wolle;
    public Sprite Sport;


    [Header("Washing")]
    public GameObject WashingObject;
    public GameObject MashineOpenedDoor;
    public GameObject MashineClosedDoor;

    public GameObject ChooseMittelWashenObject;
    public GameObject ChooseTempretureWashenObject;

    public List<GameObject> TemperatureButtons;

    public GameObject WashingDone;

    [Header("Abgabe")]
    public GameObject AbgabeObject;
    public TextMeshProUGUI AbgabeText;
    public Image AbgabeCharacterImage;
    public Image AbgabeOutfitImage;
    public Image AbgabeStain1Image;
    public Image AbgabeStain2Image;

    bool badWaeschemittel;
    bool badTemperature;

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
        WashingObject.SetActive(false);
        AbgabeObject.SetActive(false);
        
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

            Sprite emotion = currentCharacter.CharPortraitDefault;
            DialogPortrait.sprite = currentCharacter.CharPortraitDefault;

            if (currentDialog.DialogParts[dayPartIndex].VariantDialogs[variantIndex].DialogPart[textPartIndex].animation == "sad") 
            {
                DialogPortrait.sprite = currentCharacter.CharPortraitSad;
            }
            if (currentDialog.DialogParts[dayPartIndex].VariantDialogs[variantIndex].DialogPart[textPartIndex].animation == "happy")
            {
                DialogPortrait.sprite = currentCharacter.CharPortraitHappy;
            }
            if (currentDialog.DialogParts[dayPartIndex].VariantDialogs[variantIndex].DialogPart[textPartIndex].animation == "flirty")
            {
                DialogPortrait.sprite = currentCharacter.CharPortraitFlirty;
            }
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
        vorwaesche1selected = false;
        vorwaesche2selected = false;
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

        TemprtureTag.sprite = Degree30;
        if (o.temperatureType == Outfit.Temperatur.forty) TemprtureTag.sprite = Degree40;
        if (o.temperatureType == Outfit.Temperatur.sixty) TemprtureTag.sprite = Degree60;

        WolleSportTag.gameObject.SetActive(true);
        if (o.waescheType == Outfit.Waesche.wolle) WolleSportTag.sprite = Wolle;
        else if (o.waescheType == Outfit.Waesche.sport) WolleSportTag.sprite = Sport;
        else if (o.waescheType == Outfit.Waesche.weiss || o.waescheType == Outfit.Waesche.farbe) WolleSportTag.gameObject.SetActive(false);

    }

    public void SelectVorwaesche(Outfit.Vorwaeasche type) 
    {
        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];       
        if (o.vorwaescheType1 == type) 
        {
            if (currentCharacter.CharIndex == 0) char1Points++;
            else char2Points++;

            Stain1BadWashingObject.gameObject.SetActive(false);
            Stain2BadWashingObject.gameObject.SetActive(false);
            vorwaesche1selected = true;
        }
        else if (o.vorwaescheType2 == type) 
        {
            if (currentCharacter.CharIndex == 0) char1Points++;
            else char2Points++;

            Stain1BadWashingObject.gameObject.SetActive(false);
            Stain2BadWashingObject.gameObject.SetActive(false);
            vorwaesche2selected = true;
        }
        else 
        {
            if (currentCharacter.CharIndex == 0) char1Points--;
            else char2Points--;
        }
        if (o.Stain2 == null) vorwaesche2selected = true;
    }

    public bool CheckIfAllVorwaescheAreSelected() 
    {
        return (vorwaesche1selected == true && vorwaesche2selected == true);
    }

    public void ClickDoneVorwaesche() 
    {
        if (!vorwaesche1selected || !vorwaesche2selected) 
        {
            if (currentCharacter.CharIndex == 0) char1Points--;
            else char2Points--;
            badVorwaesche = true;
        }
        bool enough = TextureCheckScript.SamplePanorama();
        if (enough) 
        {
            OpenWasching();
        }
        else ScratchMoreObject.SetActive(true);
    }

    public void CloseScratchMore() 
    {
        ScratchMoreObject.SetActive(false);        
    }

    public void OpenWasching() 
    {
        Debug.Log("open washing");
        currentState = State.wasching;
        ScratchingGameObject.SetActive(false);
        WashingObject.SetActive(true);

        ChooseMittelWashenObject.SetActive(true);
        ChooseTempretureWashenObject.SetActive(false);

        MashineClosedDoor.SetActive(false);
        MashineOpenedDoor.SetActive(true);

        foreach(GameObject go in TemperatureButtons) 
        {
            go.SetActive(false);
        }
    }
    public void ClickWaschMittel(Outfit.Waesche type) 
    {
        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];
        if (o.waescheType == type)
        {
            if (currentCharacter.CharIndex == 0) char1Points++;
            else char2Points++;
        }
        else
        {
            if (currentCharacter.CharIndex == 0) char1Points--;
            else char2Points--;
            badWaeschemittel = true;
        }

        MashineOpenedDoor.SetActive(false);
        MashineClosedDoor.SetActive(true);
        ChooseTempretureWashenObject.SetActive(true);
        ChooseMittelWashenObject.SetActive(false);
        foreach (GameObject go in TemperatureButtons)
        {
            go.SetActive(true);
        }
    }

    public void ClickTemperature(int i) 
    {
        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];

        Outfit.Temperatur type = Outfit.Temperatur.thirty;
        if (i == 40) type = Outfit.Temperatur.forty;
        if (i == 60) type = Outfit.Temperatur.sixty;

        if (o.temperatureType == type)
        {
            if (currentCharacter.CharIndex == 0) char1Points++;
            else char2Points++;
        }
        else
        {
            if (currentCharacter.CharIndex == 0) char1Points--;
            else char2Points--;
            badTemperature = true;
        }
        WashingDone.SetActive(true);
    }

    public void CloseWashingDone() 
    {
        WashingDone.SetActive(false);
        WashingObject.SetActive(false);
        OpenAbgabe();
        Debug.Log("close washing");
    }

    public void OpenAbgabe() 
    {
        currentState = State.abgabe;
        WashingObject.SetActive(false);
        AbgabeObject.SetActive(true);
        WashingDone.SetActive(false);

        AbgabeText.text = (badWaeschemittel || badVorwaesche || badTemperature) ? currentCharacter.BadWork : currentCharacter.GoodWork;
        AbgabeCharacterImage.sprite = (badWaeschemittel || badVorwaesche || badTemperature) ? currentCharacter.CharPortraitSad : currentCharacter.CharPortraitHappy;

        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];

        AbgabeOutfitImage.sprite = o.Sprite;
        AbgabeStain1Image.sprite = o.Stain1Bad;
        AbgabeStain2Image.sprite = o.Stain2Bad;

        AbgabeStain1Image.gameObject.SetActive(false);
        AbgabeStain2Image.gameObject.SetActive(false);
        if (badWaeschemittel || badVorwaesche || badTemperature) 
        {
            AbgabeStain1Image.gameObject.SetActive(true);
            AbgabeStain2Image.gameObject.SetActive(o.Stain2 != null);
        }
    }
    public void CloseAbgabe() 
    {
        AbgabeObject.SetActive(false);

        badTemperature = false;
        badVorwaesche = false;
        badWaeschemittel = false;
        vorwaesche1selected = false;
        vorwaesche2selected = false;

        if (currentCharacter.CharIndex == 0) 
        {
            StartDialogue(char2);
        }
        else 
        {
            currentDay++;
            if (currentDay < 3) 
            {
                StartDialogue(char1);
            }
            else 
            {
            //TODO OUTRO
            }
        }
    }

}
