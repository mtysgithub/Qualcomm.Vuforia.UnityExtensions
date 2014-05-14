using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

public static class CylinderDatasetReader
{
    private const string BOTTOM_DIAMETER_ATTRIBUTE = "bottomDiameter";
    private const string CONFIG_XML = "config.info";
    private const string CYLINDER_TARGET_ELEMENT = "CylinderTarget";
    private const string KEYFRAME_ELEMENT = "Keyframe";
    private const string NAME_ATTRIBUTE = "name";
    private const string SIDELENGTH_ATTRIBUTE = "sideLength";
    private const string TOP_DIAMETER_ATTRIBUTE = "topDiameter";

    private static string GetBottomImageFile(string name)
    {
        return (name + ".Bottom");
    }

    private static string GetTopImageFile(string name)
    {
        return (name + ".Top");
    }

    public static void Read(string datFile, ConfigData.CylinderTargetData[] targetData)
    {
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        Stream input = Unzipper.Instance.UnzipFile(datFile, "config.info");
        if (input != null)
        {
            using (XmlTextReader reader = new XmlTextReader(input))
            {
                int index = -1;
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                for (int i = 0; i < targetData.Length; i++)
                {
                    dictionary[targetData[i].name] = i;
                }
                string bottomImageFile = "";
                string topImageFile = "";
                while (reader.Read())
                {
                    string str8;
                    if ((reader.NodeType == XmlNodeType.Element) && ((str8 = reader.Name) != null))
                    {
                        if (!(str8 == "CylinderTarget"))
                        {
                            if (str8 == "Keyframe")
                            {
                                goto Label_019D;
                            }
                        }
                        else
                        {
                            string attribute = reader.GetAttribute("name");
                            if ((attribute == null) || !dictionary.ContainsKey(attribute))
                            {
                                index = -1;
                            }
                            else
                            {
                                int num3 = dictionary[attribute];
                                ConfigData.CylinderTargetData data = targetData[num3];
                                string s = reader.GetAttribute("sideLength");
                                string str5 = reader.GetAttribute("topDiameter");
                                string str6 = reader.GetAttribute("bottomDiameter");
                                if (((s != null) && (str5 != null)) && (str6 != null))
                                {
                                    float num4 = float.Parse(s, invariantCulture);
                                    float num5 = 1f;
                                    if (data.sideLength > 0f)
                                    {
                                        num5 = data.sideLength / num4;
                                    }
                                    else
                                    {
                                        data.sideLength = num4;
                                    }
                                    data.topDiameter = num5 * float.Parse(str5, invariantCulture);
                                    data.bottomDiameter = num5 * float.Parse(str6, invariantCulture);
                                    index = num3;
                                    targetData[num3] = data;
                                    bottomImageFile = GetBottomImageFile(attribute);
                                    topImageFile = GetTopImageFile(attribute);
                                }
                            }
                        }
                    }
                    continue;
                Label_019D:
                    if (index >= 0)
                    {
                        ConfigData.CylinderTargetData data2 = targetData[index];
                        string str7 = reader.GetAttribute("name");
                        if (str7 == topImageFile)
                        {
                            data2.hasTopGeometry = true;
                        }
                        else if (str7 == bottomImageFile)
                        {
                            data2.hasBottomGeometry = true;
                        }
                        targetData[index] = data2;
                    }
                }
            }
            input.Dispose();
        }
    }
}

