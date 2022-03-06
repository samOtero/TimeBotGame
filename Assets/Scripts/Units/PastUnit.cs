using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastUnit : RobotUnit
{
    /// <summary>
    /// Checks to see if we are in a losing position
    /// </summary>
    /// <returns></returns>
    public override GameOutcome CheckForLoseCondition()
    {
        var loseCondition = GameOutcome.None;

        //If we aren't in the level then don't check other units against self
        if (UnitIsVisible(action) == false)
            return GameOutcome.None;

        //Check if my actual vision is different from my saved vision
        var actualVision = GetVision();
        if (vision.IsEqual(actualVision) == false)
        {
            loseCondition = GameOutcome.Lose_Past_Paradox;
            //If past self sees future self
            if (actualVision.type == UnitType.Player)
                loseCondition = GameOutcome.Lose_Past_SeeSelf;
            dialogAnimator.Play("Dialog_On");
        }
            

        return loseCondition;
    }
}
