using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class DataSetImpl : DataSet
{
    private IntPtr mDataSetPtr = IntPtr.Zero;
    private string mPath = "";
    private DataSet.StorageType mStorageType = DataSet.StorageType.STORAGE_APPRESOURCE;
    private readonly Dictionary<int, Trackable> mTrackablesDict = new Dictionary<int, Trackable>();

    public DataSetImpl(IntPtr dataSetPtr)
    {
        this.mDataSetPtr = dataSetPtr;
    }

    public override bool Contains(Trackable trackable)
    {
        return this.mTrackablesDict.ContainsValue(trackable);
    }

    private void CreateCylinderTargets()
    {
        int trackableDataArrayLength = QCARWrapper.Instance.DataSetGetNumTrackableType(TypeMapping.GetTypeID(typeof(CylinderTarget)), this.mDataSetPtr);
        if (trackableDataArrayLength > 0)
        {
            IntPtr trackableDataArray = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof(SimpleTargetData)) * trackableDataArrayLength));
            if (QCARWrapper.Instance.DataSetGetTrackablesOfType(TypeMapping.GetTypeID(typeof(CylinderTarget)), trackableDataArray, trackableDataArrayLength, this.mDataSetPtr) == 0)
            {
                Debug.LogError("Could not create Cylinder Targets");
            }
            else
            {
                for (int i = 0; i < trackableDataArrayLength; i++)
                {
                    IntPtr ptr = new IntPtr(trackableDataArray.ToInt32() + (i * Marshal.SizeOf(typeof(SimpleTargetData))));
                    SimpleTargetData data = (SimpleTargetData) Marshal.PtrToStructure(ptr, typeof(SimpleTargetData));
                    if (!this.mTrackablesDict.ContainsKey(data.id))
                    {
                        int capacity = 0x80;
                        StringBuilder trackableName = new StringBuilder(capacity);
                        QCARWrapper.Instance.DataSetGetTrackableName(this.mDataSetPtr, data.id, trackableName, capacity);
                        CylinderTarget target = new CylinderTargetImpl(trackableName.ToString(), data.id, this);
                        this.mTrackablesDict[data.id] = target;
                    }
                }
                Marshal.FreeHGlobal(trackableDataArray);
            }
        }
    }

    private void CreateImageTargets()
    {
        int trackableDataArrayLength = QCARWrapper.Instance.DataSetGetNumTrackableType(TypeMapping.GetTypeID(typeof(ImageTarget)), this.mDataSetPtr);
        IntPtr trackableDataArray = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof(ImageTargetData)) * trackableDataArrayLength));
        if (QCARWrapper.Instance.DataSetGetTrackablesOfType(TypeMapping.GetTypeID(typeof(ImageTarget)), trackableDataArray, trackableDataArrayLength, this.mDataSetPtr) == 0)
        {
            Debug.LogError("Could not create Image Targets");
        }
        else
        {
            for (int i = 0; i < trackableDataArrayLength; i++)
            {
                IntPtr ptr = new IntPtr(trackableDataArray.ToInt32() + (i * Marshal.SizeOf(typeof(ImageTargetData))));
                ImageTargetData data = (ImageTargetData) Marshal.PtrToStructure(ptr, typeof(ImageTargetData));
                if (!this.mTrackablesDict.ContainsKey(data.id))
                {
                    int capacity = 0x80;
                    StringBuilder trackableName = new StringBuilder(capacity);
                    QCARWrapper.Instance.DataSetGetTrackableName(this.mDataSetPtr, data.id, trackableName, capacity);
                    ImageTarget target = new ImageTargetImpl(trackableName.ToString(), data.id, ImageTargetType.PREDEFINED, this);
                    this.mTrackablesDict[data.id] = target;
                }
            }
            Marshal.FreeHGlobal(trackableDataArray);
        }
    }

    private void CreateMultiTargets()
    {
        int trackableDataArrayLength = QCARWrapper.Instance.DataSetGetNumTrackableType(TypeMapping.GetTypeID(typeof(MultiTarget)), this.mDataSetPtr);
        if (trackableDataArrayLength > 0)
        {
            IntPtr trackableDataArray = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof(SimpleTargetData)) * trackableDataArrayLength));
            if (QCARWrapper.Instance.DataSetGetTrackablesOfType(TypeMapping.GetTypeID(typeof(MultiTarget)), trackableDataArray, trackableDataArrayLength, this.mDataSetPtr) == 0)
            {
                Debug.LogError("Could not create Multi Targets");
            }
            else
            {
                for (int i = 0; i < trackableDataArrayLength; i++)
                {
                    IntPtr ptr = new IntPtr(trackableDataArray.ToInt32() + (i * Marshal.SizeOf(typeof(SimpleTargetData))));
                    SimpleTargetData data = (SimpleTargetData) Marshal.PtrToStructure(ptr, typeof(SimpleTargetData));
                    if (!this.mTrackablesDict.ContainsKey(data.id))
                    {
                        int capacity = 0x80;
                        StringBuilder trackableName = new StringBuilder(capacity);
                        QCARWrapper.Instance.DataSetGetTrackableName(this.mDataSetPtr, data.id, trackableName, capacity);
                        MultiTarget target = new MultiTargetImpl(trackableName.ToString(), data.id, this);
                        this.mTrackablesDict[data.id] = target;
                    }
                }
                Marshal.FreeHGlobal(trackableDataArray);
            }
        }
    }

    private void CreateRigidBodyTargets()
    {
        int trackableDataArrayLength = QCARWrapper.Instance.DataSetGetNumTrackableType(TypeMapping.GetTypeID(typeof(InternalRigidBodyTarget)), this.mDataSetPtr);
        if (trackableDataArrayLength > 0)
        {
            IntPtr trackableDataArray = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof(SimpleTargetData)) * trackableDataArrayLength));
            if (QCARWrapper.Instance.DataSetGetTrackablesOfType(TypeMapping.GetTypeID(typeof(InternalRigidBodyTarget)), trackableDataArray, trackableDataArrayLength, this.mDataSetPtr) == 0)
            {
                Debug.LogError("Could not create RigidBody Targets");
            }
            else
            {
                for (int i = 0; i < trackableDataArrayLength; i++)
                {
                    IntPtr ptr = new IntPtr(trackableDataArray.ToInt32() + (i * Marshal.SizeOf(typeof(SimpleTargetData))));
                    SimpleTargetData data = (SimpleTargetData) Marshal.PtrToStructure(ptr, typeof(SimpleTargetData));
                    if (!this.mTrackablesDict.ContainsKey(data.id))
                    {
                        int capacity = 0x80;
                        StringBuilder trackableName = new StringBuilder(capacity);
                        QCARWrapper.Instance.DataSetGetTrackableName(this.mDataSetPtr, data.id, trackableName, capacity);
                        InternalRigidBodyTarget target = PremiumObjectFactory.Instance.CreateRigidBodyTarget(trackableName.ToString(), data.id);
                        this.mTrackablesDict[data.id] = target;
                    }
                }
                Marshal.FreeHGlobal(trackableDataArray);
            }
        }
    }

    public override DataSetTrackableBehaviour CreateTrackable(TrackableSource trackableSource, string gameObjectName)
    {
        GameObject gameObject = new GameObject(gameObjectName);
        return this.CreateTrackable(trackableSource, gameObject);
    }

    public override DataSetTrackableBehaviour CreateTrackable(TrackableSource trackableSource, GameObject gameObject)
    {
        TrackableSourceImpl impl = (TrackableSourceImpl) trackableSource;
        int capacity = 0x80;
        StringBuilder trackableName = new StringBuilder(capacity);
        IntPtr trackableData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SimpleTargetData)));
        int num2 = QCARWrapper.Instance.DataSetCreateTrackable(this.mDataSetPtr, impl.TrackableSourcePtr, trackableName, capacity, trackableData);
        SimpleTargetData data = (SimpleTargetData) Marshal.PtrToStructure(trackableData, typeof(SimpleTargetData));
        Marshal.FreeHGlobal(trackableData);
        if (num2 == TypeMapping.GetTypeID(typeof(ImageTarget)))
        {
            ImageTarget trackable = new ImageTargetImpl(trackableName.ToString(), data.id, ImageTargetType.USER_DEFINED, this);
            this.mTrackablesDict[data.id] = trackable;
            Debug.Log(string.Format("Trackable created: {0}, {1}", num2, trackableName));
            StateManagerImpl stateManager = (StateManagerImpl) TrackerManager.Instance.GetStateManager();
            return stateManager.FindOrCreateImageTargetBehaviourForTrackable(trackable, gameObject, this);
        }
        Debug.LogError("DataSet.CreateTrackable returned unknown or incompatible trackable type!");
        return null;
    }

    public override bool Destroy(Trackable trackable, bool destroyGameObject)
    {
        if (QCARWrapper.Instance.DataSetDestroyTrackable(this.mDataSetPtr, trackable.ID) == 0)
        {
            Debug.LogError("Could not destroy trackable with id " + trackable.ID + ".");
            return false;
        }
        this.mTrackablesDict.Remove(trackable.ID);
        if (destroyGameObject)
        {
            TrackerManager.Instance.GetStateManager().DestroyTrackableBehavioursForTrackable(trackable, true);
        }
        return true;
    }

    public override void DestroyAllTrackables(bool destroyGameObject)
    {
        List<Trackable> list = new List<Trackable>(this.mTrackablesDict.Values);
        foreach (Trackable trackable in list)
        {
            this.Destroy(trackable, destroyGameObject);
        }
    }

    internal static bool ExistsImpl(string path, DataSet.StorageType storageType)
    {
        if ((storageType == DataSet.StorageType.STORAGE_APPRESOURCE) && QCARRuntimeUtilities.IsPlayMode())
        {
            path = "Assets/StreamingAssets/" + path;
        }
        return (QCARWrapper.Instance.DataSetExists(path, (int) storageType) == 1);
    }

    public override IEnumerable<Trackable> GetTrackables()
    {
        return this.mTrackablesDict.Values;
    }

    public override bool HasReachedTrackableLimit()
    {
        return (QCARWrapper.Instance.DataSetHasReachedTrackableLimit(this.mDataSetPtr) == 1);
    }

    public override bool Load(string name)
    {
        string path = "QCAR/" + name + ".xml";
        return this.Load(path, DataSet.StorageType.STORAGE_APPRESOURCE);
    }

    public override bool Load(string path, DataSet.StorageType storageType)
    {
        if (this.mDataSetPtr == IntPtr.Zero)
        {
            Debug.LogError("Called Load without a data set object");
            return false;
        }
        string relativePath = path;
        if ((storageType == DataSet.StorageType.STORAGE_APPRESOURCE) && QCARRuntimeUtilities.IsPlayMode())
        {
            relativePath = "Assets/StreamingAssets/" + relativePath;
        }
        if (QCARWrapper.Instance.DataSetLoad(relativePath, (int) storageType, this.mDataSetPtr) == 0)
        {
            Debug.LogError("Did not load: " + path);
            return false;
        }
        this.mPath = path;
        this.mStorageType = storageType;
        this.CreateImageTargets();
        this.CreateMultiTargets();
        this.CreateCylinderTargets();
        this.CreateRigidBodyTargets();
        ((StateManagerImpl) TrackerManager.Instance.GetStateManager()).AssociateTrackableBehavioursForDataSet(this);
        return true;
    }

    public IntPtr DataSetPtr
    {
        get
        {
            return this.mDataSetPtr;
        }
    }

    public override DataSet.StorageType FileStorageType
    {
        get
        {
            return this.mStorageType;
        }
    }

    public override string Path
    {
        get
        {
            return this.mPath;
        }
    }
}

