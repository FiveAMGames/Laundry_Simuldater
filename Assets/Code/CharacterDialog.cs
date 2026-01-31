using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterDialog", menuName = "Scriptable Objects/CharacterDialog")]
public class CharacterDialog : ScriptableObject
{
    public string CharName;
    public int CharIndex;

    public Sprite CharPortraitDefault;
    public Sprite CharPortraitHappy;
    public Sprite CharPortraitSad;
    public Sprite CharPortraitFlirty;

    public string GoodWork;
    public string BadWork;

    [System.Serializable]
    public class Response
    {
        public string ResponceTextID;
        public string text;
        public int ResponcePoints;
        public void SetText(string s)
        {
            text = s;
        }
    }
    [System.Serializable]
    public class TextParts
    {
        public string TextPartID;
        public string text;
        public string animation;
        public List<Response> TextPartResponses;

        public void SetText(string s, string _animations) 
        {
            text = s;
            animation = _animations;
        }
    }
    [System.Serializable]
    public class Dialogs
    {
        public Vector2Int MinPointsForDialog;
        public List<TextParts> DialogPart;
    }

    [System.Serializable]
    public class DayDialogParts
    {        
        public List<Dialogs> VariantDialogs;
    }

    [System.Serializable]
    public class DayDialogs
    {
        public int dayIndex;
        public List<DayDialogParts> DialogParts;
        
    }
    [SerializeField]
    public List<DayDialogs> AllDialogs;
}
