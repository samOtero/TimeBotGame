using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FutureUnitContainer : RobotUnit
{
    public override void Init()
    {
        base.Init();

        //If we have a state then let's store it
        if (InitialAction != null)
        {
            //Don't care about vision for future units
            storeAction(InitialAction);
        }

    }

    protected override IUnitState GetPreviousState()
    {
        if (state == null)
            return null;

        return state.Copy();
    }

    /// <summary>
    /// Called on a unit when they in another unit's vision, this is used to reveal future units and other items
    /// </summary>
    public override void InVision()
    {
        //Reveal unit if in somebody's vision
        ToggleShowing(true);
    }

    /// <summary>
    /// Used to update state in different ways for subclasses
    /// </summary>
    /// <param name="currentAction"></param>
    protected override void CustomUpdateState(UnitStateSnapShot currentAction, bool fromUndo, IUnitState previousState)
    {
        //Do our actions
        base.CustomUpdateState(currentAction, fromUndo, previousState);

        ToggleShowing(false); //Hide future units from sight
    }

    /// <summary>
    /// When player goes in/out of time travel
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override int OnTimeTravel(int value)
    {
        //If we started time travel then let's store our initial actions and hide our future unit
        if (IsTimeTraveling.Value == 1)
        {
            //Let's store our actions incase players want to undo
            var newStorage = new ActionStorage()
            {
                StepGroudIdTiedTo = CurrentStepGroup.Value,
                storedSnapshots = new List<UnitStateSnapShot>()
            };

            //Only pick actions not in current step group
            var actionsToStore = actionList.Where(m => m.StepGroupId != CurrentStepGroup.Value).ToList();
            newStorage.storedSnapshots.AddRange(actionsToStore);
            storedActions.Add(newStorage);

            //Remove those actions from our action list (including zero time for future units)
            actionList.RemoveAll(m => m.StepGroupId != CurrentStepGroup.Value);

            //Add actions to hide future bot, only hidding since we want to recreate him during undo
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.TravelOut,
                direction =  Direction.South,
                LocX = 0,
                LocY = 0,
                time = 0,
                type =  UnitType.FutureClone,
                state = new IUnitState(),
                StepGroupId = CurrentStepGroup.Value
            });

            UpdateState();
        }
        return value;
    }

}
