using System;

public interface IEditorTextRecoBehaviour
{
    string AdditionalCustomWords { get; set; }

    string AdditionalFilterWords { get; set; }

    string CustomWordListFile { get; set; }

    string FilterListFile { get; set; }

    WordFilterMode FilterMode { get; set; }

    int MaximumWordInstances { get; set; }

    string WordListFile { get; set; }

    WordPrefabCreationMode WordPrefabCreationMode { get; set; }
}

