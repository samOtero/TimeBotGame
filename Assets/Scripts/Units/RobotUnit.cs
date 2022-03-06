using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Parent class for robot units: past, future, present. Will contain some basic functionality 
/// </summary>
public class RobotUnit : UnitContainer
{
    public GameObject KeycardPrefab;
    /// <summary>
    /// Checks to see if we are in a losing position, only past robots will give a condition
    /// </summary>
    /// <returns></returns>
    public override GameOutcome CheckForLoseCondition()
    {
        var loseCondition = GameOutcome.None;
        return loseCondition;
    }

    /// <summary>
    /// When player goes in/out of time travel
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override int OnTimeTravel(int value)
    {
        //Robots will not need to delete/store their future actions as we do want them to be played the same
        //Player unit handles his own delete/storing when 
        return value;
    }

    /// <summary>
    /// When player goes in/out of time travel
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override int OnTravelBackwards(int value)
    {
        //Robots will not need to delete/store their future actions as we do want them to be played the same
        //Player unit handles his own delete/storing when 
        return value;
    }

    /// <summary>
    /// Used to update state in different ways for subclasses
    /// </summary>
    /// <param name="currentAction"></param>
    protected override void CustomUpdateState(UnitStateSnapShot currentAction, bool fromUndo, IUnitState previousState)
    {
        //Handle any actions for robots
        HandleActions(currentAction, fromUndo, previousState);
    }

    /// <summary>
    /// Handle the different actions the robots can do to other items
    /// </summary>
    /// <param name="currentAction"></param>
    /// <param name="fromUndo"></param>
    protected void HandleActions(UnitStateSnapShot currentAction, bool fromUndo, IUnitState previousState)
    {
        //If we are doing an undo we don't need to recreate events
        if (fromUndo == true)
            return;
        if (TimeChangeFromReturnTimeTravel.Value == 1 || TimeChangeFromUndo.Value == 1)
            return;

        

        if (currentAction.action == ActionType.DoDefuseBomb)
            DoDefuseBomb();
        else if (currentAction.action == ActionType.DoOpenDoor)
            DoOpenDoor();
        else if (currentAction.action == ActionType.DoPushBox)
            DoPushBox();
        else if (currentAction.action == ActionType.GrabKey)
            DoGrabKey();
        else if (currentAction.action == ActionType.DoPressLever)
            DoPushLever();
        else if (currentAction.action == ActionType.DropKey)
            DoDropKey();
    }

    private void DoDropKey()
    {
        if (IsTimeTraveling.Value == 1)
            return;

        var newLoc = GetNewLocation(direction, LocY, LocX);

        //Drop item on new spot
        CreateItem(newLoc.Item1, newLoc.Item2, CurrentTime.Value);
    }

    private void DoGrabKey()
    {
        if (IsTimeTraveling.Value == 1)
            return;

        //First check if there is a item to grab
        var itemToGrab = GetItemToGrab();
        if (itemToGrab == null)
            return;

        itemToGrab.Grab(CurrentTime.Value);
    }

    /// <summary>
    /// Push Lever Action
    /// </summary>
    private void DoPushLever()
    {
        if (IsTimeTraveling.Value == 1)
            return;

        var lever = GetLeverToPush();
        if (lever == null)
            return;

        var newTime = CurrentTime.Value;

        if (lever.state.pressed == false)
            lever.Pressed(newTime);
        else
            lever.Depressed(newTime);
    }

    /// <summary>
    /// Push box action
    /// </summary>
    private void DoPushBox()
    {
        if (IsTimeTraveling.Value == 1)
            return;

        var box = GetBoxToPush();
        if (box == null)
            return;

        box.Pushed(CurrentTime.Value, direction);
    }

    /// <summary>
    /// Open Door Action
    /// </summary>
    private void DoOpenDoor()
    {
        if (IsTimeTraveling.Value == 1)
            return;

        var door1 = GetDoorToOpen();
        if (door1 == null)
            return;

        door1.Open(CurrentTime.Value);
    }

    /// <summary>
    /// Defuse Bomb Action
    /// </summary>
    private void DoDefuseBomb()
    {
        if (IsTimeTraveling.Value == 1)
            return;

        var bomb = GetBombToDefuse();
        if (bomb == null)
            return;

        bomb.DisarmBomb(CurrentTime.Value);        
    }

