using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using UnityEngine;

public class ConfigParser
{
    private static ConfigParser mInstance;

    public bool fileToStruct(string configXMLPath, ConfigData configData)
    {
        if (!File.Exists(configXMLPath))
        {
            return false;
        }
        List<ConfigData.CylinderTargetData> list = new List<ConfigData.CylinderTargetData>();
        using (XmlTextReader reader = new XmlTextReader(configXMLPath))
        {
            while (reader.Read())
            {
                string str;
                Vector2 vector;
                string str2;
                Vector4 vector2;
                bool flag;
                string str6;
                string str7;
                string str8;
                string str9;
                Vector3 vector3;
                Quaternion quaternion;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    str = "";
                    switch (reader.Name)
                    {
                        case "ImageTarget":
                            str = reader.GetAttribute("name");
                            if (str != null)
                            {
                                goto Label_00F1;
                            }
                            Debug.LogWarning("Found ImageTarget without name attribute in config.xml. Image Target will be ignored.");
                            break;

                        case "VirtualButton":
                            str2 = reader.GetAttribute("name");
                            if (str2 != null)
                            {
                                goto Label_01AD;
                            }
                            Debug.LogWarning("Found VirtualButton without name attribute in config.xml. Virtual Button will be ignored.");
                            break;

                        case "MultiTarget":
                            str6 = reader.GetAttribute("name");
                            if (str6 != null)
                            {
                                goto Label_0387;
                            }
                            Debug.LogWarning("Found Multi Target without name attribute in config.xml. Multi Target will be ignored.");
                            break;

                        case "CylinderTarget":
                            str7 = reader.GetAttribute("name");
                            if (str7 != null)
                            {
                                goto Label_03D1;
                            }
                            Debug.LogWarning("Found Cylinder Target without name attribute in config.xml. Cylinder Target will be ignored.");
                            break;

                        case "Part":
                            str9 = reader.GetAttribute("name");
                            if (str9 != null)
                            {
                                goto Label_0445;
                            }
                            Debug.LogWarning("Found Multi Target Part without name attribute in config.xml. Part will be ignored.");
                            break;

                        case "RigidBodyTarget":
                        {
                            string name = reader.GetAttribute("name");
                            ConfigData.RigidBodyTargetData data6 = new ConfigData.RigidBodyTargetData {
                                name = name
                            };
                            configData.SetRigidBodyTarget(data6, name);
                            break;
                        }
                    }
                }
                continue;
            Label_00F1:
                vector = Vector2.zero;
                string[] valuesToParse = reader.GetAttribute("size").Split(new char[] { ' ' });
                if (valuesToParse != null)
                {
                    if (QCARUtilities.SizeFromStringArray(out vector, valuesToParse))
                    {
                        goto Label_015C;
                    }
                    Debug.LogWarning("Found illegal itSize attribute for Image Target " + str + " in config.xml. Image Target will be ignored.");
                }
                else
                {
                    Debug.LogWarning("Image Target " + str + " is missing a itSize attribut in config.xml. Image Target will be ignored.");
                }
                continue;
            Label_015C:
                reader.MoveToElement();
                ConfigData.ImageTargetData item = new ConfigData.ImageTargetData {
                    size = vector,
                    virtualButtons = new List<ConfigData.VirtualButtonData>()
                };
                configData.SetImageTarget(item, str);
                continue;
            Label_01AD:
                vector2 = Vector4.zero;
                string[] strArray2 = reader.GetAttribute("rectangle").Split(new char[] { ' ' });
                if (strArray2 != null)
                {
                    if (QCARUtilities.RectangleFromStringArray(out vector2, strArray2))
                    {
                        goto Label_021B;
                    }
                    Debug.LogWarning("Found invalid rectangle attribute for Virtual Button " + str2 + " in config.xml. Virtual Button will be ignored.");
                }
                else
                {
                    Debug.LogWarning("Virtual Button " + str2 + " has no rectangle attribute in config.xml. Virtual Button will be ignored.");
                }
                continue;
            Label_021B:
                flag = true;
                string attribute = reader.GetAttribute("enabled");
                if (attribute != null)
                {
                    if (string.Compare(attribute, "true", true) == 0)
                    {
                        flag = true;
                    }
                    else if (string.Compare(attribute, "false", true) == 0)
                    {
                        flag = false;
                    }
                    else
                    {
                        Debug.LogWarning("Found invalid enabled attribute for Virtual Button " + str2 + " in config.xml. Default setting will be used.");
                    }
                }
                VirtualButton.Sensitivity lOW = VirtualButton.Sensitivity.LOW;
                string strA = reader.GetAttribute("sensitivity");
                if (strA != null)
                {
                    if (string.Compare(strA, "low", true) == 0)
                    {
                        lOW = VirtualButton.Sensitivity.LOW;
                    }
                    else if (string.Compare(strA, "medium", true) == 0)
                    {
                        lOW = VirtualButton.Sensitivity.MEDIUM;
                    }
                    else if (string.Compare(strA, "high", true) == 0)
                    {
                        lOW = VirtualButton.Sensitivity.HIGH;
                    }
                    else
                    {
                        Debug.LogWarning("Found illegal sensitivity attribute for Virtual Button " + str2 + " in config.xml. Default setting will be used.");
                    }
                }
                reader.MoveToElement();
                ConfigData.VirtualButtonData data2 = new ConfigData.VirtualButtonData();
                string latestITName = GetLatestITName(configData);
                data2.name = str2;
                data2.rectangle = vector2;
                data2.enabled = flag;
                data2.sensitivity = lOW;
                if (configData.ImageTargetExists(latestITName))
                {
                    configData.AddVirtualButton(data2, latestITName);
                }
                else
                {
                    Debug.LogWarning("Image Target with name " + latestITName + " could not be found. Virtual Button " + str2 + "will not be added.");
                }
                continue;
            Label_0387:
                reader.MoveToElement();
                ConfigData.MultiTargetData data3 = new ConfigData.MultiTargetData {
                    parts = new List<ConfigData.MultiTargetPartData>()
                };
                configData.SetMultiTarget(data3, str6);
                continue;
            Label_03D1:
                str8 = reader.GetAttribute("sideLength");
                float num = -1f;
                if (str8 != null)
                {
                    num = float.Parse(str8, CultureInfo.InvariantCulture);
                }
                reader.MoveToElement();
                ConfigData.CylinderTargetData data4 = new ConfigData.CylinderTargetData {
                    name = str7,
                    sideLength = num
                };
                list.Add(data4);
                continue;
            Label_0445:
                vector3 = Vector3.zero;
                string[] strArray3 = reader.GetAttribute("translation").Split(new char[] { ' ' });
                if (strArray3 != null)
                {
                    if (QCARUtilities.TransformFromStringArray(out vector3, strArray3))
                    {
                        goto Label_04B3;
                    }
                    Debug.LogWarning("Found illegal transform attribute for Part " + str9 + " in config.xml. Part will be ignored.");
                }
                else
                {
                    Debug.LogWarning("Multi Target Part " + str9 + " has no translation attribute in config.xml. Part will be ignored.");
                }
                continue;
            Label_04B3:
                quaternion = Quaternion.identity;
                string[] strArray4 = reader.GetAttribute("rotation").Split(new char[] { ' ' });
                if (strArray4 != null)
                {
                    if (QCARUtilities.OrientationFromStringArray(out quaternion, strArray4))
                    {
                        goto Label_0521;
                    }
                    Debug.LogWarning("Found illegal rotation attribute for Part " + str9 + " in config.xml. Part will be ignored.");
                }
                else
                {
                    Debug.LogWarning("Multi Target Part " + str9 + " has no rotation attribute in config.xml. Part will be ignored.");
                }
                continue;
            Label_0521:
                reader.MoveToElement();
                ConfigData.MultiTargetPartData data5 = new ConfigData.MultiTargetPartData();
                string latestMTName = GetLatestMTName(configData);
                data5.name = str9;
                data5.rotation = quaternion;
                data5.translation = vector3;
                if (configData.MultiTargetExists(latestMTName))
                {
                    configData.AddMultiTargetPart(data5, latestMTName);
                }
                else
                {
                    Debug.LogWarning("Multi Target with name " + latestMTName + " could not be found. Multi Target Part " + str9 + "will not be added.");
                    continue;
                }
            }
        }
        if (list.Count > 0)
        {
            string datFile = configXMLPath.Substring(0, configXMLPath.Length - 3) + "dat";
            ConfigData.CylinderTargetData[] targetData = list.ToArray();
            CylinderDatasetReader.Read(datFile, targetData);
            foreach (ConfigData.CylinderTargetData data7 in targetData)
            {
                configData.SetCylinderTarget(data7, data7.name);
            }
        }
        return true;
    }

    private static string GetLatestITName(ConfigData backlog)
    {
        if (backlog == null)
        {
            return null;
        }
        string[] arrayToFill = new string[backlog.NumImageTargets];
        try
        {
            backlog.CopyImageTargetNames(arrayToFill, 0);
        }
        catch
        {
            return null;
        }
        return arrayToFill[backlog.NumImageTargets - 1];
    }

    private static string GetLatestMTName(ConfigData backlog)
    {
        if (backlog == null)
        {
            return null;
        }
        string[] arrayToFill = new string[backlog.NumMultiTargets];
        try
        {
            backlog.CopyMultiTargetNames(arrayToFill, 0);
        }
        catch
        {
            return null;
        }
        return arrayToFill[backlog.NumMultiTargets - 1];
    }

    public bool structToFile(string configXMLPath, ConfigData configData)
    {
        if ((configData == null) || (configData.NumTrackables <= 0))
        {
            return false;
        }
        XmlWriterSettings settings = new XmlWriterSettings {
            Indent = true
        };
        using (XmlWriter writer = XmlWriter.Create(configXMLPath, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("QCARConfig");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation", null, "qcar_config.xsd");
            writer.WriteStartElement("Tracking");
            string[] arrayToFill = new string[configData.NumImageTargets];
            configData.CopyImageTargetNames(arrayToFill, 0);
            for (int i = 0; i < arrayToFill.Length; i++)
            {
                ConfigData.ImageTargetData data;
                configData.GetImageTarget(arrayToFill[i], out data);
                writer.WriteStartElement("ImageTarget");
                string str = data.size.x.ToString() + " " + data.size.y.ToString();
                writer.WriteAttributeString("size", str);
                writer.WriteAttributeString("name", arrayToFill[i]);
                List<ConfigData.VirtualButtonData> virtualButtons = data.virtualButtons;
                for (int k = 0; k < virtualButtons.Count; k++)
                {
                    writer.WriteStartElement("VirtualButton");
                    writer.WriteAttributeString("name", virtualButtons[k].name);
                    string str2 = virtualButtons[k].rectangle.x.ToString() + " " + virtualButtons[k].rectangle.y.ToString() + " " + virtualButtons[k].rectangle.z.ToString() + " " + virtualButtons[k].rectangle.w.ToString();
                    writer.WriteAttributeString("rectangle", str2);
                    if (virtualButtons[k].enabled)
                    {
                        writer.WriteAttributeString("enabled", "true");
                    }
                    else
                    {
                        writer.WriteAttributeString("enabled", "false");
                    }
                    if (virtualButtons[k].sensitivity == VirtualButton.Sensitivity.LOW)
                    {
                        writer.WriteAttributeString("sensitivity", "low");
                    }
                    else if (virtualButtons[k].sensitivity == VirtualButton.Sensitivity.MEDIUM)
                    {
                        writer.WriteAttributeString("sensitivity", "medium");
                    }
                    else if (virtualButtons[k].sensitivity == VirtualButton.Sensitivity.HIGH)
                    {
                        writer.WriteAttributeString("sensitivity", "high");
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            string[] strArray2 = new string[configData.NumMultiTargets];
            configData.CopyMultiTargetNames(strArray2, 0);
            for (int j = 0; j < strArray2.Length; j++)
            {
                ConfigData.MultiTargetData data2;
                configData.GetMultiTarget(strArray2[j], out data2);
                writer.WriteStartElement("MultiTarget");
                writer.WriteAttributeString("name", strArray2[j]);
                List<ConfigData.MultiTargetPartData> parts = data2.parts;
                for (int m = 0; m < parts.Count; m++)
                {
                    float num5;
                    Vector3 vector;
                    writer.WriteStartElement("Part");
                    writer.WriteAttributeString("name", parts[m].name);
                    string str3 = parts[m].translation.x.ToString() + " " + parts[m].translation.z.ToString() + " " + parts[m].translation.y.ToString();
                    writer.WriteAttributeString("translation", str3);
                    parts[m].rotation.ToAngleAxis(out num5, out vector);
                    string[] strArray5 = new string[] { "AD: ", (-vector.x).ToString(), " ", (-vector.z).ToString(), " ", vector.y.ToString(), " ", num5.ToString() };
                    string str4 = string.Concat(strArray5);
                    writer.WriteAttributeString("rotation", str4);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
        return true;
    }

    public static ConfigParser Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new ConfigParser();
            }
            return mInstance;
        }
    }
}

