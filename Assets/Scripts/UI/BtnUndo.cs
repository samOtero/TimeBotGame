using UnityEngine;

public class BtnUndo : MonoBehaviour
{
    public IntEvent DoUndoEvent;
    public GameObject LoseDialog;
    public void OnPressed()
    {
        DoUndoEvent.Raise(1);

        if (LoseDialog != null)
            Destroy(LoseDialog);
    }
}
