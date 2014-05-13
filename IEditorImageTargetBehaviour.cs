using System;
using System.Runtime.InteropServices;
using UnityEngine;

public interface IEditorImageTargetBehaviour : IEditorDataSetTrackableBehaviour, IEditorTrackableBehaviour
{
    void AssociateExistingVirtualButtonBehaviour(VirtualButtonAbstractBehaviour virtualButtonBehaviour);
    void CreateMissingVirtualButtonBehaviours();
    Vector2 GetSize();
    void InitializeImageTarget(ImageTarget imageTarget);
    bool SetAspectRatio(float aspectRatio);
    void SetHeight(float height);
    bool SetImageTargetType(ImageTargetType imageTargetType);
    void SetWidth(float width);
    bool TryGetVirtualButtonBehaviourByID(int id, out VirtualButtonAbstractBehaviour virtualButtonBehaviour);

    float AspectRatio { get; }

    ImageTargetType ImageTargetType { get; }
}

