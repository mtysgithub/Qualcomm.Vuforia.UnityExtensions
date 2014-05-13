using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloudRecoAbstractBehaviour : MonoBehaviour, ITrackerEventHandler
{
    public string AccessKey = "";
    public Color FeaturePointColor = new Color(0.427f, 0.988f, 0.286f);
    private bool mCloudRecoStarted;
    private bool mCurrentlyInitializing;
    private readonly List<ICloudRecoEventHandler> mHandlers = new List<ICloudRecoEventHandler>();
    private ImageTracker mImageTracker;
    private bool mInitSuccess;
    private bool mOnInitializedCalled;
    private bool mTargetFinderStartedBeforeDisable = true;
    public Color ScanlineColor = new Color(1f, 1f, 1f);
    public string SecretKey = "";

    protected CloudRecoAbstractBehaviour()
    {
    }

    private void CheckInitialization()
    {
        TargetFinder.InitState initState = this.mImageTracker.TargetFinder.GetInitState();
        if (initState == TargetFinder.InitState.INIT_SUCCESS)
        {
            foreach (ICloudRecoEventHandler handler in this.mHandlers)
            {
                handler.OnInitialized();
            }
            this.mImageTracker.TargetFinder.SetUIScanlineColor(this.ScanlineColor);
            this.mImageTracker.TargetFinder.SetUIPointColor(this.FeaturePointColor);
            this.mCurrentlyInitializing = false;
            this.mInitSuccess = true;
            this.StartCloudReco();
        }
        else if (initState < TargetFinder.InitState.INIT_DEFAULT)
        {
            foreach (ICloudRecoEventHandler handler2 in this.mHandlers)
            {
                handler2.OnInitError(initState);
            }
            this.mCurrentlyInitializing = false;
        }
    }

    private void Deinitialize()
    {
        this.mCurrentlyInitializing = !this.mImageTracker.TargetFinder.Deinit();
        if (this.mCurrentlyInitializing)
        {
            Debug.LogError("CloudRecoBehaviour: TargetFinder deinitialization failed!");
        }
        else
        {
            this.mInitSuccess = false;
        }
    }

    private void Initialize()
    {
        this.mCurrentlyInitializing = this.mImageTracker.TargetFinder.StartInit(this.AccessKey, this.SecretKey);
        if (!this.mCurrentlyInitializing)
        {
            Debug.LogError("CloudRecoBehaviour: TargetFinder initialization failed!");
        }
    }

    private void OnDestroy()
    {
        if (QCARManager.Instance.Initialized && this.mOnInitializedCalled)
        {
            this.Deinitialize();
        }
        QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (behaviour != null)
        {
            behaviour.UnregisterTrackerEventHandler(this);
        }
    }

    private void OnDisable()
    {
        if (QCARManager.Instance.Initialized && this.mOnInitializedCalled)
        {
            this.mTargetFinderStartedBeforeDisable = this.mCloudRecoStarted;
            this.StopCloudReco();
        }
    }

    private void OnEnable()
    {
        if (this.mOnInitializedCalled && this.mTargetFinderStartedBeforeDisable)
        {
            this.StartCloudReco();
        }
    }

    public void OnInitialized()
    {
        this.mImageTracker = TrackerManager.Instance.GetTracker<ImageTracker>();
        if (this.mImageTracker != null)
        {
            this.Initialize();
        }
        this.mOnInitializedCalled = true;
    }

    public void OnTrackablesUpdated()
    {
    }

    public void RegisterEventHandler(ICloudRecoEventHandler eventHandler)
    {
        this.mHandlers.Add(eventHandler);
        if (this.mOnInitializedCalled)
        {
            eventHandler.OnInitialized();
        }
    }

    private void Start()
    {
        if (KeepAliveAbstractBehaviour.Instance.KeepCloudRecoBehaviourAlive)
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }
        QCARAbstractBehaviour behaviour = (QCARAbstractBehaviour) UnityEngine.Object.FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (behaviour != null)
        {
            behaviour.RegisterTrackerEventHandler(this, false);
        }
    }

    private void StartCloudReco()
    {
        if ((this.mImageTracker != null) && !this.mCloudRecoStarted)
        {
            this.mCloudRecoStarted = this.mImageTracker.TargetFinder.StartRecognition();
            foreach (ICloudRecoEventHandler handler in this.mHandlers)
            {
                handler.OnStateChanged(true);
            }
        }
    }

    private void StopCloudReco()
    {
        if (this.mCloudRecoStarted)
        {
            this.mCloudRecoStarted = !this.mImageTracker.TargetFinder.Stop();
            if (this.mCloudRecoStarted)
            {
                Debug.LogError("Cloud Reco could not be stopped at this point!");
            }
            else
            {
                foreach (ICloudRecoEventHandler handler in this.mHandlers)
                {
                    handler.OnStateChanged(false);
                }
            }
        }
    }

    public bool UnregisterEventHandler(ICloudRecoEventHandler eventHandler)
    {
        return this.mHandlers.Remove(eventHandler);
    }

    private void Update()
    {
        if (this.mOnInitializedCalled)
        {
            if (this.mCurrentlyInitializing)
            {
                this.CheckInitialization();
            }
            else if (this.mInitSuccess)
            {
                TargetFinder.UpdateState updateError = this.mImageTracker.TargetFinder.Update();
                if (updateError == TargetFinder.UpdateState.UPDATE_RESULTS_AVAILABLE)
                {
                    foreach (TargetFinder.TargetSearchResult result in this.mImageTracker.TargetFinder.GetResults())
                    {
                        foreach (ICloudRecoEventHandler handler in this.mHandlers)
                        {
                            handler.OnNewSearchResult(result);
                        }
                    }
                }
                else if (updateError < TargetFinder.UpdateState.UPDATE_NO_MATCH)
                {
                    foreach (ICloudRecoEventHandler handler2 in this.mHandlers)
                    {
                        handler2.OnUpdateError(updateError);
                    }
                }
            }
        }
    }

    public bool CloudRecoEnabled
    {
        get
        {
            return this.mCloudRecoStarted;
        }
        set
        {
            if (value)
            {
                this.StartCloudReco();
            }
            else
            {
                this.StopCloudReco();
            }
        }
    }

    public bool CloudRecoInitialized
    {
        get
        {
            return this.mInitSuccess;
        }
    }
}

