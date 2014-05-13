using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class WordManagerImpl : WordManager
{
    private readonly Dictionary<int, WordAbstractBehaviour> mActiveWordBehaviours = new Dictionary<int, WordAbstractBehaviour>();
    private bool mAutomaticTemplate;
    private readonly List<Word> mLostWords = new List<Word>();
    private int mMaxInstances = 1;
    private readonly List<WordResult> mNewWords = new List<WordResult>();
    private readonly Dictionary<int, WordResult> mTrackedWords = new Dictionary<int, WordResult>();
    private readonly List<Word> mWaitingQueue = new List<Word>();
    private readonly Dictionary<string, List<WordAbstractBehaviour>> mWordBehaviours = new Dictionary<string, List<WordAbstractBehaviour>>();
    private readonly List<WordAbstractBehaviour> mWordBehavioursMarkedForDeletion = new List<WordAbstractBehaviour>();
    private WordPrefabCreationMode mWordPrefabCreationMode;
    private const string TEMPLATE_IDENTIFIER = "Template_ID";

    private WordAbstractBehaviour AssociateWordBehaviour(WordResult wordResult)
    {
        List<WordAbstractBehaviour> list;
        string key = wordResult.Word.StringValue.ToLowerInvariant();
        if (this.mWordBehaviours.ContainsKey(key))
        {
            list = this.mWordBehaviours[key];
        }
        else if (this.mWordBehaviours.ContainsKey("Template_ID"))
        {
            list = this.mWordBehaviours["Template_ID"];
        }
        else
        {
            Debug.Log("No prefab available for string value " + key);
            return null;
        }
        foreach (WordAbstractBehaviour behaviour in list)
        {
            if (behaviour.Trackable == null)
            {
                return this.AssociateWordBehaviour(wordResult, behaviour);
            }
        }
        if (list.Count < this.mMaxInstances)
        {
            WordAbstractBehaviour item = InstantiateWordBehaviour(list.First<WordAbstractBehaviour>());
            list.Add(item);
            return this.AssociateWordBehaviour(wordResult, item);
        }
        return null;
    }

    private WordAbstractBehaviour AssociateWordBehaviour(WordResult wordResult, WordAbstractBehaviour wordBehaviourTemplate)
    {
        if (this.mActiveWordBehaviours.Count >= this.mMaxInstances)
        {
            return null;
        }
        Word word = wordResult.Word;
        WordAbstractBehaviour behaviour = wordBehaviourTemplate;
        IEditorWordBehaviour behaviour2 = behaviour;
        behaviour2.SetNameForTrackable(word.StringValue);
        behaviour2.InitializeWord(word);
        this.mActiveWordBehaviours.Add(word.ID, behaviour);
        return behaviour;
    }

    private void AssociateWordResultsWithBehaviours()
    {
        List<Word> list = new List<Word>(this.mWaitingQueue);
        foreach (Word word in list)
        {
            if (this.mTrackedWords.ContainsKey(word.ID))
            {
                WordResult wordResult = this.mTrackedWords[word.ID];
                if (this.AssociateWordBehaviour(wordResult) != null)
                {
                    this.mWaitingQueue.Remove(word);
                }
            }
            else
            {
                this.mWaitingQueue.Remove(word);
            }
        }
        foreach (WordResult result2 in this.mNewWords)
        {
            if (this.AssociateWordBehaviour(result2) == null)
            {
                this.mWaitingQueue.Add(result2.Word);
            }
        }
    }

    private static WordAbstractBehaviour CreateWordBehaviour()
    {
        GameObject gameObject = new GameObject("Word-AutoTemplate");
        WordAbstractBehaviour behaviour = BehaviourComponentFactory.Instance.AddWordBehaviour(gameObject);
        Debug.Log("Creating Word Behaviour");
        return behaviour;
    }

    public override void DestroyWordBehaviour(WordAbstractBehaviour behaviour, [Optional, DefaultParameterValue(true)] bool destroyGameObject)
    {
        foreach (string str in this.mWordBehaviours.Keys.ToArray<string>())
        {
            if (this.mWordBehaviours[str].Contains(behaviour))
            {
                this.mWordBehaviours[str].Remove(behaviour);
                if (this.mWordBehaviours[str].Count == 0)
                {
                    this.mWordBehaviours.Remove(str);
                }
                if (destroyGameObject)
                {
                    UnityEngine.Object.Destroy(behaviour.gameObject);
                    this.mWordBehavioursMarkedForDeletion.Add(behaviour);
                }
                else
                {
                    IEditorWordBehaviour behaviour2 = behaviour;
                    behaviour2.UnregisterTrackable();
                }
            }
        }
    }

    public override IEnumerable<WordResult> GetActiveWordResults()
    {
        return this.mTrackedWords.Values;
    }

    public override IEnumerable<Word> GetLostWords()
    {
        return this.mLostWords;
    }

    public override IEnumerable<WordResult> GetNewWords()
    {
        return this.mNewWords;
    }

    public override IEnumerable<WordAbstractBehaviour> GetTrackableBehaviours()
    {
        List<WordAbstractBehaviour> list = new List<WordAbstractBehaviour>();
        foreach (List<WordAbstractBehaviour> list2 in this.mWordBehaviours.Values)
        {
            list.AddRange(list2);
        }
        return list;
    }

    internal void InitializeWordBehaviourTemplates()
    {
        if (this.mWordPrefabCreationMode == WordPrefabCreationMode.DUPLICATE)
        {
            List<WordAbstractBehaviour> list = this.mWordBehavioursMarkedForDeletion.ToList<WordAbstractBehaviour>();
            if (this.mAutomaticTemplate && this.mWordBehaviours.ContainsKey("Template_ID"))
            {
                foreach (WordAbstractBehaviour behaviour in this.mWordBehaviours["Template_ID"])
                {
                    list.Add(behaviour);
                    UnityEngine.Object.Destroy(behaviour.gameObject);
                }
                this.mWordBehaviours.Remove("Template_ID");
            }
            WordAbstractBehaviour[] behaviourArray = (WordAbstractBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(WordAbstractBehaviour));
            foreach (WordAbstractBehaviour behaviour2 in behaviourArray)
            {
                if (!list.Contains(behaviour2))
                {
                    IEditorWordBehaviour behaviour3 = behaviour2;
                    string key = behaviour3.IsTemplateMode ? "Template_ID" : behaviour3.SpecificWord.ToLowerInvariant();
                    if (!this.mWordBehaviours.ContainsKey(key))
                    {
                        this.mWordBehaviours[key] = new List<WordAbstractBehaviour> { behaviour2 };
                        if (key == "Template_ID")
                        {
                            this.mAutomaticTemplate = false;
                        }
                    }
                }
            }
            if (!this.mWordBehaviours.ContainsKey("Template_ID"))
            {
                WordAbstractBehaviour behaviour4 = CreateWordBehaviour();
                this.mWordBehaviours.Add("Template_ID", new List<WordAbstractBehaviour> { behaviour4 });
                this.mAutomaticTemplate = true;
            }
        }
        this.mWordBehavioursMarkedForDeletion.Clear();
    }

    internal void InitializeWordBehaviourTemplates(WordPrefabCreationMode wordPrefabCreationMode, int maxInstances)
    {
        this.mWordPrefabCreationMode = wordPrefabCreationMode;
        this.mMaxInstances = maxInstances;
        this.InitializeWordBehaviourTemplates();
    }

    private static WordAbstractBehaviour InstantiateWordBehaviour(WordAbstractBehaviour input)
    {
        GameObject obj2 = UnityEngine.Object.Instantiate(input.gameObject) as GameObject;
        return obj2.GetComponent<WordAbstractBehaviour>();
    }

    internal void RemoveDestroyedTrackables()
    {
        foreach (List<WordAbstractBehaviour> list in this.mWordBehaviours.Values)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                }
            }
        }
        foreach (string str in this.mWordBehaviours.Keys.ToArray<string>())
        {
            if (this.mWordBehaviours[str].Count == 0)
            {
                this.mWordBehaviours.Remove(str);
            }
        }
    }

    internal void SetWordBehavioursToNotFound()
    {
        foreach (WordAbstractBehaviour behaviour in this.mActiveWordBehaviours.Values)
        {
            behaviour.OnTrackerUpdate(TrackableBehaviour.Status.NOT_FOUND);
        }
    }

    public override bool TryGetWordBehaviour(Word word, out WordAbstractBehaviour behaviour)
    {
        return this.mActiveWordBehaviours.TryGetValue(word.ID, out behaviour);
    }

    private void UnregisterLostWords()
    {
        foreach (Word word in this.mLostWords)
        {
            if (this.mActiveWordBehaviours.ContainsKey(word.ID))
            {
                WordAbstractBehaviour behaviour = this.mActiveWordBehaviours[word.ID];
                behaviour.OnTrackerUpdate(TrackableBehaviour.Status.NOT_FOUND);
                ((IEditorTrackableBehaviour) behaviour).UnregisterTrackable();
                this.mActiveWordBehaviours.Remove(word.ID);
            }
        }
    }

    private void UpdateWordBehaviourPoses()
    {
        foreach (KeyValuePair<int, WordAbstractBehaviour> pair in this.mActiveWordBehaviours)
        {
            if (this.mTrackedWords.ContainsKey(pair.Key))
            {
                WordResult result = this.mTrackedWords[pair.Key];
                Vector3 position = result.Position;
                Quaternion orientation = result.Orientation;
                Vector2 size = result.Word.Size;
                pair.Value.transform.rotation = orientation;
                Vector3 vector3 = (Vector3) (pair.Value.transform.rotation * new Vector3(-size.x * 0.5f, 0f, -size.y * 0.5f));
                pair.Value.transform.position = position + vector3;
                pair.Value.OnTrackerUpdate(result.CurrentStatus);
            }
        }
    }

    private void UpdateWordResultPoses(Camera arCamera, IEnumerable<QCARManagerImpl.WordResultData> wordResults)
    {
        QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (behaviour == null)
        {
            Debug.LogError("QCAR Behaviour could not be found");
        }
        else
        {
            Rect viewportRectangle = behaviour.GetViewportRectangle();
            bool videoBackGroundMirrored = behaviour.VideoBackGroundMirrored;
            CameraDevice.VideoModeData videoMode = behaviour.GetVideoMode();
            foreach (QCARManagerImpl.WordResultData data2 in wordResults)
            {
                WordResultImpl impl = (WordResultImpl) this.mTrackedWords[data2.id];
                Vector3 position = arCamera.transform.TransformPoint(data2.pose.position);
                Quaternion orientation = data2.pose.orientation;
                Quaternion quaternion2 = (arCamera.transform.rotation * orientation) * Quaternion.AngleAxis(270f, Vector3.left);
                impl.SetPose(position, quaternion2);
                impl.SetStatus(data2.status);
                OrientedBoundingBox cameraFrameObb = new OrientedBoundingBox(data2.orientedBoundingBox.center, data2.orientedBoundingBox.halfExtents, data2.orientedBoundingBox.rotation);
                impl.SetObb(QCARRuntimeUtilities.CameraFrameToScreenSpaceCoordinates(cameraFrameObb, viewportRectangle, videoBackGroundMirrored, videoMode));
            }
            if (this.mWordPrefabCreationMode == WordPrefabCreationMode.DUPLICATE)
            {
                this.UpdateWordBehaviourPoses();
            }
        }
    }

    private void UpdateWords(IEnumerable<QCARManagerImpl.WordData> newWordData, IEnumerable<QCARManagerImpl.WordResultData> wordResults)
    {
        this.mNewWords.Clear();
        this.mLostWords.Clear();
        foreach (QCARManagerImpl.WordData data in newWordData)
        {
            if (!this.mTrackedWords.ContainsKey(data.id))
            {
                string text = Marshal.PtrToStringUni(data.stringValue);
                WordImpl word = new WordImpl(data.id, text, data.size);
                WordResultImpl impl2 = new WordResultImpl(word);
                this.mTrackedWords.Add(data.id, impl2);
                this.mNewWords.Add(impl2);
            }
        }
        List<int> list = new List<int>();
        foreach (QCARManagerImpl.WordResultData data2 in wordResults)
        {
            list.Add(data2.id);
        }
        foreach (int num in this.mTrackedWords.Keys.ToList<int>())
        {
            if (!list.Contains(num))
            {
                this.mLostWords.Add(this.mTrackedWords[num].Word);
                this.mTrackedWords.Remove(num);
            }
        }
        if (this.mWordPrefabCreationMode == WordPrefabCreationMode.DUPLICATE)
        {
            this.UnregisterLostWords();
            this.AssociateWordResultsWithBehaviours();
        }
    }

    internal void UpdateWords(Camera arCamera, QCARManagerImpl.WordData[] newWordData, QCARManagerImpl.WordResultData[] wordResults)
    {
        this.UpdateWords(newWordData, wordResults);
        this.UpdateWordResultPoses(arCamera, wordResults);
    }
}

