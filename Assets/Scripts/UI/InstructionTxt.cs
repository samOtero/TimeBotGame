using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InstructionTxt : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string normalInstruction;
    public string timeDimensionInstruction;
    public IntEvent OnTimeTravelEvent;
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
        OnTimeTravelEvent.RegisterListener(OnTimeTravel);
    }

    private void UnregisterEvents()
    {
        OnTimeTravelEvent.UnregisterListener(OnTimeTravel);
    }

    public int OnTimeTravel(int count)
    {
        UpdateStatus();
        return count;
    }

    private void UpdateStatus()
    {
        //var txt = string.Format("Bombs left: {0}", bombsLeft);

       // text.SetText(txt);
    }
}
