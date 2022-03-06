using System;

/// <summary>
/// Describes the unit's state at a given time
/// </summary>
[Serializable]
public class UnitAction 
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
    /// Time for state
    /// </summary>
    public int time;

    /// <summary>
    /// Action that got us to this state
    /// </summary>
    public ActionType action;

    /// <summary>
    /// Whic step this action belongs to, used for undoing actions
    /// </summary>
    public int StepGroupId;

    /// <summary>
    /// Unit's unique state, ex bomb's armed
    /// </summary>
    public IUnitState state;

    /// <summary>
    /// Check to see if another action is equal to this one
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsEqual(UnitAction other)
    {
        var equal = true;

        //Check if states are equal
        if (state != null && state.IsEqual(other.state) == false)
            equal = false;

        if (AreTypesEqual(type, other.type) == false ||
            LocX != other.LocX ||
            LocY != other.LocY ||
            direction  != other.direction ||
            time != other.time
            //action != other.action //Ignoring action for now as we use actions to properly trigger certain events
            )
            equal = false;

        return equal;
    }

    /// <summary>
    /// Check if types will be considered equal, ex. player and clone should be equal for our purposes
    /// </summary>
    /// <param name="originalType"></param>
    /// <param name="otherType"></param>
    /// <returns></returns>
    private bool AreTypesEqual(UnitType originalType, UnitType otherType)
    {
        var areEqual = false;

        areEqual = originalType == otherType;

        //Only do this when they are not equal
        if (areEqual == false)
        {
            //Clone should equal player
            if (UnitContainer.isTimeBotType(originalType) == true && UnitContainer.isTimeBotType(otherType) == true)
                areEqual = true;
        }

        return areEqual;
    }

}



/// <summary>
/// Describes the unit's state and vision at a given time
/// </summary>
[Serializable]
public class UnitStateSnapShot : UnitAction
{
    /// <summary>
    /// What this object is seeing at this point in time
    /// </summary>
    public UnitAction vision;
    /// <summary>
    /// Flag for vision being calculated
    /// </summary>
    public bool visionSet;
}
