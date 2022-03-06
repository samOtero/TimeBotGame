using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravelUIGfx : MonoBehaviour
{
    public Animator gfxAnimator;
    public IntEvent OnTimeTravelEvent;
    public IntVariable IsTimeTraveling;
    // Start is called before the first frame update
    void Start()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        OnTimeTravelEvent.RegisterListener(OnTimeTraveling);
    }

    private void UnregisterEvents()
    {
        OnTimeTravelEvent.UnregisterListener(OnTimeTraveling);
    }

    public int OnTimeTraveling(int direction)
    {
        UpdateStatus();
        return direction;
    }

    private void UpdateStatus()
    {
        var animation = "TimeTravelUI_Off";
        if (IsTimeTraveling.Value == 1)
            animation = "TimeTravelUI_On";

        gfxAnimator.Play(animation);

    }
}
