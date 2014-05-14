using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigDataManager
{
    private Dictionary<string, ConfigData> mConfigData = new Dictionary<string, ConfigData>();
    private static ConfigDataManager mInstance;
    private TextConfigData mTextConfigData = new TextConfigData();

    private ConfigDataManager()
    {
    }

    public bool ConfigDataExists(string configDataName)
    {
        return this.mConfigData.ContainsKey(configDataName);
    }

    private List<string> CorrectXMLFileList(List<string> xmlFileList)
    {
        List<string> list = new List<string>(xmlFileList.Count);
        List<string> filePaths = this.GetFilePaths("Assets/StreamingAssets/QCAR/", "dat");
        foreach (string str in xmlFileList)
        {
            bool flag = false;
            string str2 = str.Remove(str.Length - 4);
            foreach (string str3 in filePaths)
            {
                if (str3.IndexOf(str2) == 0)
                {
                    list.Add(str);
                    flag = true;
                }
            }
            if (!flag)
            {
                Debug.LogWarning(str + " ignored. No corresponding DAT file found.");
            }
        }
        return list;
    }

    private ConfigData.CylinderTargetData CreateDefaultCylinderTarget()
    {
        return new ConfigData.CylinderTargetData { sideLength = 100f, topDiameter = 50f, bottomDiameter = 50f, hasTopGeometry = false, hasBottomGeometry = false };
    }

    private ConfigData CreateDefaultDataSet()
    {
        ConfigData data = new ConfigData();
        data.SetImageTarget(QCARUtilities.CreateDefaultImageTarget(), "--- EMPTY ---");
        data.SetMultiTarget(this.CreateDefaultMultiTarget(), "--- EMPTY ---");
        data.SetCylinderTarget(this.CreateDefaultCylinderTarget(), "--- EMPTY ---");
        data.SetRigidBodyTarget(this.CreateDefaultRigidBodyTarget(), "--- EMPTY ---");
        return data;
    }

    private ConfigData.MultiTargetData CreateDefaultMultiTarget()
    {
        return new ConfigData.MultiTargetData { parts = this.CreateDefaultParts() };
    }

    private List<ConfigData.MultiTargetPartData> CreateDefaultParts()
    {
        List<ConfigData.MultiTargetPartData> list = new List<ConfigData.MultiTargetPartData>(6);
        float y = QCARUtilities.CreateDefaultImageTarget().size.x * 0.5f;
        ConfigData.MultiTargetPartData item = new ConfigData.MultiTargetPartData {
            translation = new Vector3(0f, y, 0f),
            rotation = Quaternion.AngleAxis(0f, new Vector3(1f, 0f, 0f)),
            name = "--- EMPTY ---"
        };
        list.Add(item);
        ConfigData.MultiTargetPartData data3 = new ConfigData.MultiTargetPartData {
            translation = new Vector3(0f, -y, 0f),
            rotation = Quaternion.AngleAxis(180f, new Vector3(1f, 0f, 0f)),
            name = "--- EMPTY ---"
        };
        list.Add(data3);
        ConfigData.MultiTargetPartData data4 = new ConfigData.MultiTargetPartData {
            translation = new Vector3(-y, 0f, 0f),
            rotation = Quaternion.AngleAxis(90f, new Vector3(0f, 0f, 1f)),
            name = "--- EMPTY ---"
        };
        list.Add(data4);
        ConfigData.MultiTargetPartData data5 = new ConfigData.MultiTargetPartData {
            translation = new Vector3(y, 0f, 0f),
            rotation = Quaternion.AngleAxis(-90f, new Vector3(0f, 0f, 1f)),
            name = "--- EMPTY ---"
        };
        list.Add(data5);
        ConfigData.MultiTargetPartData data6 = new ConfigData.MultiTargetPartData {
            translation = new Vector3(0f, 0f, y),
            rotation = Quaternion.AngleAxis(90f, new Vector3(1f, 0f, 0f)),
            name = "--- EMPTY ---"
        };
        list.Add(data6);
        ConfigData.MultiTargetPartData data7 = new ConfigData.MultiTargetPartData {
            translation = new Vector3(0f, 0f, -y),
            rotation = Quaternion.AngleAxis(-90f, new Vector3(1f, 0f, 0f)),
            name = "--- EMPTY ---"
        };
        list.Add(data7);
        return list;
    }

    private ConfigData.RigidBodyTargetData CreateDefaultRigidBodyTarget()
    {
        return new ConfigData.RigidBodyTargetData { name = "--- EMPTY ---" };
    }

    public void DoRead()
    {
        List<string> filePaths = this.GetFilePaths("Assets/StreamingAssets/QCAR/", "xml");
        List<string> list2 = this.CorrectXMLFileList(filePaths);
        this.mConfigData.Clear();
        this.mConfigData.Add("--- EMPTY ---", this.CreateDefaultDataSet());
        foreach (string str in list2)
        {
            this.ReadConfigData(str);
        }
        this.ReadTextConfigData();
    }

    public ConfigData GetConfigData(string dataSetName)
    {
        return this.mConfigData[dataSetName];
    }

    public void GetConfigDataNames(string[] configDataNames)
    {
        try
        {
            this.GetConfigDataNames(configDataNames, true);
        }
        catch
        {
            throw;
        }
    }

    public void GetConfigDataNames(string[] configDataNames, bool includeDefault)
    {
        try
        {
            if (includeDefault)
            {
                this.mConfigData.Keys.CopyTo(configDataNames, 0);
            }
            else
            {
                Dictionary<string, ConfigData>.KeyCollection.Enumerator enumerator = this.mConfigData.Keys.GetEnumerator();
                int index = 0;
                enumerator.MoveNext();
                while (enumerator.MoveNext())
                {
                    configDataNames[index] = enumerator.Current;
                    index++;
                }
            }
        }
        catch
        {
            throw;
        }
    }

    private List<string> GetFilePaths(string directoryPath, string fileType)
    {
        List<string> list = new List<string>();
        if (Directory.Exists(directoryPath))
        {
            foreach (string str in Directory.GetFiles(directoryPath))
            {
                if (QCARRuntimeUtilities.StripExtensionFromPath(str).IndexOf(fileType, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    list.Add(str);
                }
            }
        }
        return list;
    }

    public TextConfigData GetTextConfigData()
    {
        return this.mTextConfigData;
    }

    private void ReadConfigData(string dataSetFilePath)
    {
        ConfigData configData = new ConfigData();
        ConfigParser.Instance.fileToStruct(dataSetFilePath, configData);
        string str = QCARRuntimeUtilities.StripFileNameFromPath(dataSetFilePath);
        string str2 = str.Remove(str.Length - 4);
        this.mConfigData[str2] = configData;
    }

    private void ReadTextConfigData()
    {
        List<string> filePaths = this.GetFilePaths("Assets/StreamingAssets/QCAR/", "vwl");
        List<string> list2 = this.GetFilePaths("Assets/StreamingAssets/QCAR/", "lst");
        TextConfigData data = new TextConfigData();
        data.SetDictionaryData(new TextConfigData.DictionaryData(), "--- EMPTY ---");
        foreach (string str in filePaths)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str);
            string str3 = "QCAR/" + fileNameWithoutExtension + ".vwl";
            TextConfigData.DictionaryData data2 = new TextConfigData.DictionaryData {
                BinaryFile = str3
            };
            data.SetDictionaryData(data2, fileNameWithoutExtension);
        }
        data.SetWordListData(new TextConfigData.WordListData(), "--- EMPTY ---");
        foreach (string str4 in list2)
        {
            string name = Path.GetFileNameWithoutExtension(str4);
            string str6 = "QCAR/" + name + ".lst";
            TextConfigData.WordListData data3 = new TextConfigData.WordListData {
                TextFile = str6
            };
            data.SetWordListData(data3, name);
        }
        this.mTextConfigData = data;
    }

    public static ConfigDataManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                lock (typeof(ConfigDataManager))
                {
                    if (mInstance == null)
                    {
                        mInstance = new ConfigDataManager();
                    }
                }
            }
            return mInstance;
        }
    }

    public int NumConfigDataObjects
    {
        get
        {
            return this.mConfigData.Count;
        }
    }
}

