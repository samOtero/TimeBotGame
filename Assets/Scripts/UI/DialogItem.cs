using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Item")]
public class DialogItem : ScriptableObject
{
    public string text;
    public bool waitPlayerMoveAction;
    public bool waitPlayerTravelAction;
    public bool exitAfterTime;
    public float exitTime;
    public DialogCharacters character;
}
