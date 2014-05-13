using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(QCARAbstractBehaviour))]
public abstract class KeepAliveAbstractBehaviour : MonoBehaviour
{
    private readonly List<ILoadLevelEventHandler> mHandlers = new List<ILoadLevelEventHandler>();
    [SerializeField, HideInInspector]
    private bool mKeepARCameraAlive;
    [SerializeField, HideInInspector]
    private bool mKeepCloudRecoBehaviourAlive;
    [HideInInspector, SerializeField]
    private bool mKeepTextRecoBehaviourAlive;
    [SerializeField, HideInInspector]
    private bool mKeepTrackableBehavioursAlive;
    [HideInInspector, SerializeField]
    private bool mKeepUDTBuildingBehaviourAlive;
    private static KeepAliveAbstractBehaviour sKeepAliveBehaviour;

    protected KeepAliveAbstractBehaviour()
    {
    }

    private void OnLevelWasLoaded()
    {
        if (this.mKeepARCameraAlive)
        {
            List<TrackableBehaviour> list;
            StateManagerImpl stateManager = (StateManagerImpl) TrackerManager.Instance.GetStateManager();
            if (this.mKeepTrackableBehavioursAlive)
            {
                list = stateManager.GetTrackableBehaviours().ToList<TrackableBehaviour>();
                foreach (WordAbstractBehaviour behaviour in stateManager.GetWordManager().GetTrackableBehaviours())
                {
                    list.Add(behaviour);
                }
            }
            else
            {
                list = new List<TrackableBehaviour>();
            }
            foreach (ILoadLevelEventHandler handler in this.mHandlers)
            {
                handler.OnLevelLoaded(list);
            }
            TrackableBehaviour[] behaviourArray = (TrackableBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(TrackableBehaviour));
            stateManager.RemoveDestroyedTrackables();
            stateManager.AssociateMarkerBehaviours();
            ImageTracker tracker = TrackerManager.Instance.GetTracker<ImageTracker>();
            if (tracker != null)
            {
                IEnumerable<DataSet> dataSets = tracker.GetDataSets();
                List<DataSet> list2 = tracker.GetActiveDataSets().ToList<DataSet>();
                foreach (DataSet set in dataSets)
                {
                    if (list2.Contains(set))
                    {
                        tracker.DeactivateDataSet(set);
                    }
                    stateManager.AssociateTrackableBehavioursForDataSet(set);
                    if (list2.Contains(set))
                    {
                        tracker.ActivateDataSet(set);
                    }
                }
            }
            bool flag = false;
            TextRecoAbstractBehaviour behaviour2 = (TextRecoAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(TextRecoAbstractBehaviour));
            if (behaviour2 != null)
            {
                if (!behaviour2.IsInitialized)
                {
                    flag = true;
                }
                else
                {
                    WordManagerImpl wordManager = (WordManagerImpl) stateManager.GetWordManager();
                    wordManager.RemoveDestroyedTrackables();
                    wordManager.InitializeWordBehaviourTemplates();
                }
            }
            List<TrackableBehaviour> disabledTrackables = new List<TrackableBehaviour>();
            IEnumerable<TrackableBehaviour> trackableBehaviours = stateManager.GetTrackableBehaviours();
            IEnumerable<WordAbstractBehaviour> source = stateManager.GetWordManager().GetTrackableBehaviours();
            foreach (TrackableBehaviour behaviour3 in behaviourArray)
            {
                if (behaviour3 is WordAbstractBehaviour)
                {
                    if (!flag && !source.Contains<WordAbstractBehaviour>((behaviour3 as WordAbstractBehaviour)))
                    {
                        behaviour3.gameObject.SetActive(false);
                        disabledTrackables.Add(behaviour3);
                    }
                }
                else if ((!(behaviour3 is ImageTargetAbstractBehaviour) || (((IEditorImageTargetBehaviour) behaviour3).ImageTargetType == ImageTargetType.PREDEFINED)) && !trackableBehaviours.Contains<TrackableBehaviour>(behaviour3))
                {
                    behaviour3.gameObject.SetActive(false);
                    disabledTrackables.Add(behaviour3);
                }
            }
            foreach (ILoadLevelEventHandler handler2 in this.mHandlers)
            {
                handler2.OnDuplicateTrackablesDisabled(disabledTrackables);
            }
        }
    }

    public void RegisterEventHandler(ILoadLevelEventHandler eventHandler)
    {
        this.mHandlers.Add(eventHandler);
    }

    public bool UnregisterEventHandler(ILoadLevelEventHandler eventHandler)
    {
        return this.mHandlers.Remove(eventHandler);
    }

    public static KeepAliveAbstractBehaviour Instance
    {
        get
        {
            if (sKeepAliveBehaviour == null)
            {
                sKeepAliveBehaviour = (KeepAliveAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(KeepAliveAbstractBehaviour));
            }
            return sKeepAliveBehaviour;
        }
    }

    public bool KeepARCameraAlive
    {
        get
        {
            return this.mKeepARCameraAlive;
        }
        set
        {
            if (!Application.isPlaying)
            {
                this.mKeepARCameraAlive = value;
            }
        }
    }

    public bool KeepCloudRecoBehaviourAlive
    {
        get
        {
            return this.mKeepCloudRecoBehaviourAlive;
        }
        set
        {
            if (!Application.isPlaying)
            {
                this.mKeepCloudRecoBehaviourAlive = value;
            }
        }
    }

    public bool KeepTextRecoBehaviourAlive
    {
        get
        {
            return this.mKeepTextRecoBehaviourAlive;
        }
        set
        {
            if (!Application.isPlaying)
            {
                this.mKeepTextRecoBehaviourAlive = value;
            }
        }
    }

    public bool KeepTrackableBehavioursAlive
    {
        get
        {
            return this.mKeepTrackableBehavioursAlive;
        }
        set
        {
            if (!Application.isPlaying)
            {
                this.mKeepTrackableBehavioursAlive = value;
            }
        }
    }

    public bool KeepUDTBuildingBehaviourAlive
    {
        get
        {
            return this.mKeepUDTBuildingBehaviourAlive;
        }
        set
        {
            if (!Application.isPlaying)
            {
                this.mKeepUDTBuildingBehaviourAlive = value;
            }
        }
    }
}

