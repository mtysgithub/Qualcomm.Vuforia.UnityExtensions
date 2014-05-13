using System;
using UnityEngine;

public interface IEditorVirtualButtonBehaviour
{
    void InitializeVirtualButton(VirtualButton virtualButton);
    bool SetPosAndScaleFromButtonArea(Vector2 topLeft, Vector2 bottomRight);
    bool SetPreviousParent(GameObject parent);
    bool SetPreviousTransform(Matrix4x4 transform);
    bool SetSensitivitySetting(VirtualButton.Sensitivity sensibility);
    bool SetVirtualButtonName(string virtualButtonName);
    bool UpdatePose();

    bool enabled { get; set; }

    GameObject gameObject { get; }

    bool HasUpdatedPose { get; }

    GameObject PreviousParent { get; }

    Matrix4x4 PreviousTransform { get; }

    Renderer renderer { get; }

    VirtualButton.Sensitivity SensitivitySetting { get; }

    Transform transform { get; }

    bool UnregisterOnDestroy { get; set; }

    string VirtualButtonName { get; }
}

