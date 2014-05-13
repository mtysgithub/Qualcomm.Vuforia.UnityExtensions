using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TextRecoAbstractBehaviour : MonoBehaviour, ITrackerEventHandler, IEditorTextRecoBehaviour
{
    [HideInInspector, SerializeField]
    private string mAdditionalCustomWords;
    [SerializeField, HideInInspector]
    private string mAdditionalFilterWords;
    [SerializeField, HideInInspector]
    private string mCustomWordListFile;
    [SerializeField, HideInInspector]
    private string mFilterListFile;
    [SerializeField, HideInInspector]
    private WordFilterMode mFilterMode;
    private bool mHasInitializedOnce;
    [HideInInspector, SerializeField]
    private int mMaximumWordInstances;
    private List<ITextRecoEventHandler> mTextRecoEventHandlers = new List<ITextRecoEventHandler>();
    [HideInInspector, SerializeField]
    private string mWordListFile;
    [SerializeField, HideInInspector]
    private WordPrefabCreationMode mWordPrefabCreationMode;

    protected TextRecoAbstractBehaviour()
    {
    }

    private void Awake()
    {
        if (QCARRuntimeUtilities.IsQCAREnabled())
        {
            if (QCARRuntimeUtilities.IsPlayMode())
            {
                QCARUnity.CheckInitializationError();
            }
            bool flag = false;
            QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
            if ((behaviour != null) && behaviour.enabled)
            {
                behaviour.enabled = false;
                flag = true;
            }
            if (TrackerManager.Instance.GetTracker<TextTracker>() == null)
            {
                TrackerManager.Instance.InitTracker<TextTracker>();
            }
            if (flag)
            {
                behaviour.enabled = true;
            }
        }
    }

    private void NotifyEventHandlersOfChanges(IEnumerable<Word> lostWords, IEnumerable<WordResult> newWords)
    {
        foreach (Word word in lostWords)
        {
            foreach (ITextRecoEventHandler handler in this.mTextRecoEventHandlers)
            {
                handler.OnWordLost(word);
            }
        }
        foreach (WordResult result in newWords)
        {
            foreach (ITextRecoEventHandler handler2 in this.mTextRecoEventHandlers)
            {
                handler2.OnWordDetected(result);
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!QCARRuntimeUtilities.IsPlayMode())
        {
            if (pause)
            {
                this.StopTextTracker();
            }
            else
            {
                this.StartTextTracker();
            }
        }
    }

    private void OnDestroy()
    {
        QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (behaviour != null)
        {
            behaviour.UnregisterTrackerEventHandler(this);
        }
        TextTracker tracker = TrackerManager.Instance.GetTracker<TextTracker>();
        if (tracker != null)
        {
            tracker.WordList.UnloadAllLists();
        }
    }

    private void OnDisable()
    {
        this.StopTextTracker();
    }

    private void OnEnable()
    {
        if (this.mHasInitializedOnce)
        {
            this.StartTextTracker();
        }
    }

    public void OnInitialized()
    {
        this.SetupWordList();
        this.StartTextTracker();
        this.mHasInitializedOnce = true;
        ((WordManagerImpl) TrackerManager.Instance.GetStateManager().GetWordManager()).InitializeWordBehaviourTemplates(this.mWordPrefabCreationMode, this.mMaximumWordInstances);
        foreach (ITextRecoEventHandler handler in this.mTextRecoEventHandlers)
        {
            handler.OnInitialized();
        }
    }

    public void OnTrackablesUpdated()
    {
        WordManagerImpl wordManager = (WordManagerImpl) TrackerManager.Instance.GetStateManager().GetWordManager();
        IEnumerable<WordResult> newWords = wordManager.GetNewWords();
        IEnumerable<Word> lostWords = wordManager.GetLostWords();
        this.NotifyEventHandlersOfChanges(lostWords, newWords);
    }

    public void RegisterTextRecoEventHandler(ITextRecoEventHandler trackableEventHandler)
    {
        this.mTextRecoEventHandlers.Add(trackableEventHandler);
        if (this.mHasInitializedOnce)
        {
            trackableEventHandler.OnInitialized();
        }
    }

    private void SetupWordList()
    {
        TextTracker tracker = TrackerManager.Instance.GetTracker<TextTracker>();
        if (tracker != null)
        {
            WordList wordList = tracker.WordList;
            wordList.LoadWordListFile(this.mWordListFile);
            if (this.mCustomWordListFile != "")
            {
                wordList.AddWordsFromFile(this.mCustomWordListFile);
            }
            if (this.mAdditionalCustomWords != null)
            {
                foreach (string str in this.mAdditionalCustomWords.Split(new char[] { '\r', '\n' }))
                {
                    if (str.Length > 0)
                    {
                        wordList.AddWord(str);
                    }
                }
            }
            wordList.SetFilterMode(this.mFilterMode);
            if (this.mFilterMode != WordFilterMode.NONE)
            {
                if (this.mFilterListFile != "")
                {
                    wordList.LoadFilterListFile(this.mFilterListFile);
                }
                if (this.mAdditionalFilterWords != null)
                {
                    foreach (string str2 in this.mAdditionalFilterWords.Split(new char[] { '\n' }))
                    {
                        if (str2.Length > 0)
                        {
                            wordList.AddWordToFilterList(str2);
                        }
                    }
                }
            }
        }
    }

    private void Start()
    {
        if ((KeepAliveAbstractBehaviour.Instance != null) && KeepAliveAbstractBehaviour.Instance.KeepTextRecoBehaviourAlive)
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }
        QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (behaviour != null)
        {
            behaviour.RegisterTrackerEventHandler(this, false);
        }
    }

    private void StartTextTracker()
    {
        Debug.Log("Starting Text Tracker");
        TextTracker tracker = TrackerManager.Instance.GetTracker<TextTracker>();
        if (tracker != null)
        {
            tracker.Start();
        }
    }

    private void StopTextTracker()
    {
        Debug.Log("Stopping Text Tracker");
        TextTracker tracker = TrackerManager.Instance.GetTracker<TextTracker>();
        if (tracker != null)
        {
            tracker.Stop();
        }
    }

    public bool UnregisterTextRecoEventHandler(ITextRecoEventHandler trackableEventHandler)
    {
        return this.mTextRecoEventHandlers.Remove(trackableEventHandler);
    }

    string IEditorTextRecoBehaviour.AdditionalCustomWords
    {
        get
        {
            return this.mAdditionalCustomWords;
        }
        set
        {
            this.mAdditionalCustomWords = value;
        }
    }

    string IEditorTextRecoBehaviour.AdditionalFilterWords
    {
        get
        {
            return this.mAdditionalFilterWords;
        }
        set
        {
            this.mAdditionalFilterWords = value;
        }
    }

    string IEditorTextRecoBehaviour.CustomWordListFile
    {
        get
        {
            return this.mCustomWordListFile;
        }
        set
        {
            this.mCustomWordListFile = value;
        }
    }

    string IEditorTextRecoBehaviour.FilterListFile
    {
        get
        {
            return this.mFilterListFile;
        }
        set
        {
            this.mFilterListFile = value;
        }
    }

    WordFilterMode IEditorTextRecoBehaviour.FilterMode
    {
        get
        {
            return this.mFilterMode;
        }
        set
        {
            this.mFilterMode = value;
        }
    }

    int IEditorTextRecoBehaviour.MaximumWordInstances
    {
        get
        {
            return this.mMaximumWordInstances;
        }
        set
        {
            this.mMaximumWordInstances = value;
        }
    }

    string IEditorTextRecoBehaviour.WordListFile
    {
        get
        {
            return this.mWordListFile;
        }
        set
        {
            this.mWordListFile = value;
        }
    }

    WordPrefabCreationMode IEditorTextRecoBehaviour.WordPrefabCreationMode
    {
        get
        {
            return this.mWordPrefabCreationMode;
        }
        set
        {
            this.mWordPrefabCreationMode = value;
        }
    }

    public bool IsInitialized
    {
        get
        {
            return this.mHasInitializedOnce;
        }
    }
}

