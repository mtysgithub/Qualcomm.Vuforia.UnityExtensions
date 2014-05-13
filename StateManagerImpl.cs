using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class StateManagerImpl : StateManager
{
    private readonly List<TrackableBehaviour> mActiveTrackableBehaviours = new List<TrackableBehaviour>();
    private readonly List<int> mAutomaticallyCreatedBehaviours = new List<int>();
    private readonly List<TrackableBehaviour> mBehavioursMarkedForDeletion = new List<TrackableBehaviour>();
    private readonly Dictionary<int, TrackableBehaviour> mTrackableBehaviours = new Dictionary<int, TrackableBehaviour>();
    private readonly WordManagerImpl mWordManager = new WordManagerImpl();

    internal void AssociateMarkerBehaviours()
    {
        MarkerTrackerImpl tracker = (MarkerTrackerImpl) TrackerManager.Instance.GetTracker<MarkerTracker>();
        if (tracker != null)
        {
            MarkerAbstractBehaviour[] behaviourArray = (MarkerAbstractBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(MarkerAbstractBehaviour));
            foreach (MarkerAbstractBehaviour behaviour in behaviourArray)
            {
                if (this.mBehavioursMarkedForDeletion.Contains(behaviour))
                {
                    this.mTrackableBehaviours.Remove(behaviour.Trackable.ID);
                    this.mBehavioursMarkedForDeletion.Remove(behaviour);
                }
                else
                {
                    IEditorMarkerBehaviour behaviour2 = behaviour;
                    Marker markerByMarkerID = tracker.GetMarkerByMarkerID(behaviour2.MarkerID);
                    if (markerByMarkerID != null)
                    {
                        this.InitializeMarkerBehaviour(behaviour, markerByMarkerID);
                    }
                    else
                    {
                        markerByMarkerID = tracker.InternalCreateMarker(behaviour2.MarkerID, behaviour2.TrackableName, behaviour2.transform.localScale.x);
                        if (markerByMarkerID == null)
                        {
                            Debug.LogWarning("Disabling MarkerBehaviour named " + behaviour2.TrackableName);
                            behaviour.enabled = false;
                        }
                        else
                        {
                            this.InitializeMarkerBehaviour(behaviour, markerByMarkerID);
                            behaviour.enabled = true;
                        }
                    }
                }
            }
        }
    }

    internal void AssociateTrackableBehavioursForDataSet(DataSet dataSet)
    {
        DataSetTrackableBehaviour[] behaviourArray = (DataSetTrackableBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(DataSetTrackableBehaviour));
        foreach (DataSetTrackableBehaviour behaviour in behaviourArray)
        {
            if (!this.mBehavioursMarkedForDeletion.Contains(behaviour))
            {
                IEditorDataSetTrackableBehaviour behaviour2 = behaviour;
                if (behaviour2.TrackableName == null)
                {
                    Debug.LogError("Found Trackable without name.");
                }
                else if (behaviour2.DataSetPath.Equals(dataSet.Path))
                {
                    bool flag = false;
                    foreach (Trackable trackable in dataSet.GetTrackables())
                    {
                        if (trackable.Name.Equals(behaviour2.TrackableName))
                        {
                            if (this.mTrackableBehaviours.ContainsKey(trackable.ID))
                            {
                                if (!this.mAutomaticallyCreatedBehaviours.Contains(trackable.ID) && !this.mBehavioursMarkedForDeletion.Contains(this.mTrackableBehaviours[trackable.ID]))
                                {
                                    flag = true;
                                    continue;
                                }
                                UnityEngine.Object.Destroy(this.mTrackableBehaviours[trackable.ID].gameObject);
                                this.mTrackableBehaviours.Remove(trackable.ID);
                                this.mAutomaticallyCreatedBehaviours.Remove(trackable.ID);
                            }
                            if ((behaviour is ImageTargetAbstractBehaviour) && (trackable is ImageTarget))
                            {
                                IEditorImageTargetBehaviour behaviour3 = (ImageTargetAbstractBehaviour) behaviour;
                                flag = true;
                                behaviour3.InitializeImageTarget((ImageTarget) trackable);
                                this.mTrackableBehaviours[trackable.ID] = behaviour;
                                Debug.Log(string.Concat(new object[] { "Found Trackable named ", behaviour.Trackable.Name, " with id ", behaviour.Trackable.ID }));
                            }
                            else if ((behaviour is MultiTargetAbstractBehaviour) && (trackable is MultiTarget))
                            {
                                flag = true;
                                IEditorMultiTargetBehaviour behaviour4 = (MultiTargetAbstractBehaviour) behaviour;
                                behaviour4.InitializeMultiTarget((MultiTarget) trackable);
                                this.mTrackableBehaviours[trackable.ID] = behaviour;
                                Debug.Log(string.Concat(new object[] { "Found Trackable named ", behaviour.Trackable.Name, " with id ", behaviour.Trackable.ID }));
                            }
                            else if ((behaviour is CylinderTargetAbstractBehaviour) && (trackable is CylinderTarget))
                            {
                                flag = true;
                                IEditorCylinderTargetBehaviour behaviour5 = (CylinderTargetAbstractBehaviour) behaviour;
                                behaviour5.InitializeCylinderTarget((CylinderTarget) trackable);
                                this.mTrackableBehaviours[trackable.ID] = behaviour;
                                Debug.Log(string.Concat(new object[] { "Found Trackable named ", behaviour.Trackable.Name, " with id ", behaviour.Trackable.ID }));
                            }
                            else if ((behaviour is IEditorRigidBodyTargetBehaviour) && (trackable is InternalRigidBodyTarget))
                            {
                                flag = true;
                                ((IEditorRigidBodyTargetBehaviour) behaviour).InitializeRigidBodyTarget((InternalRigidBodyTarget) trackable);
                                this.mTrackableBehaviours[trackable.ID] = behaviour;
                                Debug.Log(string.Concat(new object[] { "Found Trackable named ", behaviour.Trackable.Name, " with id ", behaviour.Trackable.ID }));
                            }
                        }
                    }
                    if (!flag)
                    {
                        Debug.LogError("Could not associate DataSetTrackableBehaviour '" + behaviour2.TrackableName + "' - no matching Trackable found in DataSet!");
                    }
                }
            }
        }
        VirtualButtonAbstractBehaviour[] vbBehaviours = (VirtualButtonAbstractBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(VirtualButtonAbstractBehaviour));
        this.AssociateVirtualButtonBehaviours(vbBehaviours, dataSet);
        this.CreateMissingDataSetTrackableBehaviours(dataSet);
    }

    private void AssociateVirtualButtonBehaviours(VirtualButtonAbstractBehaviour[] vbBehaviours, DataSet dataSet)
    {
        for (int i = 0; i < vbBehaviours.Length; i++)
        {
            VirtualButtonAbstractBehaviour virtualButtonBehaviour = vbBehaviours[i];
            if (virtualButtonBehaviour.VirtualButtonName == null)
            {
                Debug.LogError("VirtualButton at " + i + " has no name.");
            }
            else
            {
                ImageTargetAbstractBehaviour imageTargetBehaviour = virtualButtonBehaviour.GetImageTargetBehaviour();
                if (imageTargetBehaviour == null)
                {
                    Debug.LogError("VirtualButton named " + virtualButtonBehaviour.VirtualButtonName + " is not attached to an ImageTarget.");
                }
                else if (dataSet.Contains(imageTargetBehaviour.Trackable))
                {
                    ((IEditorImageTargetBehaviour) imageTargetBehaviour).AssociateExistingVirtualButtonBehaviour(virtualButtonBehaviour);
                }
            }
        }
    }

    internal void ClearTrackableBehaviours()
    {
        this.mTrackableBehaviours.Clear();
        this.mActiveTrackableBehaviours.Clear();
        this.mAutomaticallyCreatedBehaviours.Clear();
        this.mBehavioursMarkedForDeletion.Clear();
    }

    private CylinderTargetAbstractBehaviour CreateCylinderTargetBehaviour(CylinderTarget cylinderTarget)
    {
        GameObject gameObject = new GameObject();
        CylinderTargetAbstractBehaviour behaviour = BehaviourComponentFactory.Instance.AddCylinderTargetBehaviour(gameObject);
        IEditorCylinderTargetBehaviour behaviour2 = behaviour;
        Debug.Log(string.Concat(new object[] { "Creating Cylinder Target with values: \n ID:           ", cylinderTarget.ID, "\n Name:         ", cylinderTarget.Name, "\n Path:         ", behaviour2.DataSetPath, "\n Side Length:  ", cylinderTarget.GetSideLength(), "\n Top Diameter: ", cylinderTarget.GetTopDiameter(), "\n Bottom Diam.: ", cylinderTarget.GetBottomDiameter() }));
        behaviour2.SetNameForTrackable(cylinderTarget.Name);
        behaviour2.SetDataSetPath(behaviour2.DataSetPath);
        float sideLength = cylinderTarget.GetSideLength();
        behaviour2.transform.localScale = new Vector3(sideLength, sideLength, sideLength);
        behaviour2.CorrectScale();
        behaviour2.SetAspectRatio(cylinderTarget.GetTopDiameter() / sideLength, cylinderTarget.GetBottomDiameter() / sideLength);
        behaviour2.InitializeCylinderTarget(cylinderTarget);
        return behaviour;
    }

    private ImageTargetAbstractBehaviour CreateImageTargetBehaviour(ImageTarget imageTarget)
    {
        GameObject gameObject = new GameObject();
        ImageTargetAbstractBehaviour behaviour = BehaviourComponentFactory.Instance.AddImageTargetBehaviour(gameObject);
        IEditorImageTargetBehaviour behaviour2 = behaviour;
        Debug.Log(string.Concat(new object[] { "Creating Image Target with values: \n ID:           ", imageTarget.ID, "\n Name:         ", imageTarget.Name, "\n Path:         ", behaviour2.DataSetPath, "\n Size:         ", imageTarget.GetSize().x, "x", imageTarget.GetSize().y }));
        behaviour2.SetNameForTrackable(imageTarget.Name);
        behaviour2.SetDataSetPath(behaviour2.DataSetPath);
        Vector2 size = imageTarget.GetSize();
        float x = Mathf.Max(size.x, size.y);
        behaviour2.transform.localScale = new Vector3(x, x, x);
        behaviour2.CorrectScale();
        behaviour2.SetAspectRatio(size.y / size.x);
        behaviour2.InitializeImageTarget(imageTarget);
        return behaviour;
    }

    private void CreateMissingDataSetTrackableBehaviours(DataSet dataSet)
    {
        foreach (Trackable trackable in dataSet.GetTrackables())
        {
            if (!this.mTrackableBehaviours.ContainsKey(trackable.ID))
            {
                if (trackable is ImageTarget)
                {
                    ImageTargetAbstractBehaviour behaviour = this.CreateImageTargetBehaviour((ImageTarget) trackable);
                    ((IEditorImageTargetBehaviour) behaviour).CreateMissingVirtualButtonBehaviours();
                    this.mTrackableBehaviours[trackable.ID] = behaviour;
                    this.mAutomaticallyCreatedBehaviours.Add(trackable.ID);
                }
                else if (trackable is MultiTarget)
                {
                    MultiTargetAbstractBehaviour behaviour2 = this.CreateMultiTargetBehaviour((MultiTarget) trackable);
                    this.mTrackableBehaviours[trackable.ID] = behaviour2;
                    this.mAutomaticallyCreatedBehaviours.Add(trackable.ID);
                }
                else if (trackable is CylinderTarget)
                {
                    CylinderTargetAbstractBehaviour behaviour3 = this.CreateCylinderTargetBehaviour((CylinderTarget) trackable);
                    this.mTrackableBehaviours[trackable.ID] = behaviour3;
                    this.mAutomaticallyCreatedBehaviours.Add(trackable.ID);
                }
            }
        }
    }

    private MultiTargetAbstractBehaviour CreateMultiTargetBehaviour(MultiTarget multiTarget)
    {
        GameObject gameObject = new GameObject();
        MultiTargetAbstractBehaviour behaviour = BehaviourComponentFactory.Instance.AddMultiTargetBehaviour(gameObject);
        IEditorMultiTargetBehaviour behaviour2 = behaviour;
        Debug.Log(string.Concat(new object[] { "Creating Multi Target with values: \n ID:           ", multiTarget.ID, "\n Name:         ", multiTarget.Name, "\n Path:         ", behaviour2.DataSetPath }));
        behaviour2.SetNameForTrackable(multiTarget.Name);
        behaviour2.SetDataSetPath(behaviour2.DataSetPath);
        behaviour2.InitializeMultiTarget(multiTarget);
        return behaviour;
    }

    internal MarkerAbstractBehaviour CreateNewMarkerBehaviourForMarker(Marker trackable, string gameObjectName)
    {
        GameObject gameObject = new GameObject(gameObjectName);
        return this.CreateNewMarkerBehaviourForMarker(trackable, gameObject);
    }

    internal MarkerAbstractBehaviour CreateNewMarkerBehaviourForMarker(Marker trackable, GameObject gameObject)
    {
        MarkerAbstractBehaviour behaviour = BehaviourComponentFactory.Instance.AddMarkerBehaviour(gameObject);
        float size = trackable.GetSize();
        Debug.Log(string.Concat(new object[] { "Creating Marker with values: \n MarkerID:     ", trackable.MarkerID, "\n TrackableID:  ", trackable.ID, "\n Name:         ", trackable.Name, "\n Size:         ", size, "x", size }));
        IEditorMarkerBehaviour behaviour2 = behaviour;
        behaviour2.SetMarkerID(trackable.MarkerID);
        behaviour2.SetNameForTrackable(trackable.Name);
        behaviour2.transform.localScale = new Vector3(size, size, size);
        behaviour2.InitializeMarker(trackable);
        this.mTrackableBehaviours[trackable.ID] = behaviour;
        return behaviour;
    }

    public override void DestroyTrackableBehavioursForTrackable(Trackable trackable, [Optional, DefaultParameterValue(true)] bool destroyGameObjects)
    {
        TrackableBehaviour behaviour;
        if (this.mTrackableBehaviours.TryGetValue(trackable.ID, out behaviour))
        {
            if (destroyGameObjects)
            {
                this.mBehavioursMarkedForDeletion.Add(this.mTrackableBehaviours[trackable.ID]);
                UnityEngine.Object.Destroy(behaviour.gameObject);
            }
            else
            {
                IEditorTrackableBehaviour behaviour2 = behaviour;
                behaviour2.UnregisterTrackable();
            }
            this.mTrackableBehaviours.Remove(trackable.ID);
            this.mAutomaticallyCreatedBehaviours.Remove(trackable.ID);
        }
    }

    internal void EnableTrackableBehavioursForTrackable(Trackable trackable, bool enabled)
    {
        TrackableBehaviour behaviour;
        if (this.mTrackableBehaviours.TryGetValue(trackable.ID, out behaviour) && (behaviour != null))
        {
            behaviour.enabled = enabled;
        }
    }

    internal ImageTargetAbstractBehaviour FindOrCreateImageTargetBehaviourForTrackable(ImageTarget trackable, GameObject gameObject)
    {
        return this.FindOrCreateImageTargetBehaviourForTrackable(trackable, gameObject, null);
    }

    internal ImageTargetAbstractBehaviour FindOrCreateImageTargetBehaviourForTrackable(ImageTarget trackable, GameObject gameObject, DataSet dataSet)
    {
        DataSetTrackableBehaviour component = gameObject.GetComponent<DataSetTrackableBehaviour>();
        if (component == null)
        {
            component = BehaviourComponentFactory.Instance.AddImageTargetBehaviour(gameObject);
            ((IEditorTrackableBehaviour) component).SetInitializedInEditor(true);
        }
        if (!(component is ImageTargetAbstractBehaviour))
        {
            Debug.LogError(string.Format("DataSet.CreateTrackable: Trackable of type ImageTarget was created, but behaviour of type {0} was provided!", component.GetType()));
            return null;
        }
        IEditorImageTargetBehaviour behaviour2 = (ImageTargetAbstractBehaviour) component;
        if (dataSet != null)
        {
            behaviour2.SetDataSetPath(dataSet.Path);
        }
        behaviour2.SetImageTargetType(trackable.ImageTargetType);
        behaviour2.SetNameForTrackable(trackable.Name);
        behaviour2.InitializeImageTarget(trackable);
        this.mTrackableBehaviours[trackable.ID] = component;
        return (component as ImageTargetAbstractBehaviour);
    }

    public override IEnumerable<TrackableBehaviour> GetActiveTrackableBehaviours()
    {
        return this.mActiveTrackableBehaviours;
    }

    public override IEnumerable<TrackableBehaviour> GetTrackableBehaviours()
    {
        return this.mTrackableBehaviours.Values;
    }

    public override WordManager GetWordManager()
    {
        return this.mWordManager;
    }

    private void InitializeMarkerBehaviour(MarkerAbstractBehaviour markerBehaviour, Marker marker)
    {
        IEditorMarkerBehaviour behaviour = markerBehaviour;
        behaviour.InitializeMarker(marker);
        if (!this.mTrackableBehaviours.ContainsKey(marker.ID))
        {
            this.mTrackableBehaviours[marker.ID] = markerBehaviour;
            Debug.Log(string.Concat(new object[] { "Found Marker named ", marker.Name, " with id ", marker.ID }));
        }
    }

    private void PositionCamera(TrackableBehaviour trackableBehaviour, Camera arCamera, QCARManagerImpl.PoseData camToTargetPose)
    {
        arCamera.transform.localPosition = ((Vector3) (((trackableBehaviour.transform.rotation * Quaternion.AngleAxis(90f, Vector3.left)) * Quaternion.Inverse(camToTargetPose.orientation)) * -camToTargetPose.position)) + trackableBehaviour.transform.position;
        arCamera.transform.rotation = (trackableBehaviour.transform.rotation * Quaternion.AngleAxis(90f, Vector3.left)) * Quaternion.Inverse(camToTargetPose.orientation);
    }

    private void PositionTrackable(TrackableBehaviour trackableBehaviour, Camera arCamera, QCARManagerImpl.PoseData camToTargetPose)
    {
        trackableBehaviour.transform.position = arCamera.transform.TransformPoint(camToTargetPose.position);
        trackableBehaviour.transform.rotation = (arCamera.transform.rotation * camToTargetPose.orientation) * Quaternion.AngleAxis(270f, Vector3.left);
    }

    internal void RemoveDestroyedTrackables()
    {
        foreach (int num in this.mTrackableBehaviours.Keys.ToArray<int>())
        {
            if (this.mTrackableBehaviours[num] == null)
            {
                this.mTrackableBehaviours.Remove(num);
                this.mAutomaticallyCreatedBehaviours.Remove(num);
            }
        }
    }

    internal void RemoveDisabledTrackablesFromQueue(ref LinkedList<int> trackableIDs)
    {
        LinkedListNode<int> next;
        for (LinkedListNode<int> node = trackableIDs.First; node != null; node = next)
        {
            TrackableBehaviour behaviour;
            next = node.Next;
            if (this.mTrackableBehaviours.TryGetValue(node.Value, out behaviour) && !behaviour.enabled)
            {
                trackableIDs.Remove(node);
            }
        }
    }

    internal void SetTrackableBehavioursForTrackableToNotFound(Trackable trackable)
    {
        TrackableBehaviour behaviour;
        if (this.mTrackableBehaviours.TryGetValue(trackable.ID, out behaviour))
        {
            behaviour.OnTrackerUpdate(TrackableBehaviour.Status.NOT_FOUND);
        }
    }

    internal void UpdateCameraPose(Camera arCamera, QCARManagerImpl.TrackableResultData[] trackableResultDataArray, int originTrackableID)
    {
        if (originTrackableID >= 0)
        {
            foreach (QCARManagerImpl.TrackableResultData data in trackableResultDataArray)
            {
                if (data.id == originTrackableID)
                {
                    TrackableBehaviour behaviour;
                    if ((((data.status == TrackableBehaviour.Status.DETECTED) || (data.status == TrackableBehaviour.Status.TRACKED)) || (data.status == TrackableBehaviour.Status.EXTENDED_TRACKED)) && (this.mTrackableBehaviours.TryGetValue(originTrackableID, out behaviour) && behaviour.enabled))
                    {
                        this.PositionCamera(behaviour, arCamera, data.pose);
                        return;
                    }
                    break;
                }
            }
        }
    }

    internal void UpdateTrackablePoses(Camera arCamera, QCARManagerImpl.TrackableResultData[] trackableResultDataArray, int originTrackableID, int frameIndex)
    {
        Dictionary<int, QCARManagerImpl.TrackableResultData> dictionary = new Dictionary<int, QCARManagerImpl.TrackableResultData>();
        foreach (QCARManagerImpl.TrackableResultData data in trackableResultDataArray)
        {
            TrackableBehaviour behaviour;
            dictionary.Add(data.id, data);
            if (((this.mTrackableBehaviours.TryGetValue(data.id, out behaviour) && (data.id != originTrackableID)) && (((data.status == TrackableBehaviour.Status.DETECTED) || (data.status == TrackableBehaviour.Status.TRACKED)) || (data.status == TrackableBehaviour.Status.EXTENDED_TRACKED))) && behaviour.enabled)
            {
                this.PositionTrackable(behaviour, arCamera, data.pose);
            }
        }
        this.mActiveTrackableBehaviours.Clear();
        foreach (TrackableBehaviour behaviour2 in this.mTrackableBehaviours.Values)
        {
            if (behaviour2.enabled)
            {
                QCARManagerImpl.TrackableResultData data2;
                if (dictionary.TryGetValue(behaviour2.Trackable.ID, out data2))
                {
                    behaviour2.OnTrackerUpdate(data2.status);
                    behaviour2.OnFrameIndexUpdate(frameIndex);
                }
                else
                {
                    behaviour2.OnTrackerUpdate(TrackableBehaviour.Status.NOT_FOUND);
                }
                if (((behaviour2.CurrentStatus == TrackableBehaviour.Status.TRACKED) || (behaviour2.CurrentStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)) || (behaviour2.CurrentStatus == TrackableBehaviour.Status.DETECTED))
                {
                    this.mActiveTrackableBehaviours.Add(behaviour2);
                }
            }
        }
    }

    internal void UpdateVirtualButtons(int numVirtualButtons, IntPtr virtualButtonPtr)
    {
        Dictionary<int, QCARManagerImpl.VirtualButtonData> dictionary = new Dictionary<int, QCARManagerImpl.VirtualButtonData>();
        for (int i = 0; i < numVirtualButtons; i++)
        {
            IntPtr ptr = new IntPtr(virtualButtonPtr.ToInt32() + (i * Marshal.SizeOf(typeof(QCARManagerImpl.VirtualButtonData))));
            QCARManagerImpl.VirtualButtonData data = (QCARManagerImpl.VirtualButtonData) Marshal.PtrToStructure(ptr, typeof(QCARManagerImpl.VirtualButtonData));
            dictionary.Add(data.id, data);
        }
        List<VirtualButtonAbstractBehaviour> list = new List<VirtualButtonAbstractBehaviour>();
        foreach (TrackableBehaviour behaviour in this.mTrackableBehaviours.Values)
        {
            ImageTargetAbstractBehaviour behaviour2 = behaviour as ImageTargetAbstractBehaviour;
            if ((behaviour2 != null) && behaviour2.enabled)
            {
                foreach (VirtualButtonAbstractBehaviour behaviour3 in behaviour2.GetVirtualButtonBehaviours())
                {
                    if (behaviour3.enabled)
                    {
                        list.Add(behaviour3);
                    }
                }
            }
        }
        foreach (VirtualButtonAbstractBehaviour behaviour4 in list)
        {
            QCARManagerImpl.VirtualButtonData data2;
            if (dictionary.TryGetValue(behaviour4.VirtualButton.ID, out data2))
            {
                behaviour4.OnTrackerUpdated(data2.isPressed > 0);
            }
            else
            {
                behaviour4.OnTrackerUpdated(false);
            }
        }
    }

    internal void UpdateWords(Camera arCamera, QCARManagerImpl.WordData[] wordData, QCARManagerImpl.WordResultData[] wordResultData)
    {
        this.mWordManager.UpdateWords(arCamera, wordData, wordResultData);
    }
}

