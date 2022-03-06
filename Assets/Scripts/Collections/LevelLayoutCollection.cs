using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic list for level layouts
/// </summary>
[Serializable]
[CreateAssetMenu(menuName = "Collections/Level Layouts")]
public class LevelLayoutCollection : Collection<LevelData>
{
    public int currentVersion;
    public string List;
    public void Reset()
    {
        if (Items == null)
            Items = new List<LevelData>();

        Items.Clear();
    }
}