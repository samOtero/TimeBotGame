using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Button1Unit : UnitContainer
{

    protected BombUnit connectedBomb;
    protected Door1Unit connectedDoor;
    protected List<Button1Unit> allButtons;
    protected bool cablesInit;

    public override void Init()
    {
        base.Init();

        //If we have a state then let's store it
        if (InitialAction != null)
        {
            storeAction(InitialAction);
        }

        connectedDoor = null;
        connectedBomb = null;
        cablesInit = false;
        //First try to get a door
        var door = levelUnits.Items.Where(m => m is Door1Unit).FirstOrDefault();
        if (door != null)
        {
            connectedDoor = door as Door1Unit;
        }
        else
        {
            //Get our bomb if no door was available
            connectedBomb = levelUnits.Items.Where(m => m is BombUnit).FirstOrDefault() as BombUnit;
        }
        
        allButtons = levelUnits.Items.Where(m => m is Button1Unit).ToList().Select(m => m as Button1Unit).ToList();
    }

    /// <summary>
    /// Resolve how having another unit on top of each other will resolve, button and box will resolve this differently
    /// </summary>
    /// <param name="unitsInSamePlace"></param>
    /// <returns></returns>
    protected override GameOutcome ResolveItemOnTop(UnitContainer unitInSamePlace)
    {
        var loseCondition = GameOutcome.None;

        //If we are passable and other units are on us that are not robots or boxes will lose the game
        if (isTimeBotType(unitInSamePlace.type) == false && unitInSamePlace.type != UnitType.Box) 
            loseCondition = GameOutcome.Lose_Paradox;

        return loseCondition;
    }

    private bool AllButtonsPressed()
    {
        var allPressed = true;

        foreach(var btn in allButtons)
        {
            if (btn.state.pressed == false)
            {
                allPressed = false;
                break;
            }
                
        }

        return allPressed;
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
                gfxAnimator.Play("pressed");
            else
                gfxAnimator.Play("initial");
        }
    }

    public void Pressed()
    {
        if (state.pressed == false)
        {
            var time = CurrentTime.Value;

            //Add an action right before for recreating in time travel
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Depressed,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = time - 1,
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
                time = time,
                type = type,
                vision = null, //No vision
                state = new IUnitState() { pressed = true },
                StepGroupId = CurrentStepGroup.Value
            });

            UpdateState(); //Calling this to update unit
            AddCloneImages(); //Update our lines as well


        }
    }

    /// <summary>
    /// Disarm bomb if all buttons are pressed
    /// </summary>
    private void DoButtonCheck(int time)
    {
        var allPressed = AllButtonsPressed();

        if (connectedBomb && allPressed)
        {
            connectedBomb.DisarmBomb(time);
        }

        if (connectedDoor)
        {
            if (allPressed == true)
                connectedDoor.Open(time);
            else if (allPressed == false)
                connectedDoor.Close(time);
        }
    }

    public void Depressed()
    {
        if (state.pressed == true)
        {
            var time = CurrentTime.Value;

            //Add an action right before for recreating in time travel
            storeAction(new UnitStateSnapShot()
            {
                action = ActionType.Pressed,
                direction = direction,
                LocX = LocX,
                LocY = LocY,
                time = time - 1,
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
                time = time,
                type = type,
                vision = null, //No vision
                state = new IUnitState() { pressed = false },
                StepGroupId = CurrentStepGroup.Value
            });

            UpdateState(); //Calling this to update unit
            AddCloneImages(); //Update our lines as well

            if (connectedBomb)
            {
                //connectedBomb.DisarmBomb(time);
            }
        }
    }

    /// <summary>
    /// Do any update after the state is updated
    /// </summary>
    protected override void CustomUpdatePostState(UnitStateSnapShot currentAction=null)
    {
        //Don't check for this during time travel
        if (IsTimeTraveling.Value > 0)
            return;

        var time = CurrentTime.Value;

        //Check to see if we are being pressed by robot, usually clones won't actually trigger this but will keep it pressed
        var unitOnTop = levelUnits.Items.Where(m => m != this && m.LocX == LocX && m.LocY == LocY && UnitIsVisible(m.action) == true).FirstOrDefault();
        if (unitOnTop == null)
        {
            if (state.pressed == true)
            {
                Depressed();
            }
        }
        else
        {
            if (state.pressed == false)
            {
                Pressed();
            }
        }

        //Check our triggers
        DoButtonCheck(time);
    }

    /// <summary>
    /// Add/update cable lines from this button to it's target
    /// </summary>
    protected override void AddCloneImages()
    {

        if (CloneImages == null)
            CloneImages = new List<GameObject>();

        if (cablesInit == false)
        {
            cablesInit = true;

            //Need to find out how many we need and what positions they will have
            var originX = LocX;
            var originY = LocY;

            UnitContainer target = connectedBomb == null ? connectedDoor as UnitContainer : connectedBomb as UnitContainer;
            var targetX = target.LocX;
            var targetY = target.LocY;

            var directionY = 0;
            var directionX = 0;

            var currentX = originX;
            var currentY = originY;
            var prevX = originX;
            var prevY = originY;
            ButtonCableContainer prevCable = null;

            if (currentX < targetX)
                directionX = 1;
            else if (currentX > targetX)
                directionX = -1;

            if (currentY < targetY)
                directionY = 1;
            else if (currentY > targetY)
                directionY = -1;

            var gotThere = false;

            while (gotThere == false)
            {
                if (currentY != targetY)
                {
                    currentY += directionY;
                }
                else if (currentX != targetX)
                {
                    currentX += directionX;
                }

                //If we got to the target then stop
                if (currentY == targetY && currentX == targetX)
                    break;

                var newImage = Instantiate(CloneImagePrefab);
                var newScript = newImage.GetComponent<ButtonCableContainer>();
                newScript.x = currentX;
                newScript.y = currentY;
                newScript.prevX = prevX;
                newScript.prevY = prevY;
                if (prevCable != null)
                {
                    prevCable.nextX = currentX;
                    prevCable.nextY = currentY;
                }
                prevCable = newScript;
                prevX = currentX;
                prevY = currentY;
                var newYLoc = currentY * BlockSize.Value;
                var newXLoc = currentX * BlockSize.Value;
                newYLoc += LevelEdgeYLoc.Value;
                newXLoc += LevelEdgeXLoc.Value;
                newImage.transform.position = new Vector3(newXLoc, newYLoc);
                CloneImages.Add(newImage);
            }
            //Set final to target
            prevCable.nextX = targetX;
            prevCable.nextY = targetY;
        }

        foreach(var image in CloneImages)
        {
            var cable = image.GetComponent<ButtonCableContainer>();
            cable.Refresh(state.pressed);
        }
    }

}
