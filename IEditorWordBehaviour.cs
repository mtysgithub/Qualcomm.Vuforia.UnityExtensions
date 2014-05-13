using System;

public interface IEditorWordBehaviour : IEditorTrackableBehaviour
{
    void InitializeWord(Word word);
    void SetMode(WordTemplateMode mode);
    void SetSpecificWord(string word);

    bool IsSpecificWordMode { get; }

    bool IsTemplateMode { get; }

    WordTemplateMode Mode { get; }

    string SpecificWord { get; }
}

