using System;
using UnityEngine;

public abstract class WordAbstractBehaviour : TrackableBehaviour, IEditorWordBehaviour, IEditorTrackableBehaviour
{
    [SerializeField, HideInInspector]
    private WordTemplateMode mMode;
    [SerializeField, HideInInspector]
    private string mSpecificWord;
    private Word mWord;

    protected WordAbstractBehaviour()
    {
    }

    void IEditorWordBehaviour.InitializeWord(Word word)
    {
        base.mTrackable = this.mWord = word;
        Vector2 size = word.Size;
        Vector3 one = Vector3.one;
        MeshFilter component = base.GetComponent<MeshFilter>();
        if (component != null)
        {
            one = component.sharedMesh.bounds.size;
        }
        float x = size.y / one.z;
        base.transform.localScale = new Vector3(x, x, x);
    }

    void IEditorWordBehaviour.SetMode(WordTemplateMode mode)
    {
        this.mMode = mode;
    }

    void IEditorWordBehaviour.SetSpecificWord(string word)
    {
        this.mSpecificWord = word;
    }

    protected override void InternalUnregisterTrackable()
    {
        base.mTrackable = (Trackable) (this.mWord = null);
    }

    bool IEditorTrackableBehaviour.enabled
    {
        get
        {
            return base.enabled;
        }
        set
        {
            base.enabled = value;
        }
    }

    GameObject IEditorTrackableBehaviour.gameObject
    {
        get
        {
            return base.gameObject;
        }
    }

    Renderer IEditorTrackableBehaviour.renderer
    {
        get
        {
            return base.renderer;
        }
    }

    Transform IEditorTrackableBehaviour.transform
    {
        get
        {
            return base.transform;
        }
    }

    bool IEditorWordBehaviour.IsSpecificWordMode
    {
        get
        {
            return (this.mMode == WordTemplateMode.SpecificWord);
        }
    }

    bool IEditorWordBehaviour.IsTemplateMode
    {
        get
        {
            return (this.mMode == WordTemplateMode.Template);
        }
    }

    WordTemplateMode IEditorWordBehaviour.Mode
    {
        get
        {
            return this.mMode;
        }
    }

    string IEditorWordBehaviour.SpecificWord
    {
        get
        {
            return this.mSpecificWord;
        }
    }

    public Word Word
    {
        get
        {
            return this.mWord;
        }
    }
}

