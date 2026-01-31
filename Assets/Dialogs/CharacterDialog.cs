using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterDialog", menuName = "Scriptable Objects/CharacterDialog")]
public class CharacterDialog : ScriptableObject
{
    public string CharName;

    public Sprite CharPortraitDefault;
    public Sprite CharPortraitHappy;
    public Sprite CharPortraitSad;
    public Sprite CharPortraitFlirty;

    [System.Serializable]
    public struct Response
    {
        public string ResponceTextID;
        public string text;
        public int ResponcePoints;
    }
    [System.Serializable]
    public struct TextParts
    {
        public int PointsForTextPart;
        public string TextPartID;
        public string text;
        public List<Response> TextPartResponses;
    }
    [System.Serializable]
    public struct Dialogs
    {
        public int MinPointsForDialog;
        public List<TextParts> DialogsVariant;
    }

    [System.Serializable]
    public struct DayDialogs
    {
        public int dayIndex;
        public List<Dialogs> AllDayDialogs;
    }
    [SerializeField]
    public List<DayDialogs> AllDialogs;
}