    /// <summary>
    /// Returns the bomb infront of us to defuse, otherwise null
    /// </summary>
    /// <returns></returns>
    protected BombUnit GetBombToDefuse()
    {
        var itemInFront = GetItemInFront();
        if (itemInFront == null || itemInFront.type != UnitType.Bomb)
            return null;

        var bomb = itemInFront as BombUnit;

        if (bomb.state.armed == false)
            return null;

        return bomb;
    }

    /// <summary>
    /// Returns a door infront of us to open, otherwise null
    /// </summary>
    /// <returns></returns>
    protected Door1Unit GetDoorToOpen()
    {
        var itemInFront = GetItemInFront();
        if (itemInFront == null || itemInFront.type != UnitType.Door1)
            return null;

        var door1 = itemInFront as Door1Unit;

        //If door is open then don't open it again
        if (door1.state.closed == false)
            return null;

        return door1;
    }

    /// <summary>
    /// Returns a box that we can push, otherwise null
    /// </summary>
    /// <returns></returns>
    protected BoxUnit GetBoxToPush()
    {
        var itemsInFront = GetItemsInFront();
        BoxUnit box = null;
        foreach(var item in itemsInFront)
        {
            if (item.type != UnitType.Box)
                continue;

            box = item as BoxUnit;
            break;
        }
        if (box == null)
            return null;

        //check to see if we can push the box
        if (box.CanBePushed(direction) == false)
            return null;

        return box;
    }

    /// <summary>
    /// Returns a Lever that we can push, otherwise null
    /// </summary>
    /// <returns></returns>
    protected LeverUnit GetLeverToPush()
    {
        var itemInFront = GetItemInFront();
        if (itemInFront == null || itemInFront.type != UnitType.Lever)
            return null;

        var lever = itemInFront as LeverUnit;

        return lever;
    }

    /// <summary>
    /// Get the item to grab that is in front of the player
    /// </summary>
    /// <returns></returns>
    protected KeyUnit GetItemToGrab()
    {
        KeyUnit keycard = null;
        var unit = GetItemInFront();
        if (unit != null && unit.type == UnitType.Keycard)
            keycard = unit as KeyUnit;

        return keycard;
    }

    /// <summary>
    /// Get item infront of unit
    /// </summary>
    /// <returns></returns>
    protected UnitContainer GetItemInFront()
    {
        var unit = GetItemsInFront().FirstOrDefault();

        return unit;
    }

    protected IEnumerable<UnitContainer> GetItemsInFront()
    {
        var newLoc = GetNewLocation(direction, LocY, LocX);

        return levelUnits.Items.Where(m => m.LocX == newLoc.Item1 && m.LocY == newLoc.Item2 && UnitIsVisible(m.action) == true);
    }

    /// <summary>
    /// Create the dropped item and give it the appropriate actions
    /// </summary>
    /// <param name="newLocX"></param>
    /// <param name="newLocY"></param>
    private void CreateItem(int newLocX, int newLocY, int timeDropped)
    {
        var item = Instantiate(KeycardPrefab);
        var itemScript = item.GetComponent<KeyUnit>();
        itemScript.actionList.Clear();
        itemScript.InitialAction = null; //remove any initial action

        //Add dropped action, to help us recreate during time travel
        itemScript.storeAction(new UnitStateSnapShot()
        {
            action = ActionType.Dropped,
            direction = itemScript.direction,
            LocX = newLocX,
            LocY = newLocY,
            time = timeDropped-1,
            type = itemScript.type,
            state = new IUnitState(),
            StepGroupId = CurrentStepGroup.Value
        });

        //Add facing action, to display the item
        itemScript.storeAction(new UnitStateSnapShot()
        {
            action = ActionType.Face,
            direction = itemScript.direction,
            LocX = newLocX,
            LocY = newLocY,
            time = timeDropped,
            type = itemScript.type,
            state = new IUnitState(),
            StepGroupId = CurrentStepGroup.Value
        });
        itemScript.Init(); //Init our unit here so it can register to the correct listeners
        itemScript.UpdateState();
    }
}
