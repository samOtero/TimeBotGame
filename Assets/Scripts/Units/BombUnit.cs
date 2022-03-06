using TMPro;

public class BombUnit : UnitContainer
{
    public IntEvent BombDefusedEvent;
    public IntEvent DoCameraShakeBombEvent;
    public TextMeshProUGUI timer;
    public int MaxTime;
    public int TimeLeft;

    public override void Init()
    {
        base.Init();

        //If we have a state then let's store it
        if (InitialAction != null)
        {
            storeAction(InitialAction);
        }            
    }

    private void updateTime()
    {
        TimeLeft = MaxTime - CurrentTime.Value;
        var timeTxt = TimeLeft.ToString();
        timer.SetText(timeTxt);

        if (gfxAnimator != null)
        {
            gfxAnimator.SetInteger("TimeLeft", TimeLeft);
        }

        if (TimeLeft <= 0 && state.armed == true)
        {
            DoCameraShakeBombEvent.Raise(1);
        }
    }

    public bool aboutToBlow()
    {
        if (TimeLeft <= 1 && state.armed == true)
            return true;

        return false;
    }

    public void DisarmBomb(int disarmTime)
    {
        if (state.armed == true)
        {

            //Add an arm action right before for re-arming in time travel
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Armed,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = disarmTime-1,
                type = type,
                vision = null, //No vision for bomb
                state = new IUnitState() { armed = true},
                StepGroupId = CurrentStepGroup.Value
            });

            //Add our defuse action
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Defused,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = disarmTime,
                type = type,
                vision = null, //No vision for bomb,
                state = new IUnitState() { armed = false},
                StepGroupId = CurrentStepGroup.Value
            });

            UpdateState(); //Calling this to update the bomb

            BombDefusedEvent.Raise(1);
        }
    }

    /// <summary>
    /// Checks to see if we are in a losing position, bomb will set the condition if it's time has passed
    /// </summary>
    /// <returns></returns>
    public override GameOutcome CheckForLoseCondition()
    {
        var loseCondition = base.CheckForLoseCondition();

        //If we are armed and the player is not time traveling
        if (state.armed == true)
        {
            if (CurrentTime.Value >= MaxTime)
                loseCondition = GameOutcome.Lose_BombExplode;
        }
        
        return loseCondition;
    }

    /// <summary>
    /// Used to update state in different ways for subclasses
    /// </summary>
    /// <param name="currentAction"></param>
    protected override void CustomUpdateState(UnitStateSnapShot currentAction, bool fromUndo, IUnitState previousState)
    {
        var statusChanged = false;

        //If our armed state changed
        if (state.armed != previousState.armed)
            statusChanged = true;       

        //Send the event if our status changed
        if (statusChanged == true)
            BombDefusedEvent.Raise(1);
    }

    protected override void CustomUpdatePostState(UnitStateSnapShot currentAction = null)
    {
        //Update our bomb time after the state is updated
        updateTime();
    }

    /// <summary>
    /// Update a unit's graphic for the current action
    /// </summary>
    /// <param name="whichAction"></param>
    protected override void UpdateGraphic(UnitStateSnapShot whichAction)
    {
        //do nothing, will handle this on time change
    }
}
