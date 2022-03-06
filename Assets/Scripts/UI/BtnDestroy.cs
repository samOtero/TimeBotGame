using UnityEngine;
using UnityEngine.EventSystems;

public class BtnDestroy : MonoBehaviour, IPointerClickHandler
{
    public GameObject toDestroy;
    public IntEvent beforeDestroyEvent;
    public bool clicked;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clicked == true)
            return;

        clicked = true;
        if (beforeDestroyEvent != null)
            beforeDestroyEvent.Raise(1);

        Destroy(toDestroy);
    }
}
