using System;
using UnityEngine;

/// <summary>
/// Basic list for chapters
/// </summary>
[Serializable]
[CreateAssetMenu(menuName = "Collections/Chapters")]
public class ChapterCollection : Collection<ChapterVariable>
{
    public void Reset()
    {
        if (Items == null)
            Items = new System.Collections.Generic.List<ChapterVariable>();

        Items.Clear();
    }
}