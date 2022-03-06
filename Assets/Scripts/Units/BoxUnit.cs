public class BoxUnit : UnitContainer
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
        //do nothing, maybe in the future we might change depending on state
    }

    /// <summary>
    /// Resolve how having another unit on top of each other will resolve, button and box will resolve this differently
    /// </summary>
    /// <param name="unitsInSamePlace"></param>
    /// <returns></returns>
    protected override GameOutcome ResolveItemOnTop(UnitContainer unitInSamePlace)
    {
        var loseCondition = GameOutcome.None;

        //If we are on top of a passable unit then that's okay!
        if (unitInSamePlace.isPassable == false)
            loseCondition = GameOutcome.Lose_Inside_Object;

        return loseCondition;
    }

    public void Pushed(int pushedTime, Direction whichDirection)
    {
        var newLocX = LocX;
        var newLocY = LocY;

        var newLoc = GetNewLocation(whichDirection, newLocY, newLocX);
        newLocX = newLoc.Item1;
        newLocY = newLoc.Item2;

        //Add an action right before for recreating in time travel
        storeAction(new UnitStateSnapShot()
        {
            action = action,
            direction = direction,
            LocX = LocX,
            LocY = LocY,
            time = pushedTime - 1,
            type = type,
            vision = null, //No vision needed
            state = new IUnitState(),
            StepGroupId = CurrentStepGroup.Value
        });

        //Add our movement action
        storeAction(new UnitStateSnapShot()
        {
            action = action,
            direction = direction,
            LocX = newLocX,
            LocY = newLocY,
            time = pushedTime,
            type = type,
            vision = null, //No vision
            state = new IUnitState(),
            StepGroupId = CurrentStepGroup.Value
        });

        UpdateState(); //Calling this to update unit
    }

    public bool CanBePushed(Direction whichDirection)
    {
        return isMoveLegal(whichDirection);
    }
}
