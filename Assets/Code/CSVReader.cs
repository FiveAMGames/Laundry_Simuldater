using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditor;



[System.Serializable]
public class Texts
{

    [Serializable]
    public class SmallText 
    {
        public string ID;
        public string Text;
        public string animation;
        public SmallText(string _ID, string _Text, string _Animation) 
        {
            ID = _ID;
            Text = _Text;
            animation = _Animation;
        }
    }
    [SerializeField]
    public List<SmallText> SmallTexts = new List<SmallText>();
}


[CreateAssetMenu(menuName = "Spreadheet Reader")]
[System.Serializable]
public class CSVReader : ScriptableObject
{

    public static CSVReader Instance;

    public static CSVReader currentSpreadsheet;
    public TextAsset TextCsv;
    public Texts AllTexts;

    public CharacterDialog char1;
    public CharacterDialog char2;

    [ExecuteInEditMode]
    [ContextMenu("Set Up Texts")]
    public void SetUpText()
    {
            AllTexts.SmallTexts.Clear();

            Texts newText = new Texts();
            string[,] lines = CSVReader.SplitCsvGrid(TextCsv.text);

            for (int i = 0; i < 65; i++)  //look through the lines with new ID
            {
                if (lines[0, i].Length == 0) break;
                else
                {
                    string ID = lines[0, i];
                    string text = lines[2, i];
                    string anim = lines[1, i];

                    Texts.SmallText smalltext = new Texts.SmallText(ID, text, anim);
                    newText.SmallTexts.Add(smalltext);
                }           
            }
            AllTexts = newText;
     
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
        Debug.Log("Texts are set");
    }

    
    public void PopulateCharDialogs(CharacterDialog _char) 
    {
        for (int i = 0; i < _char.AllDialogs.Count; i++)
        {
            for (int j = 0; j < _char.AllDialogs[i].DialogParts.Count; j++) 
            {
                for (int m = 0; m < _char.AllDialogs[i].DialogParts[j].VariantDialogs.Count; m++) 
                {
                    for (int k = 0; k < _char.AllDialogs[i].DialogParts[j].VariantDialogs[m].DialogPart.Count; k++) 
                    {
                        for (int l = 0; l < _char.AllDialogs[i].DialogParts[j].VariantDialogs[m].DialogPart[k].TextPartResponses.Count; l ++)
                        {
                            foreach (Texts.SmallText t in AllTexts.SmallTexts)
                            {
                                if (t.ID == _char.AllDialogs[i].DialogParts[j].VariantDialogs[m].DialogPart[k].TextPartResponses[l].ResponceTextID)
                                {
                                    _char.AllDialogs[i].DialogParts[j].VariantDialogs[m].DialogPart[k].TextPartResponses[l].SetText(t.Text);
                                    break;
                                }
                            }
                            
                        }
                        foreach (Texts.SmallText t in AllTexts.SmallTexts)
                            {
                                if (t.ID == _char.AllDialogs[i].DialogParts[j].VariantDialogs[m].DialogPart[k].TextPartID)
                                {
                                    _char.AllDialogs[i].DialogParts[j].VariantDialogs[m].DialogPart[k].SetText(t.Text, t.animation);
                                    break;
                                }
                            }

                    }                                
                }            
            }
        }
        EditorUtility.SetDirty(this);
    }


    // splits a CSV file into a 2D string array
    static public string[,] SplitCsvGrid(string csvText)
    {
        string[] lines = csvText.Split("\n"[0]);

        // finds the max width of row
        int width = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = SplitCsvLine(lines[i]);
            width = Mathf.Max(width, row.Length);
        }

        // creates new 2D string grid to output to
        string[,] outputGrid = new string[width + 1, lines.Length + 1];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = SplitCsvLine(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];

                // This line was to replace "" with " in my output. 
                // Include or edit it as you wish.
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }

        return outputGrid;
    }

    // splits a CSV row 
    static public string[] SplitCsvLine(string line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
        @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
        System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
    }
}





