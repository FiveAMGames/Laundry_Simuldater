using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Logic : MonoBehaviour
{
    public static Logic Instance;
    public enum State { menu, dialogue, scratching, wasching, abgabe, finale}
    public State currentState;

    public Scratch scratchScript;

    public int currentDay;
    int char1Points = 0;
    int char2Points = 0;

    public GameObject Intro;

    [Header("Dialog stuff")]
    public GameObject DialogObject;
    public Image DialogPortrait;
    public TextMeshProUGUI DialogText;
    public Image Waesche;

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
        public Sprite Stain1White;
        public Sprite Stain1Bad;
        public Sprite Stain2;
        public Sprite Stain2White;
        public Sprite Stain2Bad;

        public Sprite WaescheHaufen;

        public GameObject OutfitMaskPrefab;

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

    public SpriteRenderer Stain1MaskObject;
    public SpriteRenderer Stain2MaskObject;

    public GameObject ScratchMoreObject;
    bool vorwaesche1selected = false;
    bool vorwaesche2selected = false;
    int vowaescheRandomSelectedCount = 0;
    bool badVorwaesche = false;

    public RenderTextureColorCheck TextureCheckScript;
    public Image TemprtureTag;
    public Image WolleSportTag;
    public Vector3 TagPositionChar1;
    public Vector3 TagPositionChar2;
    public GameObject TagPosition;

    public Sprite Degree30;
    public Sprite Degree40;
    public Sprite Degree60;
    public Sprite Wolle;
    public Sprite Sport;

    public Texture2D VorwaescheAcidSprite;
    public Texture2D VorwaescheSoapSprite;
    public Texture2D VorwaescheSaltSprite;

    public Texture2D DefaultCursor;

    public Transform OutfitMaskParent;

    [Header("Washing")]
    public GameObject WashingObject;
    public GameObject MashineOpenedDoor;
    public GameObject MashineClosedDoor;

    public GameObject ChooseMittelWashenObject;
    public GameObject ChooseTempretureWashenObject;

    public List<GameObject> TemperatureButtons;

    public GameObject WashingDone;
    public Image WaescheWashing;
   
    [Header("Abgabe")]
    public GameObject AbgabeObject;
    public TextMeshProUGUI AbgabeText;
    public Image AbgabeCharacterImage;
    public Image AbgabeOutfitImage;
    public Image AbgabeStain1Image;
    public Image AbgabeStain2Image;

    public Image Voerwaesche1Abgabe;
    public GameObject Vorwaesche1AbgabeBad;
    public Image Voerwaesche2Abgabe;
    public GameObject Vorwaesche2AbgabeBad;

    public Sprite AcidSprite;
    public Sprite SoapSprite;
    public Sprite SaltSprite;

    public Image WaescheAbgabe;
    public GameObject WaescheAbgabeBad;
    public Sprite ColorMittelSprite;
    public Sprite SportMittelSprite;
    public Sprite DelicateMittelSprite;
    public Sprite WhiteMittelSprite;

    public Image TempratureAbgabe;
    public GameObject TemperatureAbgabeBad;


    [Header("Finale")]
    public GameObject Finale;
    public TextMeshProUGUI FinaleText;
    public GameObject char1Finale;
    public GameObject char2Finale;

    public string FinaleSingle;
    public string FinaleChar1;
    public string FinaleChar2;
    public string FinaleBoth;



    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip buttons;
    public AudioClip chooseAnswer;
    public AudioClip grabBottle;
    public AudioClip scrabbinSound;



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

        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];
        Waesche.sprite = o.WaescheHaufen;

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
            audioSource.PlayOneShot(buttons);
            textPartIndex++;
            UpdateDialogue();
        }        
    }

    public void ClickOnResponse(int index) 
    {
        if (hasResponses) 
        {
            audioSource.PlayOneShot(buttons);
            bool char1 = currentCharacter.CharIndex == 0;
            if (char1) char1Points += Responses[index].points;
            else char2Points += Responses[index].points;

            hasResponses = false;
            textPartIndex++;
            UpdateDialogue();
        }
    
    }
    public void ResponseSound() 
    {
        audioSource.PlayOneShot(chooseAnswer);
    }

    public void OpenScratching() 
    {
        currentState = State.scratching;
        DialogObject.SetActive(false);
        ScratchingGameObject.SetActive(true);
        vorwaesche1selected = false;
        vorwaesche2selected = false;
        vowaescheRandomSelectedCount = 0;

        Stain1BadWashingObject.gameObject.SetActive(true);
        Stain2BadWashingObject.gameObject.SetActive(true);


        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];
        OutfitImage.sprite = o.Sprite;
        Stain1Object.sprite = o.Stain1;
        Stain1MaskObject.sprite = o.Stain1White;
        Stain2Object.gameObject.SetActive(false);

        Stain1BadWashingObject.sprite = o.Stain1Bad;
        if(o.Stain2 != null) 
        {
            Stain2Object.gameObject.SetActive(true);
            Stain2Object.sprite = o.Stain2;
            Stain2BadWashingObject.sprite = o.Stain2Bad;
            Stain2MaskObject.sprite = o.Stain2White;

        }

        TagPosition.transform.position = currentCharacter.CharIndex == 0 ? TagPositionChar1 : TagPositionChar2;
        TemprtureTag.sprite = Degree30;
        if (o.temperatureType == Outfit.Temperatur.forty) TemprtureTag.sprite = Degree40;
        if (o.temperatureType == Outfit.Temperatur.sixty) TemprtureTag.sprite = Degree60;

        WolleSportTag.gameObject.SetActive(true);
        if (o.waescheType == Outfit.Waesche.wolle) WolleSportTag.sprite = Wolle;
        else if (o.waescheType == Outfit.Waesche.sport) WolleSportTag.sprite = Sport;
        else if (o.waescheType == Outfit.Waesche.weiss || o.waescheType == Outfit.Waesche.farbe) WolleSportTag.gameObject.SetActive(false);


        Instantiate(o.OutfitMaskPrefab, OutfitMaskParent);


        scratchScript.Clear();
        scratchScript.CaptureStainArea();

    }

    public void SelectVorwaesche(Outfit.Vorwaeasche type) 
    {
        audioSource.PlayOneShot(grabBottle);

        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];
        vowaescheRandomSelectedCount++;
        if (o.vorwaescheType1 == type) 
        {
            Stain1BadWashingObject.gameObject.SetActive(false);
            vorwaesche1selected = true;
        }
        else if (o.vorwaescheType2 == type) 
        {
            Stain2BadWashingObject.gameObject.SetActive(false);
            vorwaesche2selected = true;
        }
        if (o.Stain2 == null)
        {
            vowaescheRandomSelectedCount = 2;
            vorwaesche2selected = true;
        }



        Vector2 hotspot = new Vector2(-0.5f, 0.5f);

        switch (type)
        {
            case Outfit.Vorwaeasche.acid:
                cursorSet(VorwaescheAcidSprite);
                break;
            case Outfit.Vorwaeasche.soap:
                cursorSet(VorwaescheSoapSprite);
                break;
            case Outfit.Vorwaeasche.salt:
                cursorSet(VorwaescheSaltSprite);
                break;
            default:
                break;
        }
    }


    void cursorSet(Texture2D tex)
    {
        CursorMode mode = CursorMode.ForceSoftware;
        float xspot = tex.width / 2;
        float yspot = tex.height / 2;
        Vector2 hotSpot = new Vector2(xspot, yspot);
        Cursor.SetCursor(tex, hotSpot, mode);
    }

    public bool CheckIfAllVorwaescheAreSelected() 
    {
        return (vowaescheRandomSelectedCount == 2);
    }

    public void ClickDoneVorwaesche() 
    {
        audioSource.PlayOneShot(buttons);
        if (!vorwaesche1selected || !vorwaesche2selected)
        {
            badVorwaesche = true;
        }
        else badVorwaesche = false;

        bool enough = TextureCheckScript.SamplePanorama();
        if (enough) 
        {
            Cursor.SetCursor(DefaultCursor, Vector2.zero, CursorMode.Auto);
            OpenWasching();
        }
        else ScratchMoreObject.SetActive(true);
    }

    public void CloseScratchMore() 
    {
        audioSource.PlayOneShot(buttons);
        ScratchMoreObject.SetActive(false);        
    }

    public void OpenWasching() 
    {

        if (OutfitMaskParent.childCount > 0) 
        {
            Destroy(OutfitMaskParent.GetChild(0).gameObject);
        }

        StopScrubbingSound();
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

        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];
        WaescheWashing.sprite = o.WaescheHaufen;

    }
    public void ClickWaschMittel(Outfit.Waesche type) 
    {
        audioSource.PlayOneShot(grabBottle);
        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];
        if (o.waescheType == type)
        {
            badWaeschemittel = false;
        }
        else
        {
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
        audioSource.PlayOneShot(buttons);
        Outfit o = currentCharacter.CharIndex == 0 ? char1Outfits[currentDay] : char2Outfits[currentDay];

        Outfit.Temperatur type = Outfit.Temperatur.thirty;
        if (i == 40) type = Outfit.Temperatur.forty;
        if (i == 60) type = Outfit.Temperatur.sixty;

        if (o.temperatureType == type)
        {
            badTemperature = false;
        }
        else
        {
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
        audioSource.PlayOneShot(buttons);
        currentState = State.abgabe;
        WashingObject.SetActive(false);
        AbgabeObject.SetActive(true);
        WashingDone.SetActive(false);


        if ((badWaeschemittel || badVorwaesche || badTemperature)) 
        {
            if (currentCharacter.CharIndex == 0) char1Points--;
            else char2Points--;
        }
        else 
        {
            if (currentCharacter.CharIndex == 0) char1Points++;
            else char2Points++;
        }



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

        Sprite voerwaesche = AcidSprite;
        if (o.vorwaescheType1 == Outfit.Vorwaeasche.salt) voerwaesche = SaltSprite;
        else if (o.vorwaescheType1 == Outfit.Vorwaeasche.soap) voerwaesche = SoapSprite;

        Voerwaesche1Abgabe.sprite = voerwaesche;
        if (o.Stain2 != null)
        {
            voerwaesche = AcidSprite;
            if (o.vorwaescheType2 == Outfit.Vorwaeasche.salt) voerwaesche = SaltSprite;
            else if (o.vorwaescheType2 == Outfit.Vorwaeasche.soap) voerwaesche = SoapSprite;
            Voerwaesche2Abgabe.sprite = voerwaesche;
            Voerwaesche2Abgabe.gameObject.SetActive(true);
        }
        else Voerwaesche2Abgabe.gameObject.SetActive(false);

        Sprite waesche = ColorMittelSprite;
        if (o.waescheType == Outfit.Waesche.sport) waesche = SportMittelSprite;
        else if (o.waescheType == Outfit.Waesche.weiss) waesche = WhiteMittelSprite;
        else if (o.waescheType == Outfit.Waesche.wolle) waesche = DelicateMittelSprite;
        WaescheAbgabe.sprite = waesche;

        Sprite tempr = Degree30;
        if (o.temperatureType == Outfit.Temperatur.forty) tempr = Degree40;
        else if (o.temperatureType == Outfit.Temperatur.sixty) tempr = Degree60;

        TempratureAbgabe.sprite = tempr;

        Vorwaesche1AbgabeBad.SetActive(!vorwaesche1selected);
        Vorwaesche2AbgabeBad.SetActive(!vorwaesche2selected);

        WaescheAbgabeBad.SetActive(badWaeschemittel);
        TemperatureAbgabeBad.SetActive(badTemperature);
    }
    public void CloseAbgabe() 
    {
        audioSource.PlayOneShot(buttons);
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
                OpenFinale();
            }
        }
    }

    public void OpenFinale() 
    {
        currentState = State.finale;
        AbgabeObject.SetActive(false);
        Finale.SetActive(true);
        char1Finale.SetActive(char1Points >= 5);
        char2Finale.SetActive(char2Points >= 5);

        string t = FinaleSingle;
        if (char1Points >= 5 && char2Points < 5) t = FinaleChar1;
        if (char2Points >= 5 && char1Points < 5) t = FinaleChar2;
        if (char1Points >= 5 && char2Points >=5) t = FinaleBoth;

        FinaleText.text = t;
    }

    public void ClickReplay() 
    {
        audioSource.PlayOneShot(buttons);
        SceneManager.LoadScene(0);
    }

    public void PlayScrubbingSound() 
    {
        audioSource.clip = scrabbinSound;
        audioSource.loop = true;
        audioSource.Play();
    }
    public void StopScrubbingSound()
    {
        audioSource.Stop();
    }

}
