using System;
using UnityEditor;

public class CylinderTargetAccessor : TrackableAccessor
{
    public CylinderTargetAccessor(CylinderTargetAbstractBehaviour target)
    {
        base.mTarget = target;
    }

    public override void ApplyDataSetAppearance()
    {
        if (QCARUtilities.GetPrefabType(base.mTarget) != PrefabType.Prefab)
        {
            ConfigData.CylinderTargetData data;
            IEditorCylinderTargetBehaviour mTarget = (CylinderTargetAbstractBehaviour) base.mTarget;
            if (this.TrackableInDataSet(mTarget.TrackableName, mTarget.DataSetName))
            {
                ConfigDataManager.Instance.GetConfigData(mTarget.DataSetName).GetCylinderTarget(mTarget.TrackableName, out data);
            }
            else
            {
                ConfigDataManager.Instance.GetConfigData("--- EMPTY ---").GetCylinderTarget("--- EMPTY ---", out data);
                mTarget.SetDataSetPath("--- EMPTY ---");
                mTarget.SetNameForTrackable("--- EMPTY ---");
            }
            CylinderTargetEditor.UpdateAspectRatio(mTarget, data);
        }
    }

    public override void ApplyDataSetProperties()
    {
        if (QCARUtilities.GetPrefabType(base.mTarget) != PrefabType.Prefab)
        {
            ConfigData.CylinderTargetData data;
            IEditorCylinderTargetBehaviour mTarget = (CylinderTargetAbstractBehaviour) base.mTarget;
            if (this.TrackableInDataSet(mTarget.TrackableName, mTarget.DataSetName))
            {
                ConfigDataManager.Instance.GetConfigData(mTarget.DataSetName).GetCylinderTarget(mTarget.TrackableName, out data);
            }
            else
            {
                ConfigDataManager.Instance.GetConfigData("--- EMPTY ---").GetCylinderTarget("--- EMPTY ---", out data);
                mTarget.SetDataSetPath("--- EMPTY ---");
                mTarget.SetNameForTrackable("--- EMPTY ---");
            }
            CylinderTargetEditor.UpdateScale(mTarget, data.sideLength);
        }
    }

    private bool TrackableInDataSet(string trackableName, string dataSetName)
    {
        return (ConfigDataManager.Instance.ConfigDataExists(dataSetName) && ConfigDataManager.Instance.GetConfigData(dataSetName).CylinderTargetExists(trackableName));
    }
}

