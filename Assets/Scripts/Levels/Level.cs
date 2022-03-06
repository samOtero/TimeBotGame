using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    public LevelDataManager levelDataManager;
    public StringVariable CurrentLevelContainer;
    public StringVariable NextLevelContainer;
    public IntVariable CurrentLevelNum;
    public IntVariable HideTimeTravelDialog;
    public IntVariable TimeTravelUnlockedLevelNum;
    public LevelLayoutCollection LevelList;
    public IntVariable RowCount;
    public IntVariable ColCount;
    public IntVariable IsTimeTraveling;
    public UnitVariable PlayerUnit;
    public IntEvent DoLoadingEvent;
    public IntEvent DoLevelVisionEvent;
    public IntEvent LoadingCompleteEvent;
    public IntEvent OnTimeTravelEvent;
    public UnitCollection UnitList;

    /// <summary>
    /// Level blocks
    /// </summary>
    public BlockCollection blocks;
    public List<WallConfig> walls;

    public GameObject TilePrefab;
    public GameObject PlayerPrefab;
    public GameObject BombPrefab;
    public GameObject FuturePrefab;
    public GameObject Door1Prefab;
    public GameObject Button1Prefab;
    public GameObject KeycardPrefab;
    public GameObject LeverPrefab;
    public GameObject BoxPrefab;

    public CinemachineTargetGroup CameraTargetGroup;

    public FloatVariable BlockSize;
    public FloatVariable LevelEdgeYLoc;
    public FloatVariable LevelEdgeXLoc;

    public StringVariable LevelAttemptedPref;
    public StringVariable LevelBeatedPref;

    // Start is called before the first frame update
    void Start()
    {
        RegisterEvents();
        //Set our global variables
        SetNextCurrentLevel();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        OnTimeTravelEvent.RegisterListener(OnTimeTravel);
        DoLoadingEvent.RegisterListener(OnLoading);
        DoLevelVisionEvent.RegisterListener(OnLevelVisionUpdate);
    }

    private void UnregisterEvents()
    {
        OnTimeTravelEvent.UnregisterListener(OnTimeTravel);
        DoLoadingEvent.UnregisterListener(OnLoading);
        DoLevelVisionEvent.UnregisterListener(OnLevelVisionUpdate);
    }

    public int OnLoading(int value)
    {
        initMap();
        //Send out this event once we are done loading, used for bomb UI (which is disabled atm)
        LoadingCompleteEvent.Raise(1);
        return value;
    }

    public int OnLevelVisionUpdate(int value)
    {
        doTileReset();
        return value;
    }

    private void SetNextCurrentLevel()
    {
        var levels = LevelList.List.Split(',').ToList();
        var index = levels.IndexOf(CurrentLevelContainer.Value) + 1;
        CurrentLevelNum.Value = index;
        SetTimeTravelDialogUnlocked();
        SetLevelAttempted();
        var nextLevel = levels[0]; //next level will reset to first level
        if (levels.Count > index)
            nextLevel = levels[index];

        NextLevelContainer.Value = nextLevel;
    }

    /// <summary>
    /// Hide or show time travel dialog for certain levels
    /// </summary>
    private void SetTimeTravelDialogUnlocked()
    {
        var currentLevelNum = CurrentLevelNum.Value;
        var timeTravelUnlockedLevel = TimeTravelUnlockedLevelNum.Value;
        HideTimeTravelDialog.Value = 0;
        if (currentLevelNum  < timeTravelUnlockedLevel)
            HideTimeTravelDialog.Value = 1;
    }

    /// <summary>
    /// Set player pref for level attempted
    /// </summary>
    private void SetLevelAttempted()
    {
        var currentLevelNum = CurrentLevelNum.Value;
        var previousLevelAttempted = PlayerPrefs.GetInt(LevelAttemptedPref.Value, 0);
        if (previousLevelAttempted < currentLevelNum)
        {
            PlayerPrefs.SetInt(LevelAttemptedPref.Value, currentLevelNum);
            PlayerPrefs.Save();
        }
    }

    private void initMap()
    {
        //Ensure level data is updated
        levelDataManager.Init();

        blocks.Reset();
        var levelInfo = LevelList.Items.Where(m => m.Name == CurrentLevelContainer.Value).FirstOrDefault();
        var Layout = levelInfo.Layout;
        var mapRows = Layout.Split('|');
        mapRows = mapRows.Reverse().ToArray(); //Reverse to preserve visuals
        var rows = mapRows.Count();
        var cols = 0;
        var r = 1;
        var c = 1;
        var startTime = 0; //for now we always start at 0, this might change!
        foreach (var row in mapRows)
        {
            var mapCols = row.Split(',');
            cols = mapCols.Count();
            foreach (var col in mapCols)
            {
                var newBlock = Instantiate(TilePrefab);

                //Add to our dynamic camera target group
                if (CameraTargetGroup != null)
                    CameraTargetGroup.AddMember(newBlock.transform, 1, 1);

                var newYLoc = (r - 1) * BlockSize.Value;
                var newXLoc = (c - 1) * BlockSize.Value;
                newYLoc += LevelEdgeYLoc.Value;
                newXLoc += LevelEdgeXLoc.Value;
                newBlock.transform.position = new Vector3(newXLoc, newYLoc);

                var newBlockScript = newBlock.GetComponent<Block>();
                newBlockScript.col = c;
                newBlockScript.row = r;
                var walkable = true;
                var isVisible = false;
                var isBottomWall = false;
                if (col.Trim() == "w")
                {
                    walkable = false;

                    //Set is bottom value
                    if (r > 1)
                    {
                        var previousRow = mapRows[r - 2].Split(',');
                        var previousCol = previousRow[c - 1];
                        if (previousCol.Trim() != "w")
                            isBottomWall = true;
                    }
                    //Looks worse with this set, in my opinion
                    /*else
                    {
                       // isBottomWall = true;
                    }*/
                    
                }
                newBlockScript.walkable = walkable;
                newBlockScript.isVisible = isVisible;
                newBlockScript.isBottomWall = isBottomWall;
                blocks.Add(newBlockScript);

                //Handle adding bombs
                var nameSplit = col.Trim().Split('_');
                if (nameSplit[0] == "b")
                    AddBomb(r - 1, c - 1, startTime, Convert.ToInt32(nameSplit[1]));
                else if (col.Trim() == "p")
                    AddPlayer(r - 1, c - 1, startTime, levelInfo.StartDirection);
                else if (col.Trim() == "d1")
                    AddDoor1(r - 1, c - 1, startTime);
                else if (col.Trim() == "btn1")
                    AddButton1(r - 1, c - 1, startTime);
                else if (col.Trim() == "k")
                    AddKeycard(r - 1, c - 1, startTime);
                else if (col.Trim() == "l")
                    AddLever(r - 1, c - 1, startTime);
                else if (col.Trim() == "bx")
                    AddBox(r - 1, c - 1, startTime);

                c++;
            }
            c = 1;
            r++;
        }

        //Set our global values
        RowCount.Value = rows;
        ColCount.Value = cols;

        //Add our future clone characters
        AddFutureClones(levelInfo.Actions);

    }

    private void AddDoor1(int row, int col, int time)
    {
        var newDoor1 = Instantiate(Door1Prefab);
        var newDoor1Script = newDoor1.GetComponent<Door1Unit>();
        newDoor1Script.InitialAction.LocX = col;
        newDoor1Script.InitialAction.LocY = row;
        newDoor1Script.InitialAction.time = time;
    }

    private void AddButton1(int row, int col, int time)
    {
        var newUnit = Instantiate(Button1Prefab);
        var newScript = newUnit.GetComponent<Button1Unit>();
        newScript.InitialAction.LocX = col;
        newScript.InitialAction.LocY = row;
        newScript.InitialAction.time = time;
    }

    private void AddLever(int row, int col, int time)
    {
        var newUnit = Instantiate(LeverPrefab);
        var newScript = newUnit.GetComponent<LeverUnit>();
        newScript.InitialAction.LocX = col;
        newScript.InitialAction.LocY = row;
        newScript.InitialAction.time = time;
    }

    private void AddBox(int row, int col, int time)
    {
        var newUnit = Instantiate(BoxPrefab);
        var newScript = newUnit.GetComponent<BoxUnit>();
        newScript.InitialAction.LocX = col;
        newScript.InitialAction.LocY = row;
        newScript.InitialAction.time = time;
    }

    private void AddKeycard(int row, int col, int time)
    {
        var newUnit = Instantiate(KeycardPrefab);
        var newScript = newUnit.GetComponent<KeyUnit>();
        newScript.InitialAction.LocX = col;
        newScript.InitialAction.LocY = row;
        newScript.InitialAction.time = time;
    }

    private void AddBomb(int row, int col, int time, int bombTimer)
    {
        var newBomb = Instantiate(BombPrefab);
        var newBombScript = newBomb.GetComponent<BombUnit>();
        newBombScript.InitialAction.LocX = col;
        newBombScript.InitialAction.LocY = row;
        newBombScript.InitialAction.time = time;
        newBombScript.MaxTime = bombTimer;
    }

    private void AddPlayer(int row, int col, int time, int direction)
    {
        var newPlayer = Instantiate(PlayerPrefab);
        var newScript = newPlayer.GetComponent<PlayerUnitContainer>();
        //newScript.InitialAction = new UnitStateSnapShot()
        //{
        //    action = ActionType.Face,
        //    direction = Direction.South,
        //    LocX = col,
        //    LocY = row,
        //    time = time,
        //    type = UnitType.Player,
        //    vision = null //will fill out on the storing
        //};
        newScript.InitialAction.direction = (Direction)direction;
        newScript.InitialAction.LocX = col;
        newScript.InitialAction.LocY = row;
        newScript.InitialAction.time = time;
        PlayerUnit.value = newScript;
    }

    private void AddFutureClones(string StoredActions)
    {
        if (string.IsNullOrWhiteSpace(StoredActions))
            return;

        var actionData = StoredActions;
        if (string.IsNullOrWhiteSpace(actionData))
            return;
        var actionList = ActionStructure.Decode(actionData);
        var initialAction = actionList.Where(m => m.time == 0).FirstOrDefault();

        var steps = new List<UnitStateSnapShot>();
        var row = initialAction.row; //5
        var col = initialAction.col; //1
        var time = 0;

        foreach(var action in actionList)
        {
            if (action.time == 0)
                continue;

            steps.Add(new UnitStateSnapShot()
            {
                action = action.action,
                direction = action.dir,
                LocX = action.col, //2
                LocY = action.row, //5
                time = action.time,
                type = action.type,
                StepGroupId = -1 //Should not be able to delete this
            });
        }        

        AddFutureClone(row, col, time, steps);
    }

    private void AddFutureClone(int row, int col, int time, IEnumerable<UnitStateSnapShot> steps)
    {
        var futurePlayer = Instantiate(FuturePrefab);
        var newScript = futurePlayer.GetComponent<FutureUnitContainer>();
        newScript.InitialAction.LocX = col;
        newScript.InitialAction.LocY = row;
        newScript.InitialAction.time = time;

        newScript.storeInitialActions(steps);
    }

    public int OnTimeTravel(int direction)
    {
        swapTileAnimation();
        return direction;
    }

    private void swapTileAnimation()
    {
        var newAnimation = "FloorTile1";
        if (IsTimeTraveling.Value > 0)
            newAnimation = "FloorTile2";

        foreach(var block in blocks.Items)
        {
            if (block.walkable == false)
                continue;

            block.animationName = newAnimation;
            block.init = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //do nothing
    }

    private void doTileReset()
    {
        //Make all blocks hidden
        foreach (var block in blocks.Items)
            block.SetVisible(false);

        foreach (var unit in UnitList.Items)
        {
            //This will only apply to our past clones and players
            if (unit.type != UnitType.Player && unit.type != UnitType.Clone)
                continue;

            //Don't reveal if unit is not in the dimension or visible, with the exception of the cloaking device
            if (UnitContainer.UnitIsVisible(unit.action) == false)
                continue;

            //Get unit's location and direction
            var dir = unit.direction;
            var x = unit.LocX +1;
            var y = unit.LocY +1;
            var deltaX = 0;
            var deltaY = 0;
            if (dir == Direction.North)
                deltaY = 1;
            else if (dir == Direction.South)
                deltaY = -1;
            else if (dir == Direction.East)
                deltaX = 1;
            else if (dir == Direction.West)
                deltaX = -1;

            RevealTiles(x, y, deltaX, deltaY, unit);
        }        
    }
    private void RevealTiles(int x, int y, int deltaX, int deltaY, UnitContainer whichUnit)
    {
        var keepGoing = true;
         while(keepGoing)
        {
            var block = blocks.Items.Where(m => m.col == x && m.row == y).FirstOrDefault();
            if (block == null || block.walkable == false)
                keepGoing = false;
            else
            {
                var x2 = x - 1;
                var y2 = y - 1;
                var units = UnitList.Items.Where(m => m != whichUnit && m.LocX == x2 && m.LocY == y2).ToList();
                if (units != null && units.Count > 0)
                {
                    foreach (var unit in units)
                    {
                        //If unit is not visible then it shouldn't stop our view
                        if (UnitContainer.UnitIsVisible(unit.action) == false)
                            continue;

                        if (unit.LocX == whichUnit.LocX && unit.LocY == whichUnit.LocY)
                            continue;

                        keepGoing = false;
                        break;
                    }
                }

                //Make block visible
                block.SetVisible(true);
                x += deltaX;
                y += deltaY;
            }
        }
    }

}
