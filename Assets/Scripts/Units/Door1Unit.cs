using System.Linq;

public class Door1Unit : UnitContainer
{
    public override void Init()
    {
        base.Init();

        //If we have a state then let's store it
        if (InitialAction != null)
        {
            storeAction(InitialAction);
        }
    }

    /// <summary>
    /// Update a unit's graphic for the current action
    /// </summary>
    /// <param name="whichAction"></param>
    protected override void UpdateGraphic(UnitStateSnapShot whichAction)
    {
        //do nothing, will handle this on time change
    }

    /// <summary>
    /// Used to update state in different ways for subclasses
    /// </summary>
    /// <param name="currentAction"></param>
    protected override void CustomUpdateState(UnitStateSnapShot currentAction, bool fromUndo, IUnitState previousState)
    {
        //if (currentAction.action == ActionType.Open)
        //{
        //    state.closed = false;
        //}
        //else if (currentAction.action == ActionType.Closed)
        //{
        //    state.closed = true;
        //}

        isPassable = false;
        if (state.closed == false)
            isPassable = true;
    }

    public void Open(int openTime)
    {
        if (state.closed == true)
        {

            //Add an closing action right before for re-closing in time travel
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Closed,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = openTime-1,
                type = type,
                vision = null, //No vision needed
                state = new IUnitState() { closed = true },
                StepGroupId = CurrentStepGroup.Value
            });

            //Add our open action
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Open,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = openTime,
                type = type,
                vision = null, //No vision for bomb,
                state = new IUnitState() { closed = false },
                StepGroupId = CurrentStepGroup.Value
            });

            UpdateState(); //Calling this to update unit
        }
    }

    public void Close(int closeTime)
    {
        if (state.closed == false)
        {

            //Add an open action right before for re-opening in time travel
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Open,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = closeTime-1,
                type = type,
                vision = null, //No vision needed
                state = new IUnitState() { closed = false },
                StepGroupId = CurrentStepGroup.Value
            });

            //Add our closed action
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Closed,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = closeTime,
                type = type,
                vision = null, //No vision needed
                state = new IUnitState() { closed = true },
                StepGroupId = CurrentStepGroup.Value
            });

            UpdateState(); //Calling this to update unit
        }
    }
}
