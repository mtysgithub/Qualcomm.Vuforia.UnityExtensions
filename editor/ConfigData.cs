using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ConfigData
{
    private Dictionary<string, CylinderTargetData> cylinderTargets;
    private Dictionary<string, ImageTargetData> imageTargets;
    private Dictionary<string, MultiTargetData> multiTargets;
    private Dictionary<string, RigidBodyTargetData> rigidBodyTargets;

    public ConfigData()
    {
        this.imageTargets = new Dictionary<string, ImageTargetData>();
        this.multiTargets = new Dictionary<string, MultiTargetData>();
        this.cylinderTargets = new Dictionary<string, CylinderTargetData>();
        this.rigidBodyTargets = new Dictionary<string, RigidBodyTargetData>();
    }

    public ConfigData(ConfigData original)
    {
        this.imageTargets = new Dictionary<string, ImageTargetData>(original.imageTargets);
        this.multiTargets = new Dictionary<string, MultiTargetData>(original.multiTargets);
        this.cylinderTargets = new Dictionary<string, CylinderTargetData>(original.cylinderTargets);
        this.rigidBodyTargets = new Dictionary<string, RigidBodyTargetData>(original.rigidBodyTargets);
    }

    public void AddMultiTargetPart(MultiTargetPartData item, string multiTargetName)
    {
        try
        {
            MultiTargetData data = this.multiTargets[multiTargetName];
            data.parts.Add(item);
        }
        catch
        {
            throw;
        }
    }

    public void AddVirtualButton(VirtualButtonData item, string imageTargetName)
    {
        try
        {
            ImageTargetData data = this.imageTargets[imageTargetName];
            data.virtualButtons.Add(item);
        }
        catch
        {
            throw;
        }
    }

    public void ClearAll()
    {
        this.ClearImageTargets();
        this.ClearMultiTargets();
        this.ClearCylinderTargets();
        this.ClearRigidBodyTargets();
    }

    public void ClearCylinderTargets()
    {
        this.cylinderTargets.Clear();
    }

    public void ClearImageTargets()
    {
        this.imageTargets.Clear();
    }

    public void ClearMultiTargets()
    {
        this.multiTargets.Clear();
    }

    public void ClearRigidBodyTargets()
    {
        this.rigidBodyTargets.Clear();
    }

    public void ClearVirtualButtons()
    {
        foreach (ImageTargetData data in this.imageTargets.Values)
        {
            data.virtualButtons.Clear();
        }
    }

    public void CopyCylinderTargetNames(string[] arrayToFill, int index)
    {
        try
        {
            this.cylinderTargets.Keys.CopyTo(arrayToFill, index);
        }
        catch
        {
            throw;
        }
    }

    public void CopyImageTargetNames(string[] arrayToFill, int index)
    {
        try
        {
            this.imageTargets.Keys.CopyTo(arrayToFill, index);
        }
        catch
        {
            throw;
        }
    }

    public void CopyMultiTargetNames(string[] arrayToFill, int index)
    {
        try
        {
            this.multiTargets.Keys.CopyTo(arrayToFill, index);
        }
        catch
        {
            throw;
        }
    }

    public void CopyRigidBodyTargetNames(string[] arrayToFill, int index)
    {
        try
        {
            this.rigidBodyTargets.Keys.CopyTo(arrayToFill, index);
        }
        catch
        {
            throw;
        }
    }

    public bool CylinderTargetExists(string name)
    {
        return this.cylinderTargets.ContainsKey(name);
    }

    public void GetCylinderTarget(string name, out CylinderTargetData ct)
    {
        try
        {
            ct = this.cylinderTargets[name];
        }
        catch
        {
            throw;
        }
    }

    public void GetImageTarget(string name, out ImageTargetData it)
    {
        try
        {
            it = this.imageTargets[name];
        }
        catch
        {
            throw;
        }
    }

    public void GetMultiTarget(string name, out MultiTargetData mt)
    {
        try
        {
            mt = this.multiTargets[name];
        }
        catch
        {
            throw;
        }
    }

    public void GetRigidBodyTarget(string name, out RigidBodyTargetData ct)
    {
        try
        {
            ct = this.rigidBodyTargets[name];
        }
        catch
        {
            throw;
        }
    }

    public void GetVirtualButton(string name, string imageTargetName, out VirtualButtonData vb)
    {
        vb = new VirtualButtonData();
        try
        {
            ImageTargetData data;
            this.GetImageTarget(imageTargetName, out data);
            List<VirtualButtonData> virtualButtons = data.virtualButtons;
            for (int i = 0; i < virtualButtons.Count; i++)
            {
                if (virtualButtons[i].name == name)
                {
                    vb = virtualButtons[i];
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public bool ImageTargetExists(string name)
    {
        return this.imageTargets.ContainsKey(name);
    }

    public bool MultiTargetExists(string name)
    {
        return this.multiTargets.ContainsKey(name);
    }

    public bool RemoveCylinderTarget(string name)
    {
        return this.cylinderTargets.Remove(name);
    }

    public bool RemoveImageTarget(string name)
    {
        return this.imageTargets.Remove(name);
    }

    public bool RemoveMultiTarget(string name)
    {
        return this.multiTargets.Remove(name);
    }

    public bool RemoveRigidBodyTarget(string name)
    {
        return this.rigidBodyTargets.Remove(name);
    }

    public bool RigidBodyTargetExists(string name)
    {
        return this.rigidBodyTargets.ContainsKey(name);
    }

    public void SetCylinderTarget(CylinderTargetData item, string name)
    {
        this.cylinderTargets[name] = item;
    }

    public void SetImageTarget(ImageTargetData item, string name)
    {
        this.imageTargets[name] = item;
    }

    public void SetMultiTarget(MultiTargetData item, string name)
    {
        this.multiTargets[name] = item;
    }

    public void SetRigidBodyTarget(RigidBodyTargetData item, string name)
    {
        this.rigidBodyTargets[name] = item;
    }

    public int NumCylinderTargets
    {
        get
        {
            return this.cylinderTargets.Count;
        }
    }

    public int NumImageTargets
    {
        get
        {
            return this.imageTargets.Count;
        }
    }

    public int NumMultiTargets
    {
        get
        {
            return this.multiTargets.Count;
        }
    }

    public int NumRigidBodyTargets
    {
        get
        {
            return this.rigidBodyTargets.Count;
        }
    }

    public int NumTrackables
    {
        get
        {
            return (((this.NumImageTargets + this.NumMultiTargets) + this.NumCylinderTargets) + this.NumRigidBodyTargets);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CylinderTargetData
    {
        public string name;
        public float sideLength;
        public float topDiameter;
        public float bottomDiameter;
        public bool hasTopGeometry;
        public bool hasBottomGeometry;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FrameMarkerData
    {
        public string name;
        public Vector2 size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageTargetData
    {
        public Vector2 size;
        public List<ConfigData.VirtualButtonData> virtualButtons;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MultiTargetData
    {
        public List<ConfigData.MultiTargetPartData> parts;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MultiTargetPartData
    {
        public string name;
        public Vector3 translation;
        public Quaternion rotation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RigidBodyTargetData
    {
        public string name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VirtualButtonData
    {
        public string name;
        public bool enabled;
        public Vector4 rectangle;
        public VirtualButton.Sensitivity sensitivity;
    }
}

