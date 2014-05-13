using UnityEngine;

public interface IBehaviourComponentFactory
{
    CylinderTargetAbstractBehaviour AddCylinderTargetBehaviour(GameObject gameObject);
    ImageTargetAbstractBehaviour AddImageTargetBehaviour(GameObject gameObject);
    MarkerAbstractBehaviour AddMarkerBehaviour(GameObject gameObject);
    MaskOutAbstractBehaviour AddMaskOutBehaviour(GameObject gameObject);
    MultiTargetAbstractBehaviour AddMultiTargetBehaviour(GameObject gameObject);
    TurnOffAbstractBehaviour AddTurnOffBehaviour(GameObject gameObject);
    VirtualButtonAbstractBehaviour AddVirtualButtonBehaviour(GameObject gameObject);
    WordAbstractBehaviour AddWordBehaviour(GameObject gameObject);
}

