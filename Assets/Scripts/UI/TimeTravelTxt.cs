using TMPro;
using UnityEngine;

public class TimeTravelTxt : MonoBehaviour
{
    public TextMeshProUGUI text;
    public IntVariable IsTimeTraveling;
    public IntEvent TimeTravelingEvent;
    public bool isInit;
    // Start is called before the first frame update
    void Start()
    {
        isInit = false;
        RegisterEvents();
    }

    private void Update()
    {
        if (isInit == false)
        {
            isInit = true;
            UpdateStatus();
        }
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        TimeTravelingEvent.RegisterListener(OnTimeTraveling);
    }

    private void UnregisterEvents()
    {
        TimeTravelingEvent.UnregisterListener(OnTimeTraveling);
    }

    public int OnTimeTraveling(int direction)
    {
        UpdateStatus();
        return direction;
    }

    private void UpdateStatus()
    {
        var newStatus = "";
        if (IsTimeTraveling.Value == 1)
            newStatus = "Time Traveling";

        text.SetText(newStatus);
    }
}
