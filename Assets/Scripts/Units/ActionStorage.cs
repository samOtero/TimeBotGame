using System;
using System.Collections.Generic;

[Serializable]
public class ActionStorage
{
    public int StepGroudIdTiedTo;
    public List<UnitStateSnapShot> storedSnapshots;
}
