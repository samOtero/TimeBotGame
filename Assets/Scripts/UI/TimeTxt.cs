using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeTxt : MonoBehaviour
{
    public IntVariable MaxTime;
    public IntVariable CurrentTime;
    public IntEvent TimeChangedEvent;
    public TextMeshProUGUI text;
    public bool isInit;

    // Start is called before the first frame update
    void Start()
    {
        isInit = false;
        RegisterEvent();
    }

    public void Update()
    {
        if (isInit == false)
        {
            isInit = true;
            updateTime();
        }
    }

    private void OnDisable()
    {
        UnregisterEvent();
    }

    private void RegisterEvent()
    {
        TimeChangedEvent.RegisterListener(OnTimeChanged);
    }

    private void UnregisterEvent()
    {
        TimeChangedEvent.UnregisterListener(OnTimeChanged);
    }

    public int OnTimeChanged(int newTime)
    {
        updateTime();
        return newTime;
    }

    private void updateTime()
    {
        var timeList = MaxTime.Value - CurrentTime.Value;
        text.SetText("Time: " + timeList.ToString());
    }
}
