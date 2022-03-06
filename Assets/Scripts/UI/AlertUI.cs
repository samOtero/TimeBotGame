using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertUI : MonoBehaviour
{
    public IntEvent OnCurrentOutcomeChangedEvent;
    public OutcomeVariable CurrentOutcome;
    public Animator alertAnimator;
    public string OffAnimationName;
    public string OnAnimationName;

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
        OnCurrentOutcomeChangedEvent.RegisterListener(OnCurrentOutcomeChanged);
    }

    private void UnregisterEvents()
    {
        OnCurrentOutcomeChangedEvent.UnregisterListener(OnCurrentOutcomeChanged);
    }

    public int OnCurrentOutcomeChanged(int value)
    {
        if (CurrentOutcome.Value == GameOutcome.None)
        {
            alertAnimator.Play(OffAnimationName);
        }
        else
        {
            alertAnimator.Play(OnAnimationName);
        }
        return value;
    }
}
