using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SceneManager
{
    private bool mApplyAppearance;
    private bool mApplyProperties;
    private string mDataSetExportPath = "";
    private bool mDoDeserialization;
    private bool mDoSerialization;
    private bool mGoToSampleAppPage;
    private bool mGoToTargetManagerPage;
    private static SceneManager mInstance;
    private DateTime mLastUpdate = DateTime.Now;
    private bool mSceneInitialized;
    private EditorApplication.CallbackFunction mUpdateCallback;
    private bool mValidateScene;
    private static TimeSpan UPDATE_INTERVAL = TimeSpan.FromSeconds(60.0);

    private SceneManager()
    {
        this.mUpdateCallback = new EditorApplication.CallbackFunction(this.EditorUpdate);
        if (EditorApplication.update == null)
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, this.mUpdateCallback);
        }
        else if (!EditorApplication.update.Equals(this.mUpdateCallback))
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, this.mUpdateCallback);
        }
        this.mDoDeserialization = true;
        this.mSceneInitialized = false;
    }

    public void ApplyDataSetAppearance()
    {
        this.mApplyAppearance = true;
    }

    public void ApplyDataSetProperties()
    {
        this.mApplyProperties = true;
    }

    private void CheckForDuplicates(IEditorTrackableBehaviour[] trackables)
    {
        for (int i = 0; i < trackables.Length; i++)
        {
            string trackableName = trackables[i].TrackableName;
            if (trackableName != "--- EMPTY ---")
            {
                for (int j = i + 1; j < trackables.Length; j++)
                {
                    string str2 = trackables[j].TrackableName;
                    if (str2 == "--- EMPTY ---")
                    {
                        continue;
                    }
                    if ((trackables[i] is DataSetTrackableBehaviour) && (trackables[j] is DataSetTrackableBehaviour))
                    {
                        IEditorDataSetTrackableBehaviour behaviour = (DataSetTrackableBehaviour) trackables[i];
                        IEditorDataSetTrackableBehaviour behaviour2 = (DataSetTrackableBehaviour) trackables[j];
                        string dataSetName = behaviour.DataSetName;
                        string str4 = behaviour2.DataSetName;
                        if (dataSetName.IndexOf(str4) == 0)
                        {
                            goto Label_009A;
                        }
                        continue;
                    }
                    if (!(trackables[i] is MarkerAbstractBehaviour) || !(trackables[j] is MarkerAbstractBehaviour))
                    {
                        continue;
                    }
                Label_009A:
                    if (trackableName == str2)
                    {
                        UnityEngine.Debug.LogWarning("Duplicate Trackables detected: \"" + trackableName + "\". Only one of the Trackables and its respective Augmentation will be selected for use at runtime - that selection is indeterminate here.");
                    }
                }
            }
        }
    }

    private bool CorrectTrackableScales(TrackableBehaviour[] trackables)
    {
        bool flag = false;
        TrackableBehaviour[] behaviourArray = trackables;
        for (int i = 0; i < behaviourArray.Length; i++)
        {
            IEditorTrackableBehaviour behaviour = behaviourArray[i];
            if (behaviour.CorrectScale())
            {
                flag = true;
            }
        }
        return flag;
    }

    private ConfigData CreateDataSetFromTrackables(TrackableBehaviour[] trackables)
    {
        if (trackables == null)
        {
            return null;
        }
        ConfigData data = new ConfigData();
        foreach (TrackableBehaviour behaviour in trackables)
        {
            if (behaviour is DataSetTrackableBehaviour)
            {
                IEditorDataSetTrackableBehaviour behaviour2 = (DataSetTrackableBehaviour) behaviour;
                string dataSetName = behaviour2.DataSetName;
                string trackableName = behaviour2.TrackableName;
                if (((dataSetName == "--- EMPTY ---") || (dataSetName == "")) || ((trackableName == "--- EMPTY ---") || (trackableName == "")))
                {
                    UnityEngine.Debug.LogWarning("Ignoring default Trackable for export");
                }
                else if (behaviour2 is ImageTargetAbstractBehaviour)
                {
                    ImageTargetAbstractBehaviour behaviour3 = (ImageTargetAbstractBehaviour) behaviour2;
                    IEditorImageTargetBehaviour behaviour4 = behaviour3;
                    ConfigData.ImageTargetData item = new ConfigData.ImageTargetData {
                        size = behaviour4.GetSize()
                    };
                    VirtualButtonAbstractBehaviour[] componentsInChildren = behaviour3.GetComponentsInChildren<VirtualButtonAbstractBehaviour>();
                    item.virtualButtons = new List<ConfigData.VirtualButtonData>(componentsInChildren.Length);
                    foreach (VirtualButtonAbstractBehaviour behaviour5 in componentsInChildren)
                    {
                        Vector2 vector;
                        Vector2 vector2;
                        if (behaviour5.CalculateButtonArea(out vector, out vector2))
                        {
                            ConfigData.VirtualButtonData data3 = new ConfigData.VirtualButtonData();
                            IEditorVirtualButtonBehaviour behaviour6 = behaviour5;
                            data3.name = behaviour6.VirtualButtonName;
                            data3.enabled = behaviour6.enabled;
                            data3.rectangle = new Vector4(vector.x, vector.y, vector2.x, vector2.y);
                            data3.sensitivity = behaviour6.SensitivitySetting;
                            item.virtualButtons.Add(data3);
                        }
                    }
                    data.SetImageTarget(item, behaviour4.TrackableName);
                }
                else if (behaviour2 is MultiTargetAbstractBehaviour)
                {
                    UnityEngine.Debug.Log("Multi Targets not exported.");
                }
                else if (behaviour2 is CylinderTargetAbstractBehaviour)
                {
                    UnityEngine.Debug.Log("Cylinder Targets not exported.");
                }
            }
        }
        return data;
    }

    public void EditorUpdate()
    {
        TrackableBehaviour[] trackables = (TrackableBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(TrackableBehaviour));
        VirtualButtonAbstractBehaviour[] vbs = (VirtualButtonAbstractBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(VirtualButtonAbstractBehaviour));
        this.CorrectTrackableScales(trackables);
        VirtualButtonEditor.CorrectPoses(vbs);
        if (this.mDoDeserialization)
        {
            ConfigDataManager.Instance.DoRead();
            DataSetLoadEditor.OnConfigDataChanged();
            this.ApplyDataSetAppearance();
            this.mDoDeserialization = false;
        }
        if (this.mApplyAppearance)
        {
            this.UpdateTrackableAppearance(trackables);
            this.mApplyAppearance = false;
        }
        if (this.mApplyProperties)
        {
            this.UpdateTrackableProperties(trackables);
            this.mApplyProperties = false;
        }
        if (this.mDoSerialization)
        {
            ConfigData configData = this.CreateDataSetFromTrackables(trackables);
            if (!ConfigParser.Instance.structToFile(this.mDataSetExportPath, configData))
            {
                UnityEngine.Debug.LogError("Export of scene file failed.");
            }
            this.mDoSerialization = false;
        }
        if (this.mValidateScene)
        {
            this.CheckForDuplicates(trackables);
            WordEditor.CheckForDuplicates();
            VirtualButtonEditor.Validate();
            this.mValidateScene = false;
        }
        if (this.mGoToTargetManagerPage)
        {
            this.mGoToTargetManagerPage = false;
            Process.Start("https://developer.vuforia.com/target-manager");
        }
        if (this.mGoToSampleAppPage)
        {
            this.mGoToSampleAppPage = false;
            Process.Start("https://developer.vuforia.com/resources/sample-apps");
        }
        if (this.mLastUpdate.Add(UPDATE_INTERVAL) < DateTime.Now)
        {
            SetUnityVersion();
            this.mLastUpdate = DateTime.Now;
        }
    }

    public void ExportScene(string path)
    {
        this.mDataSetExportPath = path;
        this.mDoSerialization = true;
    }

    public void FilesUpdated()
    {
        this.mDoDeserialization = true;
    }

    public string[] GetCylinderTargetNames(string dataSetName)
    {
        ConfigData configData = ConfigDataManager.Instance.GetConfigData(dataSetName);
        string[] arrayToFill = new string[configData.NumCylinderTargets + 1];
        arrayToFill[0] = "--- EMPTY ---";
        configData.CopyCylinderTargetNames(arrayToFill, 1);
        return arrayToFill;
    }

    public string[] GetImageTargetNames(string dataSetName)
    {
        ConfigData configData = ConfigDataManager.Instance.GetConfigData(dataSetName);
        string[] arrayToFill = new string[configData.NumImageTargets + 1];
        arrayToFill[0] = "--- EMPTY ---";
        configData.CopyImageTargetNames(arrayToFill, 1);
        return arrayToFill;
    }

    public string[] GetMultiTargetNames(string dataSetName)
    {
        ConfigData configData = ConfigDataManager.Instance.GetConfigData(dataSetName);
        string[] arrayToFill = new string[configData.NumMultiTargets + 1];
        arrayToFill[0] = "--- EMPTY ---";
        configData.CopyMultiTargetNames(arrayToFill, 1);
        return arrayToFill;
    }

    public int GetNextFrameMarkerID()
    {
        IEditorMarkerBehaviour[] behaviourArray = (MarkerAbstractBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(MarkerAbstractBehaviour));
        if (behaviourArray.Length <= 0)
        {
            return 0;
        }
        if (behaviourArray.Length >= 0x200)
        {
            UnityEngine.Debug.LogWarning("Too many frame markers in scene.");
            return 0x1ff;
        }
        int num = 0;
        bool flag = false;
        while (!flag)
        {
            flag = true;
            for (int i = 0; i < behaviourArray.Length; i++)
            {
                if (behaviourArray[i].MarkerID == num)
                {
                    flag = false;
                    num++;
                    continue;
                }
            }
        }
        return num;
    }

    public void GoToSampleAppPage()
    {
        this.mGoToSampleAppPage = true;
    }

    public void GoToTargetManagerPage()
    {
        this.mGoToTargetManagerPage = true;
    }

    public void InitScene()
    {
        DataSetTrackableBehaviour[] trackables = (DataSetTrackableBehaviour[]) UnityEngine.Object.FindObjectsOfType(typeof(DataSetTrackableBehaviour));
        ConfigDataManager.Instance.DoRead();
        this.UpdateTrackableAppearance(trackables);
        this.mValidateScene = true;
        this.mSceneInitialized = true;
        SetUnityVersion();
    }

    public void SceneUpdated()
    {
        this.mValidateScene = true;
    }

    private static void SetUnityVersion()
    {
        QCARUnityImpl.SetUnityVersion(Path.Combine(Application.dataPath, "StreamingAssets/QCAR"), false);
    }

    private void UpdateTrackableAppearance(TrackableBehaviour[] trackables)
    {
        if (!Application.isPlaying)
        {
            foreach (TrackableBehaviour behaviour in trackables)
            {
                if (behaviour is DataSetTrackableBehaviour)
                {
                    DataSetTrackableBehaviour target = (DataSetTrackableBehaviour) behaviour;
                    TrackableAccessor accessor = AccessorFactory.Create(target);
                    if (accessor != null)
                    {
                        accessor.ApplyDataSetAppearance();
                    }
                }
            }
        }
    }

    private void UpdateTrackableProperties(TrackableBehaviour[] trackables)
    {
        foreach (TrackableBehaviour behaviour in trackables)
        {
            if (behaviour is DataSetTrackableBehaviour)
            {
                DataSetTrackableBehaviour target = (DataSetTrackableBehaviour) behaviour;
                TrackableAccessor accessor = AccessorFactory.Create(target);
                if (accessor != null)
                {
                    accessor.ApplyDataSetProperties();
                }
            }
        }
    }

    public static SceneManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                lock (typeof(SceneManager))
                {
                    if (mInstance == null)
                    {
                        mInstance = new SceneManager();
                    }
                }
            }
            return mInstance;
        }
    }

    public bool SceneInitialized
    {
        get
        {
            return this.mSceneInitialized;
        }
    }
}

