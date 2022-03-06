using System;
using UnityEngine;

/// <summary>
/// Basic list for level blocks
/// </summary>
[Serializable]
[CreateAssetMenu(menuName = "Collections/Block")]
public class BlockCollection : Collection<Block>
{

    public void Reset()
    {
        Items.Clear();
    }
}