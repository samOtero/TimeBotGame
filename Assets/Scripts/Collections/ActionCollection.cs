using System;
using UnityEngine;

/// <summary>
/// Basic list for unit
/// </summary>
[Serializable]
[CreateAssetMenu(menuName = "Collections/Actions")]
public class ActionCollection : Collection<UnitAction>
{
    public void Reset()
    {
        if (Items == null)
            Items = new System.Collections.Generic.List<UnitAction>();

        Items.Clear();
    }
}