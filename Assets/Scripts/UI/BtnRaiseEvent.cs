using UnityEngine;

public class BtnRaiseEvent : MonoBehaviour
{
    public IntEvent DoEvent;
    public GameObject DestroyThis;
    public IntVariable RaiseValue;
    public void OnPressed()
    {
        var raise = 1;
        if (RaiseValue != null)
            raise = RaiseValue.Value;

        DoEvent.Raise(raise);

        if (DestroyThis != null)
            Destroy(DestroyThis);
    }
}
