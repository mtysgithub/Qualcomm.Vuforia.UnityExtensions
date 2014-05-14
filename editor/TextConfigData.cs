using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class TextConfigData
{
    private Dictionary<string, DictionaryData> mDictionaries = new Dictionary<string, DictionaryData>();
    private Dictionary<string, WordListData> mWordLists = new Dictionary<string, WordListData>();

    public void CopyDictionaryNames(string[] arrayToFill, int index)
    {
        this.mDictionaries.Keys.CopyTo(arrayToFill, index);
    }

    public void CopyDictionaryNamesAndFiles(string[] namesArray, string[] filesArray, int index)
    {
        foreach (KeyValuePair<string, DictionaryData> pair in this.mDictionaries)
        {
            namesArray[index] = pair.Key;
            filesArray[index] = pair.Value.BinaryFile;
            index++;
        }
    }

    public void CopyWordListNames(string[] arrayToFill, int index)
    {
        this.mWordLists.Keys.CopyTo(arrayToFill, index);
    }

    public void CopyWordListNamesAndFiles(string[] namesArray, string[] filesArray, int index)
    {
        foreach (KeyValuePair<string, WordListData> pair in this.mWordLists)
        {
            namesArray[index] = pair.Key;
            filesArray[index] = pair.Value.TextFile;
            index++;
        }
    }

    public DictionaryData GetDictionaryData(string name)
    {
        return this.mDictionaries[name];
    }

    public WordListData GetWordListData(string name)
    {
        return this.mWordLists[name];
    }

    public void SetDictionaryData(DictionaryData data, string name)
    {
        this.mDictionaries[name] = data;
    }

    public void SetWordListData(WordListData data, string name)
    {
        this.mWordLists[name] = data;
    }

    public int NumDictionaries
    {
        get
        {
            return this.mDictionaries.Count;
        }
    }

    public int NumWordLists
    {
        get
        {
            return this.mWordLists.Count;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DictionaryData
    {
        public string BinaryFile;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WordListData
    {
        public string TextFile;
    }
}

