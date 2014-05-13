using System;

public abstract class WordList
{
    protected WordList()
    {
    }

    public abstract bool AddWord(string word);
    public abstract int AddWordsFromFile(string relativePath);
    public abstract int AddWordsFromFile(string path, DataSet.StorageType storageType);
    public abstract bool AddWordToFilterList(string word);
    public abstract bool ClearFilterList();
    public abstract bool ContainsWord(string word);
    public abstract string GetFilterListWord(int index);
    public abstract int GetFilterListWordCount();
    public abstract WordFilterMode GetFilterMode();
    public abstract bool LoadFilterListFile(string relativePath);
    public abstract bool LoadFilterListFile(string path, DataSet.StorageType storageType);
    public abstract bool LoadWordListFile(string relativePath);
    public abstract bool LoadWordListFile(string relativePath, DataSet.StorageType storageType);
    public abstract bool RemoveWord(string word);
    public abstract bool RemoveWordFromFilterList(string word);
    public abstract bool SetFilterMode(WordFilterMode mode);
    public abstract bool UnloadAllLists();
}

