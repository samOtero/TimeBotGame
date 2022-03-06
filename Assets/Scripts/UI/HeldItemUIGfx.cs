using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeldItemUIGfx : MonoBehaviour
{
    public IntEvent ItemGrabbedEvent;
    public IntEvent ItemDroppedEvent;
    public Image itemGfx;
    // Start is called before the first frame update
    void Start()
    {
        itemGfx.enabled = false;
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        ItemGrabbedEvent.RegisterListener(OnItemGrabbed);
        ItemDroppedEvent.RegisterListener(OnItemDropped);
    }

    private void UnregisterEvents()
    {
        ItemGrabbedEvent.UnregisterListener(OnItemGrabbed);
        ItemDroppedEvent.UnregisterListener(OnItemDropped);
    }

    public int OnItemGrabbed(int value)
    {
        itemGfx.enabled = true;
        return value;
    }

    public int OnItemDropped(int value)
    {
        itemGfx.enabled = false;
        return value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
