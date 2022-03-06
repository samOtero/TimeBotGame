using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Group")]
public class DialogItemGroup : ScriptableObject
{
    public List<DialogItem> Items;
    public string levelName;
    public int index;
    public bool TriggerOnSpecificSpot;
    public int row;
    public int col;
    public bool playAfterNoMovement;
    public float noMovementTime;
    public bool TriggerOnSpecificTime;
    public int time;

    public void Reset()
    {
        index = 0;
    }

    public DialogItem GetNextItem()
    {
        DialogItem nextItem = null;

        if (Items != null && Items.Count > index)
        {
            nextItem = Items[index];
            index++;
        }

        return nextItem;
    }
}
