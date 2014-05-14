using System;
using UnityEditor;

public class MultiTargetAccessor : TrackableAccessor
{
    public MultiTargetAccessor(MultiTargetAbstractBehaviour target)
    {
        base.mTarget = target;
    }

    public override void ApplyDataSetAppearance()
    {
        this.ApplyDataSetProperties();
    }

    public override void ApplyDataSetProperties()
    {
        if (QCARUtilities.GetPrefabType(base.mTarget) != PrefabType.Prefab)
        {
            ConfigData.MultiTargetData data;
            IEditorMultiTargetBehaviour mTarget = (MultiTargetAbstractBehaviour) base.mTarget;
            if (this.TrackableInDataSet(mTarget.TrackableName, mTarget.DataSetName))
            {
                ConfigDataManager.Instance.GetConfigData(mTarget.DataSetName).GetMultiTarget(mTarget.TrackableName, out data);
            }
            else
            {
                ConfigDataManager.Instance.GetConfigData("--- EMPTY ---").GetMultiTarget("--- EMPTY ---", out data);
                mTarget.SetDataSetPath("--- EMPTY ---");
                mTarget.SetNameForTrackable("--- EMPTY ---");
            }
            MultiTargetEditor.UpdateParts(mTarget, data.parts.ToArray());
        }
    }

    private bool TrackableInDataSet(string trackableName, string dataSetName)
    {
        return (ConfigDataManager.Instance.ConfigDataExists(dataSetName) && ConfigDataManager.Instance.GetConfigData(dataSetName).MultiTargetExists(trackableName));
    }
}

