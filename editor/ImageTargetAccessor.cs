using System;
using UnityEditor;

public class ImageTargetAccessor : TrackableAccessor
{
    public ImageTargetAccessor(ImageTargetAbstractBehaviour target)
    {
        base.mTarget = target;
    }

    public override void ApplyDataSetAppearance()
    {
        if (QCARUtilities.GetPrefabType(base.mTarget) != PrefabType.Prefab)
        {
            ConfigData.ImageTargetData data;
            IEditorImageTargetBehaviour mTarget = (ImageTargetAbstractBehaviour) base.mTarget;
            if (this.TrackableInDataSet(mTarget.TrackableName, mTarget.DataSetName))
            {
                ConfigDataManager.Instance.GetConfigData(mTarget.DataSetName).GetImageTarget(mTarget.TrackableName, out data);
            }
            else if (mTarget.ImageTargetType != ImageTargetType.PREDEFINED)
            {
                data = QCARUtilities.CreateDefaultImageTarget();
            }
            else
            {
                ConfigDataManager.Instance.GetConfigData("--- EMPTY ---").GetImageTarget("--- EMPTY ---", out data);
                mTarget.SetDataSetPath("--- EMPTY ---");
                mTarget.SetNameForTrackable("--- EMPTY ---");
            }
            ImageTargetEditor.UpdateAspectRatio(mTarget, data.size);
            ImageTargetEditor.UpdateMaterial(mTarget);
        }
    }

    public override void ApplyDataSetProperties()
    {
        if (QCARUtilities.GetPrefabType(base.mTarget) != PrefabType.Prefab)
        {
            ConfigData.ImageTargetData data;
            ImageTargetAbstractBehaviour mTarget = (ImageTargetAbstractBehaviour) base.mTarget;
            IEditorImageTargetBehaviour behaviour2 = mTarget;
            if (this.TrackableInDataSet(behaviour2.TrackableName, behaviour2.DataSetName))
            {
                ConfigDataManager.Instance.GetConfigData(behaviour2.DataSetName).GetImageTarget(behaviour2.TrackableName, out data);
            }
            else if (behaviour2.ImageTargetType != ImageTargetType.PREDEFINED)
            {
                data = QCARUtilities.CreateDefaultImageTarget();
            }
            else
            {
                ConfigDataManager.Instance.GetConfigData("--- EMPTY ---").GetImageTarget("--- EMPTY ---", out data);
                behaviour2.SetDataSetPath("--- EMPTY ---");
                behaviour2.SetNameForTrackable("--- EMPTY ---");
            }
            ImageTargetEditor.UpdateScale(mTarget, data.size);
            ImageTargetEditor.UpdateVirtualButtons(mTarget, data.virtualButtons.ToArray());
        }
    }

    private bool TrackableInDataSet(string trackableName, string dataSetName)
    {
        return (ConfigDataManager.Instance.ConfigDataExists(dataSetName) && ConfigDataManager.Instance.GetConfigData(dataSetName).ImageTargetExists(trackableName));
    }
}

