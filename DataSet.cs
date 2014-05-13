using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataSet
{
    protected DataSet()
    {
    }

    public abstract bool Contains(Trackable trackable);
    public abstract DataSetTrackableBehaviour CreateTrackable(TrackableSource trackableSource, string gameObjectName);
    public abstract DataSetTrackableBehaviour CreateTrackable(TrackableSource trackableSource, GameObject gameObject);
    public abstract bool Destroy(Trackable trackable, bool destroyGameObject);
    public abstract void DestroyAllTrackables(bool destroyGameObject);
    public static bool Exists(string name)
    {
        return Exists("QCAR/" + name + ".xml", StorageType.STORAGE_APPRESOURCE);
    }

    public static bool Exists(string path, StorageType storageType)
    {
        return DataSetImpl.ExistsImpl(path, storageType);
    }

    public abstract IEnumerable<Trackable> GetTrackables();
    public abstract bool HasReachedTrackableLimit();
    public abstract bool Load(string name);
    public abstract bool Load(string path, StorageType storageType);

    public abstract StorageType FileStorageType { get; }

    public abstract string Path { get; }

    public enum StorageType
    {
        STORAGE_APP,
        STORAGE_APPRESOURCE,
        STORAGE_ABSOLUTE
    }
}

