using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextRecoAbstractBehaviour), true)]
public class TextRecoEditor : Editor
{
    private static void ConvertEOLAndEncodingIfUserWantsTo(string file)
    {
        if (EditorUtility.DisplayDialog("Wrong Line Endings", "Would you like to automatically convert the line endings and/or remove the BOM of the selected word list file?", "Yes", "No"))
        {
            string contents = File.ReadAllText(file).Replace("\r\n", "\n");
            File.WriteAllText(file, contents, new UTF8Encoding(false));
        }
    }

    public void OnEnable()
    {
        TextRecoAbstractBehaviour target = (TextRecoAbstractBehaviour) base.target;
        if ((QCARUtilities.GetPrefabType(target) != PrefabType.Prefab) && !SceneManager.Instance.SceneInitialized)
        {
            SceneManager.Instance.InitScene();
        }
    }

    public override void OnInspectorGUI()
    {
        TextRecoAbstractBehaviour target = (TextRecoAbstractBehaviour) base.target;
        IEditorTextRecoBehaviour behaviour2 = (IEditorTextRecoBehaviour) base.target;
        EditorGUILayout.HelpBox("The list of words the TextTracker can detect and track.\nThe word list is loaded from a binary file and can be extended by a list of custom words.", MessageType.Info);
        TextConfigData textConfigData = ConfigDataManager.Instance.GetTextConfigData();
        if (textConfigData.NumDictionaries > 1)
        {
            string[] strArray = new string[textConfigData.NumDictionaries];
            string[] strArray2 = new string[textConfigData.NumDictionaries];
            textConfigData.CopyDictionaryNamesAndFiles(strArray, strArray2, 0);
            int num = QCARUtilities.GetIndexFromString(behaviour2.WordListFile, strArray2);
            if (num < 0)
            {
                num = 0;
            }
            int num2 = EditorGUILayout.Popup("Word List", num, strArray, new GUILayoutOption[0]);
            behaviour2.WordListFile = strArray2[num2];
            if (num2 == 0)
            {
                GUI.enabled = false;
            }
        }
        else
        {
            if (GUILayout.Button("No word list available. \nPlease copy it from the TextRecognition sample app. \nPress here to go to the download page for sample apps!", new GUILayoutOption[0]))
            {
                SceneManager.Instance.GoToSampleAppPage();
            }
            GUI.enabled = false;
        }
        int numWordLists = textConfigData.NumWordLists;
        string[] namesArray = new string[numWordLists];
        string[] filesArray = new string[numWordLists];
        textConfigData.CopyWordListNamesAndFiles(namesArray, filesArray, 0);
        int indexFromString = QCARUtilities.GetIndexFromString(behaviour2.CustomWordListFile, filesArray);
        if (indexFromString < 0)
        {
            indexFromString = 0;
        }
        int index = EditorGUILayout.Popup("Additional Word File", indexFromString, namesArray, new GUILayoutOption[0]);
        if (index != indexFromString)
        {
            if (index != 0)
            {
                TestValidityOfWordListFile(filesArray[index]);
            }
            behaviour2.CustomWordListFile = filesArray[index];
        }
        EditorGUILayout.LabelField("Additional Words:", new GUILayoutOption[0]);
        EditorGUILayout.HelpBox("Write one word per line. Open compound words can be specified using whitespaces.", MessageType.None);
        behaviour2.AdditionalCustomWords = EditorGUILayout.TextArea(behaviour2.AdditionalCustomWords, new GUILayoutOption[0]);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("The filter list allows to specify subset of words that will be detected and tracked.", MessageType.Info);
        string[] displayedOptions = new string[] { "NONE", "BLACK_LIST", "WHITE_LIST" };
        List<WordFilterMode> list = new List<WordFilterMode> { WordFilterMode.NONE, WordFilterMode.BLACK_LIST, WordFilterMode.WHITE_LIST };
        int selectedIndex = list.IndexOf(behaviour2.FilterMode);
        selectedIndex = EditorGUILayout.Popup("Filter Mode", selectedIndex, displayedOptions, new GUILayoutOption[0]);
        behaviour2.FilterMode = list[selectedIndex];
        if (behaviour2.FilterMode != WordFilterMode.NONE)
        {
            int num7 = QCARUtilities.GetIndexFromString(behaviour2.FilterListFile, filesArray);
            if (num7 < 0)
            {
                num7 = 0;
            }
            int num8 = EditorGUILayout.Popup("Filter List File", num7, namesArray, new GUILayoutOption[0]);
            if (num8 != num7)
            {
                if (num8 != 0)
                {
                    TestValidityOfWordListFile(filesArray[num8]);
                }
                behaviour2.FilterListFile = filesArray[num8];
            }
            EditorGUILayout.LabelField("Additional Filter Words:", new GUILayoutOption[0]);
            EditorGUILayout.HelpBox("Write one word per line. Open compound words can be specified using whitespaces.", MessageType.None);
            behaviour2.AdditionalFilterWords = EditorGUILayout.TextArea(behaviour2.AdditionalFilterWords, new GUILayoutOption[0]);
        }
        EditorGUILayout.HelpBox("It is possible to use Word Prefabs to define augmentations for detected words. Each Word Prefab can be instantiated up to a maximum number.", MessageType.Info);
        if (EditorGUILayout.Toggle("Use Word Prefabs", behaviour2.WordPrefabCreationMode == WordPrefabCreationMode.DUPLICATE, new GUILayoutOption[0]))
        {
            behaviour2.WordPrefabCreationMode = WordPrefabCreationMode.DUPLICATE;
            behaviour2.MaximumWordInstances = EditorGUILayout.IntField("Max Simultaneous Words", behaviour2.MaximumWordInstances, new GUILayoutOption[0]);
        }
        else
        {
            behaviour2.WordPrefabCreationMode = WordPrefabCreationMode.NONE;
        }
        GUI.enabled = true;
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    public void OnSceneGUI()
    {
        TextRecoAbstractBehaviour target = (TextRecoAbstractBehaviour) base.target;
        GUIStyle style2 = new GUIStyle {
            alignment = TextAnchor.LowerRight,
            fontSize = 0x12
        };
        style2.normal.textColor = Color.white;
        GUIStyle style = style2;
        Handles.Label(target.transform.position, "Text\nRecognition", style);
    }

    private static void TestValidityOfWordListFile(string file)
    {
        string path = "Assets/StreamingAssets/" + file;
        bool flag = false;
        bool flag2 = true;
        StreamReader reader = File.OpenText(path);
        Stream baseStream = reader.BaseStream;
        for (int i = 0; i < 7; i++)
        {
            int num2 = baseStream.ReadByte();
            if (num2 != "UTF-8\n\n"[i])
            {
                if (((i == 0) && (num2 == 0xef)) && ((baseStream.ReadByte() == 0xbb) && (baseStream.ReadByte() == 0xbf)))
                {
                    Debug.LogWarning("Only UTF-8 documents without BOM are supported.");
                    flag2 = false;
                    flag = true;
                }
                else if ((i == 5) && (num2 == 13))
                {
                    Debug.LogWarning("Only UTF-8 documents without CARRIAGE RETURN are supported.");
                    flag2 = false;
                    flag = true;
                }
                else
                {
                    Debug.LogWarning("Not a valid UTF-8 encoded file. It needs to start with the header \"UTF-8\" followed by an empty line.");
                    flag2 = false;
                }
                break;
            }
        }
        int num3 = 0;
        while (flag2 && !reader.EndOfStream)
        {
            string str2 = reader.ReadLine();
            if (str2.Length == 0)
            {
                Debug.LogWarning("There is an empty line in your word list.");
                flag2 = false;
            }
            else if (str2.Length < 2)
            {
                Debug.LogWarning("The word " + str2 + " is too short for Text-Reco.");
                flag2 = false;
            }
            else if (str2.Length > 0x2d)
            {
                Debug.LogWarning("The word " + str2 + " is too long for Text-Reco.");
                flag2 = false;
            }
            else
            {
                foreach (char ch in str2)
                {
                    if ((((ch < 'A') || (ch > 'Z')) && ((ch < 'a') || (ch > 'z'))) && (((ch != '-') && (ch != '\'')) && (ch != ' ')))
                    {
                        Debug.LogWarning(string.Concat(new object[] { "The word ", str2, " is not supported because of character ", ch }));
                        flag2 = false;
                    }
                }
            }
            num3++;
            if (num3 > 0x2710)
            {
                Debug.LogWarning("The maximum number of words is " + 0x2710 + ".");
                flag2 = false;
            }
        }
        reader.Close();
        if (!flag2 && flag)
        {
            ConvertEOLAndEncodingIfUserWantsTo(path);
        }
    }
}

