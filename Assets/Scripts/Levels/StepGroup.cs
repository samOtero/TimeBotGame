using System;

[Serializable]
public class StepGroup
{
    /// <summary>
    /// Group Id for all actions done on this step
    /// </summary>
    public int GroupId;
    /// <summary>
    /// Time before this group of task was completed
    /// </summary>
    public int PreviousTime;

    /// <summary>
    /// Are we in Time Dimension before this group of task was completed
    /// </summary>
    public bool InTimeDimension;
}
