using System;
using UnityEngine;

public abstract class QCARManager
{
    private static QCARManager sInstance;

    protected QCARManager()
    {
    }

    public abstract void Deinit();
    public abstract bool Init();

    public abstract Camera ARCamera { get; set; }

    public abstract bool DrawVideoBackground { get; set; }

    public abstract bool Initialized { get; }

    public static QCARManager Instance
    {
        get
        {
            if (sInstance == null)
            {
                lock (typeof(QCARManager))
                {
                    if (sInstance == null)
                    {
                        sInstance = new QCARManagerImpl();
                    }
                }
            }
            return sInstance;
        }
    }

    public abstract TrackableBehaviour WorldCenter { get; set; }

    public abstract QCARAbstractBehaviour.WorldCenterMode WorldCenterMode { get; set; }
}

