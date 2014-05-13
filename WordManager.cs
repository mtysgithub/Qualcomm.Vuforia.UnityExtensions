using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public abstract class WordManager
{
    protected WordManager()
    {
    }

    public abstract void DestroyWordBehaviour(WordAbstractBehaviour behaviour, [Optional, DefaultParameterValue(true)] bool destroyGameObject);
    public abstract IEnumerable<WordResult> GetActiveWordResults();
    public abstract IEnumerable<Word> GetLostWords();
    public abstract IEnumerable<WordResult> GetNewWords();
    public abstract IEnumerable<WordAbstractBehaviour> GetTrackableBehaviours();
    public abstract bool TryGetWordBehaviour(Word word, out WordAbstractBehaviour behaviour);
}

