using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Unit Action Structure
/// </summary>
public class ActionStructure
{
    public string id;
    public int row;
    public int col;
    public Direction dir;
    public ActionType action;
    public UnitType type;
    public int time;

    public static List<ActionStructure> Decode(string data)
    {
        var actions = new List<ActionStructure>();

        var actionsRaw = data.Split('|').ToList();

        foreach(var action in actionsRaw)
        {
            actions.Add(DecodeSingle(action));
        }

        return actions;
    }

    private static ActionStructure DecodeSingle(string singleData)
    {
        var action = new ActionStructure();

        var actionRaw = singleData.Split(',').ToList();
        action.id = actionRaw[0];
        action.row = Convert.ToInt32(actionRaw[1]);
        action.col = Convert.ToInt32(actionRaw[2]);
        action.dir = (Direction)Convert.ToInt32(actionRaw[3]);
        action.action = (ActionType)Convert.ToInt32(actionRaw[4]);
        action.type = (UnitType)Convert.ToInt32(actionRaw[5]);
        action.time = Convert.ToInt32(actionRaw[6]);

        return action;
    }
}
