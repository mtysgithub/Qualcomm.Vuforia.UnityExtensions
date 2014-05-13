using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UserDefinedTargetBuildingAbstractBehaviour : MonoBehaviour, ITrackerEventHandler
{
    private bool mCurrentlyBuilding;
    private bool mCurrentlyScanning;
    private readonly List<IUserDefinedTargetEventHandler> mHandlers = new List<IUserDefinedTargetEventHandler>();
    private ImageTracker mImageTracker;
    private ImageTargetBuilder.FrameQuality mLastFrameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE;
    private bool mOnInitializedCalled;
    private bool mWasBuildingBeforeDisable;
    private bool mWasScanningBeforeDisable;
    public bool StartScanningAutomatically;
    public bool StopScanningWhenFinshedBuilding;
    public bool StopTrackerWhileScanning;

    protected UserDefinedTargetBuildingAbstractBehaviour()
    {
    }

    public void BuildNewTarget(string targetName, float sceenSizeWidth)
    {
        this.mCurrentlyBuilding = true;
        this.mImageTracker.ImageTargetBuilder.Build(targetName, sceenSizeWidth);
    }

    private void OnApplicationPause(bool pause)
    {
        if (!QCARRuntimeUtilities.IsPlayMode())
        {
            if (pause)
            {
                this.OnDisable();
            }
            else
            {
                this.OnEnable();
            }
        }
    }

    private void OnDisable()
    {
        if (this.mOnInitializedCalled)
        {
            this.mWasScanningBeforeDisable = this.mCurrentlyScanning;
            this.mWasBuildingBeforeDisable = this.mCurrentlyBuilding;
            if (this.mCurrentlyScanning)
            {
                this.StopScanning();
            }
        }
    }

    private void OnEnable()
    {
        if (this.mOnInitializedCalled)
        {
            this.mCurrentlyScanning = this.mWasScanningBeforeDisable;
            this.mCurrentlyBuilding = this.mWasBuildingBeforeDisable;
            if (this.mWasScanningBeforeDisable)
            {
                this.StartScanning();
            }
        }
    }

    public void OnInitialized()
    {
        this.mOnInitializedCalled = true;
        this.mImageTracker = TrackerManager.Instance.GetTracker<ImageTracker>();
        foreach (IUserDefinedTargetEventHandler handler in this.mHandlers)
        {
            handler.OnInitialized();
        }
        if (this.StartScanningAutomatically)
        {
            this.StartScanning();
        }
    }

    public void OnTrackablesUpdated()
    {
    }

    public void RegisterEventHandler(IUserDefinedTargetEventHandler eventHandler)
    {
        this.mHandlers.Add(eventHandler);
        if (this.mOnInitializedCalled)
        {
            eventHandler.OnInitialized();
        }
    }

    private void SetFrameQuality(ImageTargetBuilder.FrameQuality frameQuality)
    {
        if (frameQuality != this.mLastFrameQuality)
        {
            foreach (IUserDefinedTargetEventHandler handler in this.mHandlers)
            {
                handler.OnFrameQualityChanged(frameQuality);
            }
            this.mLastFrameQuality = frameQuality;
        }
    }

    private void Start()
    {
        if ((KeepAliveAbstractBehaviour.Instance != null) && KeepAliveAbstractBehaviour.Instance.KeepUDTBuildingBehaviourAlive)
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }
        QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (behaviour != null)
        {
            behaviour.RegisterTrackerEventHandler(this, false);
        }
    }

    public void StartScanning()
    {
        if (this.mImageTracker != null)
        {
            if (this.StopTrackerWhileScanning)
            {
                this.mImageTracker.Stop();
            }
            this.mImageTracker.ImageTargetBuilder.StartScan();
            this.mCurrentlyScanning = true;
        }
        this.SetFrameQuality(ImageTargetBuilder.FrameQuality.FRAME_QUALITY_LOW);
    }

    public void StopScanning()
    {
        this.mCurrentlyScanning = false;
        this.mImageTracker.ImageTargetBuilder.StopScan();
        if (this.StopTrackerWhileScanning)
        {
            this.mImageTracker.Start();
        }
        this.SetFrameQuality(ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE);
    }

    public bool UnregisterEventHandler(IUserDefinedTargetEventHandler eventHandler)
    {
        return this.mHandlers.Remove(eventHandler);
    }

    private void Update()
    {
        if (this.mOnInitializedCalled)
        {
            if (this.mCurrentlyScanning)
            {
                this.SetFrameQuality(this.mImageTracker.ImageTargetBuilder.GetFrameQuality());
            }
            if (this.mCurrentlyBuilding)
            {
                TrackableSource trackableSource = this.mImageTracker.ImageTargetBuilder.GetTrackableSource();
                if (trackableSource != null)
                {
                    this.mCurrentlyBuilding = false;
                    foreach (IUserDefinedTargetEventHandler handler in this.mHandlers)
                    {
                        handler.OnNewTrackableSource(trackableSource);
                    }
                    if (this.StopScanningWhenFinshedBuilding)
                    {
                        this.StopScanning();
                    }
                }
            }
        }
    }
}

