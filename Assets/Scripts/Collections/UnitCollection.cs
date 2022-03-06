using System;
using UnityEngine;

/// <summary>
/// Basic list for unit
/// </summary>
[Serializable]
[CreateAssetMenu(menuName = "Collections/Unit")]
public class UnitCollection : Collection<UnitContainer>
{

    public void Reset()
    {
        Items.Clear();
    }
}