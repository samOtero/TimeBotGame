using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUnitContainer : RobotUnit
{
    public IntEvent PlayerActionEvent;
    public IntEvent ChangeTimeEvent;
    public IntEvent ItemGrabbedEvent;
    public IntEvent ItemDroppedEvent;
    public IntEvent DoStage5ControlAutoTravelEvent;
    public IntEvent DoStage3VisionEvent;
    public IntEvent DoNewStepGroupEvent;
    public IntEvent DoGetOutcomeEvent;
    public IntEvent OnGetOutcomeEvent;
    public OutcomeVariable CurrentOutcome;
    public GameObject ClonePrefab;
    public IntVariable PlayerActiveInput;
    public IntVariable HideTimeTravelDialog;
    public IntVariable BlockUndo;
    public int isTravelingBack;
    public int travelingBackFrameBuffer;
    public int travelingRollBackFrameBuffer;
    public int minBuffer;
    public int currentBuffer;
    public int bufferCounter;
    public int freeFormTravelingForward;
    public int freeFormTravelingBackward;

    protected override void RegisterEvents()
    {
        base.RegisterEvents();
        PlayerActionEvent.RegisterListener(OnPlayerAction);
        DoStage5ControlAutoTravelEvent.RegisterListener(OnStage5AutoTravel);
        DoStage3VisionEvent.RegisterListener(OnStage3Vision);
    }

    protected override void UnregisterEvents()
    {
        base.UnregisterEvents();
        PlayerActionEvent.UnregisterListener(OnPlayerAction);
        DoStage5ControlAutoTravelEvent.UnregisterListener(OnStage5AutoTravel);
        DoStage3VisionEvent.UnregisterListener(OnStage3Vision);
    }

    public override void Init()
    {
        base.Init();

        isTravelingBack = 0; //Reset this
        freeFormTravelingForward = 0; //Reset this
        freeFormTravelingBackward = 0; //reset
        IsTimeTraveling.Value = 0; //reset this value

        //If we have a state then let's store it
        if (InitialAction != null)
        {
            storeAction(InitialAction);
        }
            
    }

    /// <summary>
    /// Run update after the state is updated
    /// </summary>
    public int OnStage5AutoTravel(int value)
    {
        if (isTravelingBack > 0)
        {
            PlayerActiveInput.Value = 0; //Remove player input when traveling back
            if (bufferCounter <= 0)
                DoTravelToStartUpdate();
            else
                bufferCounter--;
        }
        else if (freeFormTravelingBackward == 1)
        {
            freeFormTravelingBackward = 0;
        }
        else if (freeFormTravelingForward == 1) //if we travelled forward in time
        {
            DoGetOutcomeEvent.Raise(1);
            if (CurrentOutcome.Value != GameOutcome.None) //If we would lose the level then we start going back in time
            {
                OnGetOutcomeEvent.Raise(1); //Send out event that current outcome is bad

                PlayerActiveInput.Value = 0; //Remove player input when traveling back
                BlockUndo.Value = 1; //block undoing while we are traveling back
                freeFormTravelingForward = 2; //second flag
                //Reset our buffer
                currentBuffer = travelingRollBackFrameBuffer;
                bufferCounter = currentBuffer;
            }
            else
            {
                freeFormTravelingForward = 0;
            }
        }
        else if (freeFormTravelingForward == 2) //if we had a losing outcome then go back in time
        {
            PlayerActiveInput.Value = 0; //Remove player input when traveling back
            if (bufferCounter <= 0)
            {
                //Go back in time once
                freeFormTravelingForward = 0;
                var timeChange = -1;
                BlockUndo.Value = 0; //Unblock the undo once we go to the next time
                ChangeTimeEvent.Raise(timeChange);
            }                
            else
                bufferCounter--;
        }

        return value;
    }

    public int OnPlayerAction(int action)
    {
        //Block any actions while we are traveling back
        if (isTravelingBack > 0)
            return action;

        var newAction = (PlayerActions)action;
        var timeChange = 1; //Most actions move us forward

        //Handle coming in our out of time travel dimension
        if (newAction == PlayerActions.ToggleTimeTravel)
        {
            //Get a new set group
            DoNewStepGroupEvent.Raise(1);

            if (IsTimeTraveling.Value == 0)
            {
                IsFreeFormTraveling.Value = 1;
                return DoTimeTraveling(true);
            }
            else
            {
                IsFreeFormTraveling.Value = 0;
                return DoReturnFromTimeTravel();
            }
                
        }
        else if (newAction == PlayerActions.TravelToStart)
        {
            //Get a new set group
            DoNewStepGroupEvent.Raise(1);

            return DoTravelToStartInit();
        }
        else if (newAction == PlayerActions.TravelBackward || newAction == PlayerActions.TravelForward)
        {
            //Don't need to create a new group for this as we don't save actions either

            //If we aren't in trime travel then don't do anthing
            if (IsTimeTraveling.Value == 0)
                return 0;

            //Get a new set group
            DoNewStepGroupEvent.Raise(1);

            if (newAction == PlayerActions.TravelBackward)
            {
                timeChange = -1;
                freeFormTravelingBackward = 1;
            }
            else
                freeFormTravelingForward = 1;//flag for traveling forward

            ChangeTimeEvent.Raise(timeChange);
            return timeChange;
        }
        else if (newAction == PlayerActions.Wait)
        {
            if (IsTimeTraveling.Value == 1)
                return 0;

            //Trigger a new step group
            DoNewStepGroupEvent.Raise(1);

            //Waiting will just move time up by one
            Add_Basic_Action(CurrentTime.Value + timeChange);

            ChangeTimeEvent.Raise(timeChange);
            return timeChange;
        }
        else if (newAction == PlayerActions.DoAction)
        {
            //This is our general action button, try our different actions in a certain order, if one of them completes then stop
            var result = 0;
            if (result == 0)
                result = DoDefuseBomb(timeChange);
            if (result == 0)
                result = DoOpenDoor(timeChange);
            if (result == 0)
                result = DoPushBox(timeChange);
            if (result == 0)
                result = DoGrabItem(timeChange);
            if (result == 0)
                result = DoDropItem(timeChange);
            if (result == 0)
                result = DoPressLever(timeChange);

            return result;
        }
        else
        {
            if (IsTimeTraveling.Value == 1)
                return 0;

            var newDir = Direction.North;
            if (newAction == PlayerActions.WalkEast)
                newDir = Direction.East;
            else if (newAction == PlayerActions.WalkWest)
                newDir = Direction.West;
            else if (newAction == PlayerActions.WalkSouth)
                newDir = Direction.South;

            //First check if move is legal
            if (isMoveLegal(newDir) == false)
            {
                if (newDir == direction)
                    return 0;

                //Trigger a new step group
                DoNewStepGroupEvent.Raise(1);

                //Rotate to direction
                direction = newDir;
                Add_Basic_Action(CurrentTime.Value + timeChange);

                //Move Time Forward
                ChangeTimeEvent.Raise(timeChange);
                return timeChange;
            }

            //Trigger a new step group
            DoNewStepGroupEvent.Raise(1);

            //Add our move action
            Add_Move_Action(CurrentTime.Value + timeChange, newDir);

            //Move Time Forward
            ChangeTimeEvent.Raise(timeChange);
        }

        return timeChange;
    }

    private int DoPressLever(int timeChange)
    {
        if (IsTimeTraveling.Value == 1)
            return 0;

        var itemInFront = GetItemInFront();
        if (itemInFront == null || itemInFront.type != UnitType.Lever)
            return 0;

        //Trigger a new step group
        DoNewStepGroupEvent.Raise(1);

        //var lever = itemInFront as LeverUnit;

        var newTime = CurrentTime.Value + 1;

        //if (lever.state.pressed == false)
        //    lever.Pressed(newTime);
        //else
        //    lever.Depressed(newTime);

        //Add a basic store action for our next step
        Add_Basic_Action(newTime, ActionType.DoPressLever);

        //Move Time Forward
        ChangeTimeEvent.Raise(timeChange);

        return timeChange;
    }

    private int DoDefuseBomb(int timeChange)
    {
        if (IsTimeTraveling.Value == 1)
            return 0;

        var bomb = GetBombToDefuse();
        if (bomb == null)
            return 0;

        //Trigger a new step group
        DoNewStepGroupEvent.Raise(1);

        var newTime = CurrentTime.Value + 1;
        //Add a basic store action for our next step
        Add_Basic_Action(newTime, ActionType.DoDefuseBomb);

        //Move Time Forward to actually perform our action
        ChangeTimeEvent.Raise(timeChange);

        return timeChange;
    }

    private int DoOpenDoor(int timeChange)
    {
        if (IsTimeTraveling.Value == 1)
            return 0;

        if (state.holdingKeyCard == false)
            return 0;

        var door1 = GetDoorToOpen();
        if (door1 == null)
            return 0;

        //Trigger a new step group
        DoNewStepGroupEvent.Raise(1);

        var newTime = CurrentTime.Value + 1;

        var newState = state.Copy();
        newState.holdingKeyCard = false;
        Add_Basic_Action(newTime, ActionType.DoOpenDoor, null, newState);

        //Move Time Forward
        ChangeTimeEvent.Raise(timeChange);

        return timeChange;
    }

    /// <summary>
    /// Do grab item action
    /// </summary>
    /// <param name="timechange"></param>
    /// <returns></returns>
    private int DoGrabItem(int timeChange)
    {
        if (IsTimeTraveling.Value == 1)
            return 0;

        if (state.holdingKeyCard == true)
            return 0;

        //First check if there is a item to grab
        var itemToGrab = GetItemToGrab();
        if (itemToGrab == null)
            return 0;

        //Create a new step group
        DoNewStepGroupEvent.Raise(1);

        var newTime = CurrentTime.Value + 1;

        //Add a basic store action for our next step
        //Add_Basic_Action(newTime - 1, ActionType.DropKey, vision); //Send in current vision since this is our current step

        var newState = state.Copy();
        newState.holdingKeyCard = true;
        Add_Basic_Action(newTime, ActionType.GrabKey, null, newState);

        //Move Time Forward
        ChangeTimeEvent.Raise(timeChange);
        return timeChange;
    }

    /// <summary>
    /// Do push block action
    /// </summary>
    /// <param name="timechange"></param>
    /// <returns></returns>
    private int DoPushBox(int timeChange)
    {
        if (IsTimeTraveling.Value == 1)
            return 0;

        var box = GetBoxToPush();
        if (box == null)
            return 0;

        var newTime = CurrentTime.Value + 1;

        //Trigger a new step group
        DoNewStepGroupEvent.Raise(1);

        //Add push action
        Add_Basic_Action(newTime, ActionType.DoPushBox);

        //Move Time Forward
        ChangeTimeEvent.Raise(timeChange);
        return timeChange;
    }

    /// <summary>
    /// Do drop item action
    /// </summary>
    /// <param name="timeChange"></param>
    /// <returns></returns>
    private int DoDropItem(int timeChange)
    {
        if (IsTimeTraveling.Value == 1)
            return 0;

        if (state.holdingKeyCard == false)
            return 0;

        //Validate if we can put the item there
        if (isMoveLegal(direction, true) == false)
            return 0;

        //Trigger a new step group
        DoNewStepGroupEvent.Raise(1);

        //Get location where we would drop the item
        //var newLoc = GetNewLocation(direction, LocY, LocX);

        //Drop item on new spot
        //CreateItem(newLoc.Item1, newLoc.Item2);

        //Add a basic action for our next step
        var newTime = CurrentTime.Value + 1;
        //Add_Basic_Action(newTime-1, ActionType.GrabKey, vision); //Send our current vision since this is for the current time

        //Our new state will no longer be holding the keycard
        var newState = state.Copy();
        newState.holdingKeyCard = false;
        Add_Basic_Action(newTime, ActionType.DropKey, null, newState);

        //Move Time Forward
        ChangeTimeEvent.Raise(timeChange);

        return timeChange;
    }

    /// <summary>
    /// Used to update state in different ways for subclasses
    /// </summary>
    /// <param name="currentAction"></param>
    protected override void CustomUpdateState(UnitStateSnapShot currentAction, bool fromUndo, IUnitState previousState)
    {
        //Do our actions
        base.CustomUpdateState(currentAction, fromUndo, previousState);

        var statusChanged = false;
        if (state.holdingKeyCard != previousState.holdingKeyCard)
            statusChanged = true;

        //Send the event if our status changed
        if (statusChanged == true)
            SendItemEvent();
    }

    /// <summary>
    /// Send event for grabbing or dropping and item so we can update the UI
    /// </summary>
    private void SendItemEvent()
    {
        //Brodcast event for UI purposes
        if (state.holdingKeyCard == false)
            ItemDroppedEvent.Raise(1);
        else
            ItemGrabbedEvent.Raise(1);
    }

    /// <summary>
    /// Add basic wait action to our player, this is used when performing actions like defusing bombs
    /// </summary>
    /// <param name="time"></param>
    private void Add_Basic_Action(int time, ActionType actionType = ActionType.Face, UnitAction givenVision=null, IUnitState newState=null)
    {
        if (newState == null)
        {
            newState = state.Copy();
        }

        var newStoreAction = new UnitStateSnapShot()
        {
            action = actionType,
            direction = direction,
            LocX = LocX,
            LocY = LocY,
            time = time,
            type = type,
            vision = givenVision, //this is set on the post update
            state = newState,
            StepGroupId = CurrentStepGroup.Value
        };
        storeAction(newStoreAction);
    }

    private void Add_Move_Action(int time, Direction newDir)
    {
        var newLocX = LocX;
        var newLocY = LocY;

        var newLoc = GetNewLocation(newDir, newLocY, newLocX);
        newLocX = newLoc.Item1;
        newLocY = newLoc.Item2;

        var newStoreAction = new UnitStateSnapShot()
        {
            action = ActionType.Face,
            direction = newDir,
            LocX = newLocX,
            LocY = newLocY,
            time = time,
            type = type,
            vision = null, //this is set on the post update
            state = state.Copy(),
            StepGroupId = CurrentStepGroup.Value
        };
        storeAction(newStoreAction);
    }

    private int DoTravelToStartInit()
    {
        isTravelingBack = 1; //We want to set our flag so we will continue this
        //Trigger our time traveling
        DoTimeTraveling(false);

        //Reset our buffer
        currentBuffer = travelingBackFrameBuffer;
        bufferCounter = currentBuffer;

        return 1;
    }

    private void DoTravelToStartUpdate()
    {
        if (CurrentTime.Value == 0)
        {
            //PlayerInputActive.Value++;
            isTravelingBack = 0;
            DoReturnFromTimeTravel();
        }
        else
        {
            //Go backward in time
            var timeChange = -1;
            ChangeTimeEvent.Raise(timeChange);
            //Reset our buffer

            //decrease buffer to speed up time travel
            if (currentBuffer > minBuffer)
                currentBuffer--;

            bufferCounter = currentBuffer;
        }
    }

    private int DoTimeTraveling(bool moveUpInTime)
    {
        //Update time traveling value
        IsTimeTraveling.Value = 1;
        var timeChange = 1;
        OnTimeTravelEvent.Raise(timeChange);

        var newTime = CurrentTime.Value + timeChange;

        //Add time traveling action
        storeAction(new UnitStateSnapShot()
        {
            action = ActionType.TravelOut,
            direction = direction,
            LocX = LocX,
            LocY = LocY,
            time = newTime,
            type = UnitType.Player,
            state = state.Copy(),
            StepGroupId = CurrentStepGroup.Value
        });

        //Create clone with all our past actions
        CreateClone();

        //Clear our actions
        actionList.Clear(); //Remove items from us

        //change our action to traveling
        action = ActionType.Traveling;

        //Move up in time
        if (moveUpInTime == true)
            ChangeTimeEvent.Raise(timeChange);

        return 1;
    }

    private int DoReturnFromTimeTravel()
    {        

        //Create Travel In action at time before, this will help with removing and adding the character
        var time = CurrentTime.Value;
        storeAction(new UnitStateSnapShot()
        {
            action = ActionType.TravelIn,
            direction = direction,
            LocX = LocX,
            LocY = LocY,
            time = time - 1,
            type = UnitType.Player,
            state = state.Copy(),
            StepGroupId = CurrentStepGroup.Value
        });
        //Create action for current time at the same we came back to
        Add_Basic_Action(time);

        IsTimeTraveling.Value = 0;
        var timeChange = 0;
        TimeChangeFromReturnTimeTravel.Value = 1; //Return from Time travel tag, since we don't want clones to do their actions again
        OnTimeTravelEvent.Raise(timeChange);
        //Call time change event even if we don't move foward to trigger checks
        ChangeTimeEvent.Raise(timeChange);

        return timeChange; //Time should not change when returning back
    }

    private void CreateClone()
    {
        var clone = Instantiate(ClonePrefab);
        var cloneUnitContainer = clone.GetComponent<UnitContainer>();
        cloneUnitContainer.actionList.Clear();

        //Let's store our actions incase players want to undo
        var newStorage = new ActionStorage()
        {
            StepGroudIdTiedTo = CurrentStepGroup.Value,
            storedSnapshots = new List<UnitStateSnapShot>()
        };

        foreach(var action in actionList)
        {
            cloneUnitContainer.actionList.Add(new UnitStateSnapShot()
            {
                action = action.action,
                direction = action.direction,
                LocX = action.LocX,
                LocY = action.LocY,
                time = action.time,
                type = UnitType.Clone,
                vision = action.vision,
                state = action.state,
                StepGroupId = CurrentStepGroup.Value //all these actions will be tied to one step
            });

            //Only want to store actions not related to this step group, since those should be removed when undoing
            if (action.StepGroupId != CurrentStepGroup.Value)
                newStorage.storedSnapshots.Add(action); //Add the action to our storage
        }
        storedActions.Add(newStorage); //Add to our actions
        cloneUnitContainer.Init();
        cloneUnitContainer.UpdateState(); // will update the state right away for clone to be on the right spot
    }

    /// <summary>
    /// Run vision after change state update
    /// </summary>
    public int OnStage3Vision(int value)
    {
        var currentActions = GetActions(CurrentTime.Value).ToList();
        if (currentActions != null)
        {
            foreach (var action in currentActions)
            {
                //Update vision for this state if necessary
                if (action.visionSet == false)
                {
                    action.vision = GetVision();
                    action.visionSet = true;
                    vision = action.vision; //Update current vision
                }
                else
                {
                    GetVision(); //We want to run get vision so we can let the future unit know we are looking at it
                }
            }

        }
        //Set dialogs, but not if we are time traveling or have no controls over player
        if (IsTimeTraveling.Value == 0)
        {
            //See what is infront of us
            var unitList = GetItemsInFront();
            var showedDialog = false;
            if (unitList.Count() > 0)
            {
                foreach(var unit in unitList)
                {
                    if (unit.type == UnitType.Bomb)
                    {
                        unit.ShowDialog("DEFUSE", "G");
                        showedDialog = true;
                        break;
                    }
                    else if (unit.type == UnitType.Box)
                    {
                        unit.ShowDialog("PUSH", "G");
                        showedDialog = true;
                        break;
                    }
                    else if (unit.type == UnitType.Keycard)
                    {
                        unit.ShowDialog("GRAB", "G");
                        showedDialog = true;
                        break;
                    }
                    else if (unit.type == UnitType.Lever)
                    {
                        unit.ShowDialog("PRESS", "G");
                        showedDialog = true;
                        break;
                    }
                    else if (unit.type == UnitType.Door1 && state.holdingKeyCard == true) //Only show this if we have a keycard
                    {
                        unit.ShowDialog("UNLOCK", "G");
                        showedDialog = true;
                        break;
                    }
                }
                
            }

            if (showedDialog == false)
            {
                //Look to show travel dialog
                if (bombAboutToBlow() == true && HideTimeTravelDialog.Value == 0)
                    ShowDialog("TRAVEL", "T");
            }
        }
        

        return value;
    }

    private bool bombAboutToBlow()
    {
        var result = false;

        var bombs = levelUnits.Items.Where(m => m.type == UnitType.Bomb).Select(m => m as BombUnit).ToList();
        foreach(var bomb in bombs)
        {
            if (bomb.aboutToBlow() == true)
            {
                result = true;
                break;
            }
        }

        return result;
    }
}
