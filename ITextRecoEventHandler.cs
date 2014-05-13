using System;

public interface ITextRecoEventHandler
{
    void OnInitialized();
    void OnWordDetected(WordResult word);
    void OnWordLost(Word word);
}

