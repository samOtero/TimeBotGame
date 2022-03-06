using System.Linq;

public class KeyUnit : UnitContainer
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

    public void Grab(int grabTime)
    {
        //Add facing action to item to restore it
        storeAction(new UnitStateSnapShot()
        {
            action = ActionType.Face,
            direction = direction,
            LocX = LocX,
            LocY = LocY,
            time = grabTime - 1,
            type = type,
            state = state.Copy(),
            StepGroupId = CurrentStepGroup.Value
        });

        //Add grabbed action to item
        storeAction(new UnitStateSnapShot()
        {
            action = ActionType.Grabbed,
            direction = direction,
            LocX = LocX,
            LocY = LocY,
            time = grabTime,
            type = type,
            state = state.Copy(),
            StepGroupId = CurrentStepGroup.Value
        });

        UpdateState(); //Calling this to update the key
    }

    public void Drop(int dropTime)
    {

    }

    //public override void storeAction(UnitStateSnapShot state)
    //{
    //    //Remove anything at this time or after since we can change the keycards timeline
    //    var totalRemoved = actionList.RemoveAll(m => m.time >= state.time);
    //    base.storeAction(state);
    //}
}
