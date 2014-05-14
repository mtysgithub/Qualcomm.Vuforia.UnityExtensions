using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public abstract class QCARAbstractBehaviour : MonoBehaviour
{
    [SerializeField]
    private CameraDevice.CameraDeviceMode CameraDeviceModeSetting = CameraDevice.CameraDeviceMode.MODE_DEFAULT;
    [SerializeField]
    private CameraDevice.CameraDirection CameraDirection;
    protected IAndroidUnityPlayer mAndroidUnityPlayer;
    [SerializeField]
    private int MaxSimultaneousImageTargets = 1;
    private Color mCachedCameraBackgroundColor;
    private CameraClearFlags mCachedCameraClearFlags;
    private bool mCachedDrawVideoBackground;
    private CameraState mCameraState;
    private int mClearBuffers;
    private Material mClearMaterial;
    private bool mHasStartedOnce;
    [SerializeField]
    private QCARRenderer.VideoBackgroundReflection MirrorVideoBackground;
    private bool mIsInitialized;
    private ScreenOrientation mProjectionOrientation;
    private List<ITrackerEventHandler> mTrackerEventHandlers = new List<ITrackerEventHandler>();
    private List<IVideoBackgroundEventHandler> mVideoBgEventHandlers = new List<IVideoBackgroundEventHandler>();
    private CameraDevice.VideoModeData mVideoMode;
    private Rect mViewportRect;
    [SerializeField, HideInInspector]
    private TrackableBehaviour mWorldCenter;
    [HideInInspector, SerializeField]
    private WorldCenterMode mWorldCenterMode = WorldCenterMode.FIRST_TARGET;
    [SerializeField]
    private bool SynchronousVideo;

    protected QCARAbstractBehaviour()
    {
    }

    private void Awake()
    {
        base.gameObject.AddComponent("ComponentFactoryStarterBehaviour");
        base.gameObject.AddComponent("PremiumObjectFactoryStarterBehaviour");
        base.gameObject.AddComponent("PremiumComponentFactoryStarterBehaviour");
    }

    public void ConfigureVideoBackground(bool forceReflectionSetting)
    {
        QCARRenderer.VideoBGCfgData videoBackgroundConfig = QCARRenderer.Instance.GetVideoBackgroundConfig();
        CameraDevice.VideoModeData videoMode = CameraDevice.Instance.GetVideoMode(this.CameraDeviceModeSetting);
        this.VideoBackGroundMirrored = videoBackgroundConfig.reflection == QCARRenderer.VideoBackgroundReflection.ON;
        videoBackgroundConfig.enabled = 1;
        videoBackgroundConfig.synchronous = this.SynchronousVideo ? 1 : 0;
        videoBackgroundConfig.position = new QCARRenderer.Vec2I(0, 0);
        if (!QCARRuntimeUtilities.IsPlayMode() && forceReflectionSetting)
        {
            videoBackgroundConfig.reflection = this.MirrorVideoBackground;
        }
        bool flag = Screen.width > Screen.height;
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            flag = true;
        }
        if (flag)
        {
            float num = videoMode.height * (((float) Screen.width) / ((float) videoMode.width));
            videoBackgroundConfig.size = new QCARRenderer.Vec2I(Screen.width, (int) num);
            if (videoBackgroundConfig.size.y < Screen.height)
            {
                videoBackgroundConfig.size.x = (int) (Screen.height * (((float) videoMode.width) / ((float) videoMode.height)));
                videoBackgroundConfig.size.y = Screen.height;
            }
        }
        else
        {
            float num2 = videoMode.height * (((float) Screen.height) / ((float) videoMode.width));
            videoBackgroundConfig.size = new QCARRenderer.Vec2I((int) num2, Screen.height);
            if (videoBackgroundConfig.size.x < Screen.width)
            {
                videoBackgroundConfig.size.x = Screen.width;
                videoBackgroundConfig.size.y = (int) (Screen.width * (((float) videoMode.width) / ((float) videoMode.height)));
            }
        }
        QCARRenderer.Instance.SetVideoBackgroundConfig(videoBackgroundConfig);
        int num3 = videoBackgroundConfig.position.x + ((Screen.width - videoBackgroundConfig.size.x) / 2);
        int num4 = videoBackgroundConfig.position.y + ((Screen.height - videoBackgroundConfig.size.y) / 2);
        this.mViewportRect = new Rect((float) num3, (float) num4, (float) videoBackgroundConfig.size.x, (float) videoBackgroundConfig.size.y);
        foreach (IVideoBackgroundEventHandler handler in this.mVideoBgEventHandlers)
        {
            handler.OnVideoBackgroundConfigChanged();
        }
    }

    public ScreenOrientation GetSurfaceOrientation()
    {
        return QCARRuntimeUtilities.ScreenOrientation;
    }

    public CameraDevice.VideoModeData GetVideoMode()
    {
        return this.mVideoMode;
    }

    public Rect GetViewportRectangle()
    {
        return this.mViewportRect;
    }

    private void OnApplicationPause(bool pause)
    {
        if (!QCARRuntimeUtilities.IsPlayMode())
        {
            if (pause)
            {
                this.StopQCAR();
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    GL.Clear(false, true, new Color(0f, 0f, 0f, 1f));
                }
            }
            else
            {
                this.StartQCAR();
                this.ResetClearBuffers();
            }
        }
    }

    private void OnDestroy()
    {
        ((StateManagerImpl) TrackerManager.Instance.GetStateManager()).ClearTrackableBehaviours();
        ImageTracker tracker = TrackerManager.Instance.GetTracker<ImageTracker>();
        if (tracker != null)
        {
            tracker.DestroyAllDataSets(false);
            tracker.Stop();
        }
        MarkerTracker tracker2 = TrackerManager.Instance.GetTracker<MarkerTracker>();
        if (tracker2 != null)
        {
            tracker2.DestroyAllMarkers(false);
            tracker2.Stop();
        }
        TextTracker tracker3 = TrackerManager.Instance.GetTracker<TextTracker>();
        if (tracker3 != null)
        {
            tracker3.Stop();
        }
        QCARManager.Instance.Deinit();
        if (tracker2 != null)
        {
            TrackerManager.Instance.DeinitTracker<MarkerTracker>();
        }
        if (tracker != null)
        {
            TrackerManager.Instance.DeinitTracker<ImageTracker>();
        }
        if (tracker3 != null)
        {
            TrackerManager.Instance.DeinitTracker<TextTracker>();
        }
        if (QCARRuntimeUtilities.IsPlayMode())
        {
            QCARWrapper.Instance.QcarDeinit();
        }
        if (this.mAndroidUnityPlayer != null)
        {
            this.mAndroidUnityPlayer.Dispose();
        }
    }

    private void OnDisable()
    {
        this.StopQCAR();
        this.ResetCameraClearFlags();
    }

    private void OnEnable()
    {
        if (QCARManager.Instance.Initialized && this.mHasStartedOnce)
        {
            this.StartQCAR();
        }
    }

    private void OnPostRender()
    {
        ((QCARManagerImpl) QCARManager.Instance).FinishRendering();
        if (this.mClearBuffers > 0)
        {
            GL.Clear(false, true, new Color(0f, 0f, 0f, 1f));
            this.mClearBuffers--;
        }
    }

    private void OnPreCull()
    {
        ((QCARManagerImpl) QCARManager.Instance).PrepareRendering();
    }

    private void OnPreRender()
    {
        GL.SetRevertBackfacing(this.VideoBackGroundMirrored);
    }

    public void RegisterTrackerEventHandler(ITrackerEventHandler trackerEventHandler, [Optional, DefaultParameterValue(false)] bool hasPriority)
    {
        if (hasPriority)
        {
            this.mTrackerEventHandlers.Insert(0, trackerEventHandler);
        }
        else
        {
            this.mTrackerEventHandlers.Add(trackerEventHandler);
        }
        if (this.mIsInitialized)
        {
            trackerEventHandler.OnInitialized();
        }
    }

    public void RegisterVideoBgEventHandler(IVideoBackgroundEventHandler videoBgEventHandler)
    {
        this.mVideoBgEventHandlers.Add(videoBgEventHandler);
    }

    private void ResetCameraClearFlags()
    {
        this.mCameraState = CameraState.UNINITED;
        this.mClearBuffers = 12;
        base.camera.clearFlags = this.mCachedCameraClearFlags;
        base.camera.backgroundColor = this.mCachedCameraBackgroundColor;
    }

    public void ResetClearBuffers()
    {
        this.mClearBuffers = 12;
    }

    public void SetWorldCenter(TrackableBehaviour value)
    {
        if (!Application.isPlaying)
        {
            this.mWorldCenter = value;
        }
    }

    public void SetWorldCenterMode(WorldCenterMode value)
    {
        if (!Application.isPlaying)
        {
            this.mWorldCenterMode = value;
        }
    }

    private void Start()
    {
        if ((KeepAliveAbstractBehaviour.Instance != null) && KeepAliveAbstractBehaviour.Instance.KeepARCameraAlive)
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }
        Debug.Log("QCARWrapper.Start");
        if (this.mAndroidUnityPlayer != null)
        {
            this.mAndroidUnityPlayer.Start();
        }
        if (QCARUnity.CheckInitializationError() != QCARUnity.InitError.INIT_SUCCESS)
        {
            this.mIsInitialized = false;
        }
        else
        {
            if (TrackerManager.Instance.GetTracker<MarkerTracker>() == null)
            {
                TrackerManager.Instance.InitTracker<MarkerTracker>();
            }
            if (TrackerManager.Instance.GetTracker<ImageTracker>() == null)
            {
                TrackerManager.Instance.InitTracker<ImageTracker>();
            }
            this.mCachedDrawVideoBackground = QCARManager.Instance.DrawVideoBackground;
            this.mCachedCameraClearFlags = base.camera.clearFlags;
            this.mCachedCameraBackgroundColor = base.camera.backgroundColor;
            Screen.sleepTimeout = -1;
            this.ResetCameraClearFlags();
            this.mClearMaterial = new Material(Shader.Find("Diffuse"));
            QCARUnity.SetHint(QCARUnity.QCARHint.HINT_MAX_SIMULTANEOUS_IMAGE_TARGETS, this.MaxSimultaneousImageTargets);
            QCARUnityImpl.SetUnityVersion(Application.persistentDataPath, true);
            ((StateManagerImpl) TrackerManager.Instance.GetStateManager()).AssociateMarkerBehaviours();
            this.StartQCAR();
            QCARManager.Instance.WorldCenterMode = this.mWorldCenterMode;
            QCARManager.Instance.WorldCenter = this.mWorldCenter;
            QCARManager.Instance.ARCamera = base.camera;
            QCARManager.Instance.Init();
            this.mIsInitialized = true;
            foreach (ITrackerEventHandler handler in this.mTrackerEventHandlers)
            {
                handler.OnInitialized();
            }
            this.mHasStartedOnce = true;
            if (QCARRuntimeUtilities.IsPlayMode())
            {
                this.UpdateProjection(ScreenOrientation.Portrait);
            }
        }
    }

    private void StartQCAR()
    {
        Debug.Log("StartQCAR");
        CameraDevice.Instance.Init(this.CameraDirection);
        CameraDevice.Instance.SelectVideoMode(this.CameraDeviceModeSetting);
        CameraDevice.Instance.Start();
        MarkerTracker tracker = TrackerManager.Instance.GetTracker<MarkerTracker>();
        if (tracker != null)
        {
            tracker.Start();
        }
        ImageTracker tracker2 = TrackerManager.Instance.GetTracker<ImageTracker>();
        if (tracker2 != null)
        {
            tracker2.Start();
        }
        ScreenOrientation surfaceOrientation = (ScreenOrientation) QCARWrapper.Instance.GetSurfaceOrientation();
        this.UpdateProjection(surfaceOrientation);
    }

    private void StopQCAR()
    {
        Debug.Log("StopQCAR");
        MarkerTracker tracker = TrackerManager.Instance.GetTracker<MarkerTracker>();
        if (tracker != null)
        {
            tracker.Stop();
        }
        ImageTracker tracker2 = TrackerManager.Instance.GetTracker<ImageTracker>();
        if (tracker2 != null)
        {
            tracker2.Stop();
        }
        TextTracker tracker3 = TrackerManager.Instance.GetTracker<TextTracker>();
        if (tracker3 != null)
        {
            tracker3.Stop();
        }
        this.CameraDirection = CameraDevice.Instance.GetCameraDirection();
        CameraDevice.Instance.Stop();
        CameraDevice.Instance.Deinit();
        QCARRenderer.Instance.ClearVideoBackgroundConfig();
    }

    public bool UnregisterTrackerEventHandler(ITrackerEventHandler trackerEventHandler)
    {
        return this.mTrackerEventHandlers.Remove(trackerEventHandler);
    }

    public bool UnregisterVideoBgEventHandler(IVideoBackgroundEventHandler videoBgEventHandler)
    {
        return this.mVideoBgEventHandlers.Remove(videoBgEventHandler);
    }

    private void Update()
    {
        if (QCARManager.Instance.Initialized)
        {
            if (this.mAndroidUnityPlayer != null)
            {
                this.mAndroidUnityPlayer.Update();
            }
            ScreenOrientation surfaceOrientation = (ScreenOrientation) QCARWrapper.Instance.GetSurfaceOrientation();
            CameraDeviceImpl instance = (CameraDeviceImpl) CameraDevice.Instance;
            if (instance.CameraReady && (QCARUnity.IsRendererDirty() || (this.mProjectionOrientation != surfaceOrientation)))
            {
                this.ConfigureVideoBackground(false);
                this.UpdateProjection(surfaceOrientation);
                instance.ResetDirtyFlag();
            }
            this.mClearMaterial.SetPass(0);
            if (((QCARManagerImpl) QCARManager.Instance).Update(this.mProjectionOrientation, this.CameraDeviceMode, ref this.mVideoMode))
            {
                this.UpdateCameraClearFlags();
                foreach (ITrackerEventHandler handler in this.mTrackerEventHandlers)
                {
                    handler.OnTrackablesUpdated();
                }
            }
        }
        else if (QCARRuntimeUtilities.IsPlayMode())
        {
            Debug.LogWarning("Scripts have been recompiled during Play mode, need to restart!");
            QCARWrapper.Create();
            PlayModeEditorUtility.Instance.RestartPlayMode();
        }
    }

    private void UpdateCameraClearFlags()
    {
        if (!QCARRuntimeUtilities.IsQCAREnabled())
        {
            this.mCameraState = CameraState.UNINITED;
        }
        else
        {
            switch (this.mCameraState)
            {
                case CameraState.UNINITED:
                    this.mCameraState = CameraState.DEVICE_INITED;
                    return;

                case CameraState.DEVICE_INITED:
                    if (!QCARUnity.RequiresAlpha())
                    {
                        if (this.mCachedDrawVideoBackground)
                        {
                            base.camera.clearFlags = CameraClearFlags.Depth;
                            Debug.Log("Setting camera clear flags to depth only");
                        }
                        else
                        {
                            base.camera.clearFlags = this.mCachedCameraClearFlags;
                            base.camera.backgroundColor = this.mCachedCameraBackgroundColor;
                            Debug.Log("Setting camera clear flags to Inspector values");
                        }
                        break;
                    }
                    base.camera.clearFlags = CameraClearFlags.Color;
                    base.camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
                    Debug.Log("Setting camera clear flags to transparent black");
                    break;

                case CameraState.RENDERING_INITED:
                {
                    bool drawVideoBackground = QCARManager.Instance.DrawVideoBackground;
                    if (drawVideoBackground != this.mCachedDrawVideoBackground)
                    {
                        this.mCameraState = CameraState.DEVICE_INITED;
                        this.mCachedDrawVideoBackground = drawVideoBackground;
                    }
                    return;
                }
                default:
                    return;
            }
            this.mCameraState = CameraState.RENDERING_INITED;
        }
    }

    private /*unsafe*/ void UpdateProjection(ScreenOrientation orientation)
    {
        if (QCARRuntimeUtilities.IsQCAREnabled())
        {
            this.mProjectionOrientation = orientation;
            QCARRuntimeUtilities.CacheSurfaceOrientation(orientation);
            Matrix4x4 matrixx = QCARUnity.GetProjectionGL(base.camera.nearClipPlane, base.camera.farClipPlane, this.mProjectionOrientation);
            if (this.mViewportRect.width != Screen.width)
            {
                //ref Matrix4x4 matrixxRef;
                //Matrix4x4 *matrixxRef = &matrixx;
                float num = (this.mViewportRect.width / ((float)Screen.width)) * matrixx[0];
                //(matrixxRef = (Matrix4x4) &matrixx)[0] = matrixxRef[0] * num;
                //(*matrixxRef)[0] = (*matrixxRef)[0] * num;
                matrixx[0] = num;

            }
            if (this.mViewportRect.height != Screen.height)
            {
                //ref Matrix4x4 matrixxRef2;
                //Matrix4x4 *matrixxRef2 = (Matrix4x4 *) &matrixx;
                float num2 = (this.mViewportRect.height / ((float)Screen.height)) * matrixx[5];
                //(matrixxRef2 = (Matrix4x4) &matrixx)[5] = matrixxRef2[5] * num2;
                //(*matrixxRef2)[5] = (*matrixxRef2)[5] * num2;
                matrixx[5] = num2;
            }
            base.camera.projectionMatrix = matrixx;
        }
    }

    public CameraDevice.CameraDeviceMode CameraDeviceMode
    {
        get
        {
            return this.CameraDeviceModeSetting;
        }
    }

    public bool VideoBackGroundMirrored { get; private set; }

    public TrackableBehaviour WorldCenter
    {
        get
        {
            return this.mWorldCenter;
        }
    }

    public WorldCenterMode WorldCenterModeSetting
    {
        get
        {
            return this.mWorldCenterMode;
        }
    }

    private enum CameraState
    {
        UNINITED,
        DEVICE_INITED,
        RENDERING_INITED
    }

    public enum WorldCenterMode
    {
        SPECIFIC_TARGET,
        FIRST_TARGET,
        CAMERA
    }
}

