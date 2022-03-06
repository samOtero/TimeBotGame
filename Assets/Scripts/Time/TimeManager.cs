using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public int InitialTime;
    public IntVariable CurrentTime;

    public IntEvent ChangeTimeEvent;
    public IntEvent SetTimeEvent;
    public IntEvent TimeChangedEvent;
    // Start is called before the first frame update
    void Start()
    {
        CurrentTime.Value = InitialTime;
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        ChangeTimeEvent.RegisterListener(OnChangeTime);
        SetTimeEvent.RegisterListener(OnSetTime);
    }

    private void UnregisterEvents()
    {
        ChangeTimeEvent.UnregisterListener(OnChangeTime);
        SetTimeEvent.UnregisterListener(OnSetTime);
    }

    public int OnSetTime(int newTime)
    {
        //Check our min and max values
        if (newTime < 0)
            return 0;

        CurrentTime.Value = newTime;

        //Send event that time was changed
        TimeChangedEvent.Raise(CurrentTime.Value);
        return newTime;
    }

    public int OnChangeTime(int timeChange)
    {
        //Change our time
        var newTime = CurrentTime.Value + timeChange;

        //Check our min and max values
        if (newTime < 0)
            return 0;

        return OnSetTime(newTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
