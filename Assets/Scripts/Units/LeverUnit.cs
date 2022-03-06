using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeverUnit : UnitContainer
{
    protected Door1Unit connectedDoor;

    public override void Init()
    {
        base.Init();

        //If we have a state then let's store it
        if (InitialAction != null)
        {
            storeAction(InitialAction);
        }

        //Get our door, eventually will need additional
        connectedDoor = levelUnits.Items.Where(m => m is Door1Unit).FirstOrDefault() as Door1Unit;
    }

    /// <summary>
    /// Update a unit's graphic for the current action
    /// </summary>
    /// <param name="whichAction"></param>
    protected override void UpdateGraphic(UnitStateSnapShot whichAction)
    {
        if (gfxAnimator != null)
        {
            if (state.pressed == true)
                gfxAnimator.Play("lever_on");
            else
                gfxAnimator.Play("lever_off");
        }
    }

    /// <summary>
    /// Used to update state in different ways for subclasses
    /// </summary>
    /// <param name="currentAction"></param>
    //protected override void CustomUpdateState(UnitStateSnapShot currentAction, bool fromUndo, IUnitState previousState)
    //{
    //    if (currentAction.action == ActionType.Pressed)
    //    {
    //        state.pressed = true;
    //    }
    //    else if (currentAction.action == ActionType.Depressed)
    //    {
    //        state.pressed = false;
    //    }
    //}

    public void Pressed(int pressedTime)
    {
        if (state.pressed == false)
        {

            //Add an action right before for recreating in time travel
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Depressed,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = pressedTime - 1,
                type = type,
                vision = null, //No vision needed
                state = new IUnitState() { pressed = false },
                StepGroupId = CurrentStepGroup.Value
            });

            //Add our open action
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Pressed,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = pressedTime,
                type = type,
                vision = null, //No vision
                state = new IUnitState() { pressed = true },
                StepGroupId = CurrentStepGroup.Value
            });

            UpdateState(); //Calling this to update unit

            if (connectedDoor)
            {
                connectedDoor.Open(pressedTime);
            }
        }
    }

    public void Depressed(int pressedTime)
    {
        if (state.pressed == true)
        {

            //Add an action right before for recreating in time travel
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Pressed,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = pressedTime - 1,
                type = type,
                vision = null, //No vision needed
                state = new IUnitState() { pressed = true },
                StepGroupId = CurrentStepGroup.Value
            });

            //Add our open action
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Depressed,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = pressedTime,
                type = type,
                vision = null, //No vision
                state = new IUnitState() { pressed = false },
                StepGroupId = CurrentStepGroup.Value
            });

            UpdateState(); //Calling this to update unit

            if (connectedDoor)
            {
                connectedDoor.Close(pressedTime);
            }
        }
    }
}
