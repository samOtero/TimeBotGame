using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepGroupManager : MonoBehaviour
{
    public IntVariable CurrentStepGroup;
    public List<StepGroup> Groups;
    public IntEvent DoUndoEvent;
    public IntEvent OnNewStepGroupEvent;
    public UnitCollection unitList;
    public IntEvent TimeSetEvent;
    public IntEvent DoTimeTravelEvent;
    public IntVariable CurrentTime;
    public IntVariable TimeChangeFromUndo;
    public IntVariable IsTimeTraveling;
    public IntVariable IsFreeFormTraveling;
    public IntVariable BlockUndo;

    // Start is called before the first frame update
    void Start()
    {
        CurrentStepGroup.Value = 0; //Reset our group step
        DoUndoEvent.RegisterListener(OnUndoEvent);
        OnNewStepGroupEvent.RegisterListener(DoNewStepGroup);

    }

    private void OnDisable()
    {
        DoUndoEvent.UnregisterListener(OnUndoEvent);
        OnNewStepGroupEvent.UnregisterListener(DoNewStepGroup);
    }

    public int OnUndoEvent(int value)
    {
        if (BlockUndo.Value > 0)
            return value;

        //Don't allow undo when time traveling to the start
        if (IsTimeTraveling.Value > 0 && IsFreeFormTraveling.Value <= 0)
            return value;

        if (Groups.Count == 0)
            return value;

        TimeChangeFromUndo.Value = 1; //Make sure we know time changes are coming from the Undo

        var latestGroup = Groups[Groups.Count - 1];

        for(var i = unitList.Items.Count-1; i>=0; i--)
        {
            var unit = unitList.Items[i];
            unit.UndoStepGroup(latestGroup.GroupId, latestGroup.PreviousTime);
        }


        //Toggle Time Dimension
        var InTimeDimension = latestGroup.InTimeDimension ? 1 : 0;
        var currentValue = IsTimeTraveling.Value;
        IsFreeFormTraveling.Value = InTimeDimension;
        IsTimeTraveling.Value = InTimeDimension;

        //If we swapped from time dimension then raise event
        if (currentValue != InTimeDimension)
            DoTimeTravelEvent.Raise(InTimeDimension);

        //Set our time to the previous one before the group started
        TimeSetEvent.Raise(latestGroup.PreviousTime);

        

        //Remove our group
        Groups.Remove(latestGroup);
        return value;
    }

    /// <summary>
    /// Create a new step group
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int DoNewStepGroup(int value)
    {
        CurrentStepGroup.Value++;
        var newStepGroup = new StepGroup()
        {
            GroupId = CurrentStepGroup.Value,
            PreviousTime = CurrentTime.Value,
            InTimeDimension = IsFreeFormTraveling.Value > 0
        };
        Groups.Add(newStepGroup);

        return value;
    }
}
