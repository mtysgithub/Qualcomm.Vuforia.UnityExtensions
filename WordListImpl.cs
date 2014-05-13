using System;
using System.Runtime.InteropServices;

internal class WordListImpl : WordList
{
    public override bool AddWord(string word)
    {
        IntPtr ptr = Marshal.StringToHGlobalUni(word);
        bool flag = QCARWrapper.Instance.WordListAddWordU(ptr) == 1;
        Marshal.FreeHGlobal(ptr);
        return flag;
    }

    public override int AddWordsFromFile(string relativePath)
    {
        return this.AddWordsFromFile(relativePath, DataSet.StorageType.STORAGE_APPRESOURCE);
    }

    public override int AddWordsFromFile(string path, DataSet.StorageType storageType)
    {
        if ((storageType == DataSet.StorageType.STORAGE_APPRESOURCE) && QCARRuntimeUtilities.IsPlayMode())
        {
            path = "Assets/StreamingAssets/" + path;
        }
        return QCARWrapper.Instance.WordListAddWordsFromFile(path, (int) storageType);
    }

    public override bool AddWordToFilterList(string word)
    {
        IntPtr ptr = Marshal.StringToHGlobalUni(word);
        bool flag = QCARWrapper.Instance.WordListAddWordToFilterListU(ptr) == 1;
        Marshal.FreeHGlobal(ptr);
        return flag;
    }

    public override bool ClearFilterList()
    {
        return (QCARWrapper.Instance.WordListClearFilterList() == 1);
    }

    public override bool ContainsWord(string word)
    {
        IntPtr ptr = Marshal.StringToHGlobalUni(word);
        bool flag = QCARWrapper.Instance.WordListContainsWordU(ptr) == 1;
        Marshal.FreeHGlobal(ptr);
        return flag;
    }

    public override string GetFilterListWord(int index)
    {
        return Marshal.PtrToStringUni(QCARWrapper.Instance.WordListGetFilterListWordU(index));
    }

    public override int GetFilterListWordCount()
    {
        return QCARWrapper.Instance.WordListGetFilterListWordCount();
    }

    public override WordFilterMode GetFilterMode()
    {
        return (WordFilterMode) QCARWrapper.Instance.WordListGetFilterMode();
    }

    public override bool LoadFilterListFile(string relativePath)
    {
        return this.LoadFilterListFile(relativePath, DataSet.StorageType.STORAGE_APPRESOURCE);
    }

    public override bool LoadFilterListFile(string path, DataSet.StorageType storageType)
    {
        if ((storageType == DataSet.StorageType.STORAGE_APPRESOURCE) && QCARRuntimeUtilities.IsPlayMode())
        {
            path = "Assets/StreamingAssets/" + path;
        }
        return (QCARWrapper.Instance.WordListLoadFilterList(path, (int) storageType) == 1);
    }

    public override bool LoadWordListFile(string relativePath)
    {
        return this.LoadWordListFile(relativePath, DataSet.StorageType.STORAGE_APPRESOURCE);
    }

    public override bool LoadWordListFile(string path, DataSet.StorageType storageType)
    {
        if ((storageType == DataSet.StorageType.STORAGE_APPRESOURCE) && QCARRuntimeUtilities.IsPlayMode())
        {
            path = "Assets/StreamingAssets/" + path;
        }
        return (QCARWrapper.Instance.WordListLoadWordList(path, (int) storageType) == 1);
    }

    public override bool RemoveWord(string word)
    {
        IntPtr ptr = Marshal.StringToHGlobalUni(word);
        bool flag = QCARWrapper.Instance.WordListRemoveWordU(ptr) == 1;
        Marshal.FreeHGlobal(ptr);
        return flag;
    }

    public override bool RemoveWordFromFilterList(string word)
    {
        IntPtr ptr = Marshal.StringToHGlobalUni(word);
        bool flag = QCARWrapper.Instance.WordListRemoveWordFromFilterListU(ptr) == 1;
        Marshal.FreeHGlobal(ptr);
        return flag;
    }

    public override bool SetFilterMode(WordFilterMode mode)
    {
        return (QCARWrapper.Instance.WordListSetFilterMode((int) mode) == 1);
    }

    public override bool UnloadAllLists()
    {
        return (QCARWrapper.Instance.WordListUnloadAllLists() == 1);
    }
}

