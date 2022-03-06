using System;


[Serializable]
public class IUnitState
{
    /// <summary>
    /// Used for bombs
    /// </summary>
    public bool armed;

    /// <summary>
    /// Used for doors
    /// </summary>
    public bool closed;
    public bool pressed;
    public bool holdingKeyCard;
    public bool held;

    /// <summary>
    /// Check to see if another state is equal to this one
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsEqual(IUnitState other)
    {
        var equal = true;

        if (other == null || other.armed != armed || other.closed != closed || other.pressed != pressed || other.held != held)
            equal = false;

        return equal;
    }

    public IUnitState Copy()
    {
        var newState = new IUnitState()
        {
            armed = armed,
            closed = closed,
            holdingKeyCard = holdingKeyCard,
            pressed = pressed,
            held = held
        };

        return newState;
    }
}
