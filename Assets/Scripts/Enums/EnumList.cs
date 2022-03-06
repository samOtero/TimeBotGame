public enum DialogCharacters
{
    Professor,
    TimeBot,
    None
}

public enum GameStateStage
{
    Stage_0_Loading,
    Stage_1_Location,
    Stage_2_Changes_And_Vision,
    Stage_3_Level_Vision,
    Stage_4_Outcomes,
    Stage_5_Control_And_Auto_Travel,
    Stage_6_Level_Done
}

public enum ActionType
{
    Spawn,
    Face,
    TravelIn,
    TravelOut,
    Defused, //For bomb
    Armed, //For bomb
    None,
    Open, //For doors
    Closed, //For doors
    Pressed, //for buttons
    Depressed, //for buttons
    Grabbed, //for items
    Dropped, //for items
    GrabKey, //for bots
    DropKey, //for bots
    DoDefuseBomb, //for bots
    DoOpenDoor, //for bots
    DoPushBox, //for bots
    DoPressLever, //for bots
    Traveling,
}

public enum GameOutcome
{
    None,
    Lose_Paradox,
    Lose_BombExplode,
    Lose_BombAlreadyDefused,
    Win_BombsDefused,
    Lose_Inside_Object,
    Lose_Past_Paradox,
    Lose_Past_SeeSelf
}

public enum Direction
{
    North,
    South,
    East,
    West,
    None
}

/// <summary>
/// Actions the player can take
/// </summary>
public enum PlayerActions
{
    Wait,
    WalkNorth,
    WalkSouth,
    WalkEast,
    WalkWest,
    ToggleTimeTravel,
    TravelForward,
    TravelBackward,
    TravelToStart,
    GrabItem, //Will roll this into action
    DropItem, //Will roll this into action
    DoAction,
    None,
    NextDialog
}

public enum UnitType
{
    Player,
    Clone,
    Bomb,
    Wall,
    None,
    FutureClone,
    Door1,
    Button1,
    Keycard,
    Lever,
    Box
}
