using System;
using UnityEngine;

public class BehaviourComponentFactory
{
    private static IBehaviourComponentFactory sInstance;

    public static IBehaviourComponentFactory Instance
    {
        get
        {
            if (sInstance == null)
            {
                sInstance = new NullBehaviourComponentFactory();
            }
            return sInstance;
        }
        set
        {
            sInstance = value;
        }
    }

    private class NullBehaviourComponentFactory : IBehaviourComponentFactory
    {
        public CylinderTargetAbstractBehaviour AddCylinderTargetBehaviour(GameObject gameObject)
        {
            return null;
        }

        public ImageTargetAbstractBehaviour AddImageTargetBehaviour(GameObject gameObject)
        {
            return null;
        }

        public MarkerAbstractBehaviour AddMarkerBehaviour(GameObject gameObject)
        {
            return null;
        }

        public MaskOutAbstractBehaviour AddMaskOutBehaviour(GameObject gameObject)
        {
            return null;
        }

        public MultiTargetAbstractBehaviour AddMultiTargetBehaviour(GameObject gameObject)
        {
            return null;
        }

        public TurnOffAbstractBehaviour AddTurnOffBehaviour(GameObject gameObject)
        {
            return null;
        }

        public VirtualButtonAbstractBehaviour AddVirtualButtonBehaviour(GameObject gameObject)
        {
            return null;
        }

        public WordAbstractBehaviour AddWordBehaviour(GameObject gameObject)
        {
            return null;
        }
    }
}

