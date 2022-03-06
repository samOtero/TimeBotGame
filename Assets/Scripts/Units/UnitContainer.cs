using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UnitContainer : MonoBehaviour
{
    /// <summary>
    /// Unit type. Ex. Player, Clone, Wall, etc
    /// </summary>
    public UnitType type;
    /// <summary>
    /// Level Grid Row Id
    /// </summary>
    public int LocY;
    /// <summary>
    /// Level Grid Column Id
    /// </summary>
    public int LocX;
    /// <summary>
    /// Direction Unit is facing
    /// </summary>
    public Direction direction;

    /// <summary>
    /// Action to get to this state
    /// </summary>
    public ActionType action;

    public UnitAction vision;

    public IUnitState state;

    public UnitStateSnapShot InitialAction;
    public List<UnitStateSnapShot> actionList;

    public IntVariable CurrentTime;
    public IntVariable IsTimeTraveling;
    public IntVariable IsFreeFormTraveling;
    public IntVariable RowCount;
    public IntVariable ColCount;
    public IntVariable CurrentStepGroup;
    public IntVariable TimeChangeFromUndo;
    public IntVariable TimeChangeFromReturnTimeTravel;

    public FloatVariable BlockSize;
    public FloatVariable LevelEdgeYLoc;
    public FloatVariable LevelEdgeXLoc;

    /// <summary>
    /// Event will trigger when we need to update our state
    /// </summary>
    public IntEvent DoStage1LocationEvent;
    /// <summary>
    /// Event will trigger when we need to update our post state
    /// </summary>
    public IntEvent DoStage2ChangeEvent;
    /// <summary>
    /// Event triggered if the level has reached an outcome
    /// </summary>
    public IntEvent OutcomeReachedEvents;
    /// <summary>
    /// Triggered when we go in/out of the time travel realm
    /// </summary>
    public IntEvent OnTimeTravelEvent;
    /// <summary>
    /// Triggered when player goes backwards in free flow time travel
    /// </summary>
    public IntEvent OnBackwardTimeTravelEvent;
    public GameObject gfx;
    public Animator gfxAnimator;

    /// <summary>
    /// Used for dialog animations such as context sensitive UIs
    /// </summary>
    public Animator dialogAnimator;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI dialogBtnText;

    /// <summary>
    /// Can the player pass through this unit. Bombs are passable but clones are not
    /// </summary>
    public bool isPassable;

    /// <summary>
    /// List of units in the current level
    /// </summary>
    public UnitCollection levelUnits;

    /// <summary>
    /// List of blocks in the current level
    /// </summary>
    public BlockCollection levelBlocks;

    public GameObject CloneImagePrefab;
    public List<GameObject> CloneImages;
    public bool isInit;
    public List<ActionStorage> storedActions;

    private void OnEnable()
    {
        levelUnits.Add(this);
        actionList = new List<UnitStateSnapShot>();
        isInit = false;
    }

    private void OnDisable()
    {
        levelUnits.Remove(this);
        UnregisterEvents();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Only do init if we need it
        if (isInit == false)
            Init();
    }

    public virtual void Init()
    {
        isInit = true;
        RegisterEvents();
        storedActions = new List<ActionStorage>();
    }

    protected virtual void RegisterEvents()
    {
        DoStage1LocationEvent.RegisterListener(OnStage1LocationUpdate);
        DoStage2ChangeEvent.RegisterListener(OnStage2ChangeUpdate);
        OutcomeReachedEvents.RegisterListener(OnOutcomeReached);
        OnTimeTravelEvent.RegisterListener(OnTimeTravel);
        OnBackwardTimeTravelEvent.RegisterListener(OnTravelBackwards);
    }

    protected virtual void UnregisterEvents()
    {
        DoStage1LocationEvent.UnregisterListener(OnStage1LocationUpdate);
        DoStage2ChangeEvent.UnregisterListener(OnStage2ChangeUpdate);
        OutcomeReachedEvents.UnregisterListener(OnOutcomeReached);
        OnTimeTravelEvent.UnregisterListener(OnTimeTravel);
        OnBackwardTimeTravelEvent.UnregisterListener(OnTravelBackwards);
    }

    /// <summary>
    /// When level has reached the outcome
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int OnOutcomeReached(int value)
    {
        //Hide our dialog by default
        HideDialog();
        return value;
    }

    /// <summary>
    /// When player goes in/out of time travel
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int OnTimeTravel(int value)
    {
        //If we are coming back from time travel then remove and store any actions from this time and forward
        if (IsTimeTraveling.Value == 0)
        {
            StoreFutureActions();
        }
        return value;
    }

    /// <summary>
    /// When player is in Free Form Time Travel and they go backwards
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int OnTravelBackwards(int value)
    {
        StoreFutureActions();
        return value;
    }

    /// <summary>
    /// Store future actions that can be restored if player undos
    /// </summary>
    protected void StoreFutureActions()
    {
        //Let's store our actions incase players want to undo
        var newStorage = new ActionStorage()
        {
            StepGroudIdTiedTo = CurrentStepGroup.Value,
            storedSnapshots = new List<UnitStateSnapShot>()
        };

        //Only pick actions from our up our current time and not in current step group, also don't remove time 0 since it's usually a set up action
        var actionsToStore = actionList.Where(m => m.time > CurrentTime.Value && m.StepGroupId != CurrentStepGroup.Value).ToList();
        newStorage.storedSnapshots.AddRange(actionsToStore);
        storedActions.Add(newStorage);

        //Remove those actions from our action list
        actionList.RemoveAll(m => m.time > CurrentTime.Value && m.StepGroupId != CurrentStepGroup.Value);
    }

    /// <summary>
    /// On need update event
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int OnStage1LocationUpdate(int value)
    {
        UpdateState();
        return 1;
    }

    /// <summary>
    /// Do our post state update
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int OnStage2ChangeUpdate(int value)
    {
        UpdatePostState();
        return 1;
    }

    public virtual void storeAction(UnitStateSnapShot state)
    {
        actionList.Add(state);
    }

    /// <summary>
    /// Store inital action states, used in Future clones
    /// </summary>
    /// <param name="states"></param>
    public void storeInitialActions(IEnumerable<UnitStateSnapShot> states)
    {
        actionList.AddRange(states);
    }

    // Update is called once per frame
    void Update()
    {
        //Do nothing        
    }

    /// <summary>
    /// Called when we want unit to transition to the corresponding state
    /// </summary>
    public virtual void UpdateState()
    {
        var currentActions = GetActions(CurrentTime.Value).ToList();
        if (currentActions != null)
        {
            var previousState = GetPreviousState();

            foreach(var action in currentActions)
            {
                UpdateLocation(action);
                CustomUpdateState(action, false, previousState);
            }
            
        }

        HideDialog(); //Reset Dialog
    }

    protected void RemoveCloneImages()
    {
        if (CloneImages != null && CloneImages.Count > 0)
        {
            foreach (var clone in CloneImages)
            {
                Destroy(clone);
            }
            CloneImages.Clear();
        }
    }

    /// <summary>
    /// Add clone images for any future steps for this clone
    /// </summary>
    protected virtual void AddCloneImages()
    {
        
        if (CloneImages == null)
            CloneImages = new List<GameObject>();

        //First remove any previous clone images, may have to use pooling here in the future
        RemoveCloneImages();

        var cloneImageLength = 2; //This could be adjusted but 3 seems like way too much, thinking 2 is enough
        var opacity = "25";
        if (IsTimeTraveling.Value > 0)
        {
            cloneImageLength = 4;
            opacity = "75";
        }
            
        for (var i=1; i<= cloneImageLength; i++)
        {
            var newTime = CurrentTime.Value + i;
            var actions = GetActions(newTime).ToList();
            if (actions != null)
            {
                foreach(var action in actions)
                {
                    if (action.action == ActionType.Face)
                    {
                        var newImage = Instantiate(CloneImagePrefab);
                        var newScript = newImage.GetComponent<CloneImageContainer>();
                        var newAnimator = newScript.animator;
                        var newYLoc = action.LocY * BlockSize.Value;
                        var newXLoc = action.LocX * BlockSize.Value;
                        newYLoc += LevelEdgeYLoc.Value;
                        newXLoc += LevelEdgeXLoc.Value;
                        newImage.transform.position = new Vector3(newXLoc, newYLoc);
                        CloneImages.Add(newImage);

                        //Animation name
                        var direction = "South";
                        switch (action.direction)
                        {
                            case Direction.East:
                                direction = "East";
                                break;
                            case Direction.West:
                                direction = "West";
                                break;
                            case Direction.North:
                                direction = "North";
                                break;
                            case Direction.South:
                                direction = "South";
                                break;
                        }

                        var animationName = direction + "_" + opacity;
                        newAnimator.Play(animationName);
                    }
                }
            }
            //Swap the opacity
            switch (opacity)
            {
                case "75":
                    opacity = "50";
                    break;
                case "50":
                    opacity = "25";
                    break;
                case "25":
                    opacity = "13";
                    break;
            }
        }
            
    }

    /// <summary>
    /// Do any update after the state is updated
    /// </summary>
    protected virtual void UpdatePostState()
    {
        if (CloneImagePrefab != null)
        {
            AddCloneImages();
        }

        var currentActions = GetActions(CurrentTime.Value).ToList();
        if (currentActions.Count > 0)
        {
            foreach (var action in currentActions)
            {
                CustomUpdatePostState(action);
            }
        }
        else
        {
            //If we don't have any actions for this time we still run our post state, for example toggle buttons need it
            CustomUpdatePostState(null);
        }
    }

    protected virtual void CustomUpdatePostState(UnitStateSnapShot currentAction=null)
    {
        //Implement in subclass, used in player to set vision and buttons to handle pressing
    }

    /// <summary>
    /// Used to update state in different ways for subclasses
    /// </summary>
    /// <param name="currentAction"></param>
    protected virtual void CustomUpdateState(UnitStateSnapShot currentAction, bool fromUndo, IUnitState previousState)
    {
        //Implement in subclasses, like bomb and future unit
    }

    /// <summary>
    /// Called on a unit when they in another unit's vision, this is used to reveal future units and other items
    /// </summary>
    public virtual void InVision()
    {
        //Implement in subclass
    }

    /// <summary>
    /// Checks to see if we are in a losing position, by default items don't give a losing condition
    /// </summary>
    /// <returns></returns>
    public virtual GameOutcome CheckForLoseCondition()
    {
        var loseCondition = GameOutcome.None;

        //If we are not time traveling and we are visible
        if (UnitIsVisible(action) == true)
        {
            var unitsInSamePlace = levelUnits.Items.Where(m => m.LocX == LocX && m.LocY == LocY && m != this && UnitIsVisible(m.action) == true).ToList();

            //If we aren't passable and any other unit 
            if (unitsInSamePlace.Count > 0)
            {
                //If we have more than 2 units in the same spot then we lose
                if (unitsInSamePlace.Count > 1)
                {
                    loseCondition = GameOutcome.Lose_Inside_Object;
                }
                else
                {
                    loseCondition = ResolveItemOnTop(unitsInSamePlace.First());
                }
            }
        }

        return loseCondition;
    }

    /// <summary>
    /// Resolve how having another unit on top of each other will resolve, button and box will resolve this differently
    /// </summary>
    /// <param name="unitsInSamePlace"></param>
    /// <returns></returns>
    protected virtual GameOutcome ResolveItemOnTop(UnitContainer unitInSamePlace)
    {
        var loseCondition = GameOutcome.Lose_Inside_Object;        
        return loseCondition;
    }

    /// <summary>
    /// Get a unit's current vision
    /// </summary>
    /// <returns></returns>
    protected UnitAction GetVision()
    {
        //Follow the unit's direction of view to find out what the unit is seeing at this moment

        //Get unit's location and direction
        var blockX = LocX + 1;
        var blockY = LocY + 1;
        var deltaX = 0;
        var deltaY = 0;
        if (direction == Direction.North)
            deltaY = 1;
        else if (direction == Direction.South)
            deltaY = -1;
        else if (direction == Direction.East)
            deltaX = 1;
        else if (direction == Direction.West)
            deltaX = -1;

        UnitAction newVision = null;

        var keepGoing = true;
        while(keepGoing)
        {
            var block = levelBlocks.Items.Where(m => m.col == blockX && m.row == blockY).FirstOrDefault();
            if (block == null || block.walkable == false)
                keepGoing = false;
            else
            {
                var x = blockX - 1;
                var y = blockY - 1;
                var units = levelUnits.Items.Where(m => m != this && m.LocX == x && m.LocY == y).OrderByDescending(m => m.type).ToList();
                if (units != null && units.Count > 0)
                {
                    foreach(var unit in units)
                    {
                        if (UnitIsVisible(unit.action) == false)
                            continue;

                        newVision = new UnitAction()
                        {
                            action = unit.action,
                            direction = unit.direction,
                            LocX = unit.LocX,
                            LocY = unit.LocY,
                            time = CurrentTime.Value,
                            state = unit.state,
                            type = unit.type
                        };

                        unit.InVision(); //Tell the unit they are in vision of a past clone or player
                        keepGoing = false;
                    }                    
                }
                blockX += deltaX;
                blockY += deltaY;
                 
            }
        }

        if (newVision == null)
        {
            newVision = new UnitAction()
            {
                action = ActionType.None,
                direction = Direction.None,
                LocX = -1,
                LocY = -1,
                state = new IUnitState() { armed = false },
                time = CurrentTime.Value,
                type = UnitType.None
            };
        }

        return newVision;
    }

    protected void UpdateLocation(UnitStateSnapShot action)
    {
        LocX = action.LocX;
        LocY = action.LocY;
        type = action.type;
        direction = action.direction;
        this.action = action.action;
        state = action.state;
        vision = action.vision;

        var newYLoc = LocY * BlockSize.Value;
        var newXLoc = LocX * BlockSize.Value;
        newYLoc += LevelEdgeYLoc.Value;
        newXLoc += LevelEdgeXLoc.Value;
        gameObject.transform.position = new Vector3(newXLoc, newYLoc);
        var unitVisible = UnitIsVisible(action.action);

        ToggleShowing(unitVisible);
        UpdateGraphic(action); //Updates graphic based on action
    }

    protected virtual IUnitState GetPreviousState()
    {
        return state.Copy();
    }

    /// <summary>
    /// Update a unit's graphic for the current action
    /// </summary>
    /// <param name="whichAction"></param>
    protected virtual void UpdateGraphic(UnitStateSnapShot whichAction)
    {
        if (gfxAnimator != null)
        {
            gfxAnimator.SetInteger("Direction", (int)whichAction.direction);
        }

        HideDialog();
    }

    /// <summary>
    /// Get Actions for particular time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected IEnumerable<UnitStateSnapShot> GetActions(int time)
    {
        var currentActions = actionList.Where(m => m.time == time);

        return currentActions;
    }

    /// <summary>
    /// Remove actions related to a specific step group
    /// </summary>
    /// <param name="id"></param>
    public virtual void UndoStepGroup(int id, int previousTime)
    {
        var actionsRemoved = actionList.RemoveAll(m => m.StepGroupId == id);
        RestoreStoredActions(id);
        //Want to do this regardless if we removed any actions since we will be changing time and these can be non linear skips
        //If we have some actions left then lets update our state to the last action
        if (actionList.Count > 0)
        {
            var lastAction = actionList.Where(m => m.time <= previousTime).OrderByDescending(m => m.time).FirstOrDefault();
            if (lastAction != null)
            {
                var previousState = GetPreviousState();
                UpdateLocation(lastAction);
                CustomUpdateState(lastAction, true, previousState);
            }            
        }
        else
        {
            //Remove unit since it has no more actions or stored actions
            if (storedActions.Count == 0)
                RemoveUnit();
        }
    }

    public virtual void RemoveUnit()
    {
        RemoveCloneImages(); //Remove any clone images
        Destroy(gameObject);
    }

    /// <summary>
    /// Restore actions to unit, these are actions lost when time traveling, when doing undo we want to restore them
    /// </summary>
    /// <param name="stepGroupId"></param>
    protected virtual void RestoreStoredActions(int stepGroupId)
    {
        var stored = storedActions.Where(m => m.StepGroudIdTiedTo == stepGroupId).FirstOrDefault();
        if (stored == null)
            return;

        foreach (var a in stored.storedSnapshots)
        {
            actionList.Add(a);
        }

        storedActions.Remove(stored);
        stored.storedSnapshots = null;
    }

    protected void ToggleShowing(bool toggle)
    {
        gfx.SetActive(toggle);
    }

    /// <summary>
    /// Returns if a unit is visible based on their actions, this will prevent them from triggering any effects on them
    /// </summary>
    /// <param name="whichAction"></param>
    /// <returns></returns>
    public static bool UnitIsVisible(ActionType whichAction)
    {
        var isVisible = true;

        switch (whichAction)
        {
            case ActionType.TravelIn:
            case ActionType.TravelOut:
            case ActionType.Spawn:
            case ActionType.Defused:
            case ActionType.Open:
            case ActionType.Grabbed:
            case ActionType.Dropped:
            case ActionType.Traveling:
                isVisible = false;
                break;
        }

        return isVisible;
    }

    /// <summary>
    /// Checks if a given type is a TimeBot (past, present or future)
    /// </summary>
    /// <param name="whichType"></param>
    /// <returns></returns>
    public static bool isTimeBotType(UnitType whichType)
    {
        var isTimeBot = false;

        switch (whichType)
        {
            case UnitType.FutureClone:
            case UnitType.Clone:
            case UnitType.Player:
                isTimeBot = true;
                break;
        }

        return isTimeBot;
    }

    /// <summary>
    /// Get new location based on direction and loc
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="locY"></param>
    /// <param name="locX"></param>
    /// <returns></returns>
    protected Tuple<int, int> GetNewLocation(Direction direction, int locY, int locX)
    {
        var newLocY = locY;
        var newLocX = locX;

        switch (direction)
        {
            case Direction.North:
                newLocY++;
                break;
            case Direction.South:
                newLocY--;
                break;
            case Direction.East:
                newLocX++;
                break;
            case Direction.West:
                newLocX--;
                break;
        }


        return Tuple.Create(newLocX, newLocY);
    }

    /// <summary>
    /// Check if a move is legal based on current location
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected bool isMoveLegal(Direction direction, bool forItemDrop = false)
    {
        var isLegal = true;

        var newLoc = GetNewLocation(direction, LocY, LocX);

        //Check if there is a wall in the way where we want to move
        var newBlock = levelBlocks.Items.Where(m => m.col == newLoc.Item1 + 1 && m.row == newLoc.Item2 + 1).FirstOrDefault();
        if (newBlock != null)
        {
            if (newBlock.walkable == false)
                isLegal = false;
        }

        //If spot is out of map range
        if (newLoc.Item1 < 0 || newLoc.Item2 < 0)
            isLegal = false;
        else if (newLoc.Item1 >= ColCount.Value || newLoc.Item2 >= RowCount.Value)
            isLegal = false;

        //If we are still legal then let's check for any in passable units in the way
        if (isLegal == true)
        {
            var unitsInSpot = levelUnits.Items.Where(m => m.LocX == newLoc.Item1 && m.LocY == newLoc.Item2 && m != this).ToList();
            if (unitsInSpot != null && unitsInSpot.Count > 0)
            {
                foreach (var unitInSpot in unitsInSpot)
                {
                    //If this is for a player or clone then we can go on top of passable objects but items can't
                    if (unitInSpot.isPassable == true && forItemDrop == false)
                        continue;

                    //If the unit in the way is the player then we don't want to stop the action
                    if (unitInSpot.type == UnitType.Player)
                        continue;

                    //Make sure unit is not currently in transition
                    if (UnitIsVisible(unitInSpot.action) == true)
                    {
                        isLegal = false;
                        break;
                    }

                }
            }
        }

        return isLegal;
    }

    #region Dialog

    /// <summary>
    /// Show the context dialog with certain text and btn
    /// </summary>
    /// <param name="text"></param>
    /// <param name="actionBtn"></param>
    public void ShowDialog(string text, string actionBtn)
    {
        if (dialogAnimator != null)
        {
            dialogAnimator.Play("Dialog_Disable_Bomb");
            if (dialogText != null)
                dialogText.text = text;
            if (dialogBtnText != null)
                dialogBtnText.text = actionBtn;
        }
    }

    /// <summary>
    /// Hide our dialog gfx
    /// </summary>
    public void HideDialog()
    {
        if (dialogAnimator != null && dialogAnimator.isActiveAndEnabled)
        {
            dialogAnimator.Play("Dialog_Off");
        }
    }
    #endregion
}
