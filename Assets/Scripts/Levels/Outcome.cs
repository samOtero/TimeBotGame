using System.Linq;
using UnityEngine;

public class Outcome : MonoBehaviour
{
    public UnitCollection unitList;

    public GameObject UIWin;
    public GameObject UILose;
    public GameObject BeatAllLevelsMsg;
    public GameObject UnlockAllLevelsMsg;
    public GameObject BeatChapter1Msg;

    public StringVariable ShowedBeatAllLevelsMsgPref;
    public StringVariable ShowedUnlockAllLevelsMsgPref;
    public StringVariable ShowedBeatChapter1MsgPref;
    public IntVariable Msg1Level;
    public IntVariable Msg2Level;
    public IntVariable Msg3Level;
    public IntVariable IsTimeTraveling;
    public OutcomeVariable currentOutcome;

    public IntEvent DoStage4OutcomeEvent;
    public IntEvent OutcomeReachedEvent;
    public IntEvent DoGetOutcomeEvent;
    public IntEvent DoShowWinDialogEvent;
    public StringVariable LevelBeatenPref;
    public IntVariable LevelBeatenNum;
    public IntVariable CurrentLevelNum;
    public StringVariable LoseDialog;

    // Start is called before the first frame update
    void Start()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        DoStage4OutcomeEvent.RegisterListener(OnCheckOutcome);
        DoShowWinDialogEvent.RegisterListener(OnShowWinDialog);
        DoGetOutcomeEvent.RegisterListener(SetOutcome);
    }

    private void UnregisterEvents()
    {
        DoStage4OutcomeEvent.UnregisterListener(OnCheckOutcome);
        DoShowWinDialogEvent.UnregisterListener(OnShowWinDialog);
        DoGetOutcomeEvent.UnregisterListener(SetOutcome);
    }

    public int OnShowWinDialog(int value)
    {
        Instantiate(UIWin);
        return value;
    }

    public int SetOutcome(int value)
    {
        currentOutcome.Value = GetOutcome();
        return value;
    }

    public GameOutcome GetOutcome()
    {
        var outcome = GameOutcome.None;
        foreach (var unit in unitList.Items)
        {
            outcome = unit.CheckForLoseCondition();
            if (outcome != GameOutcome.None)
                break;
        }

        if (outcome == GameOutcome.None && CheckBombsDefused() == true)
            outcome = GameOutcome.Win_BombsDefused;

        return outcome;
    }

    public int OnCheckOutcome(int value)
    {
        //Don't end level if we are time traveling
        if (IsTimeTraveling.Value == 1)
            return 1;

        var outcome = GetOutcome();

        //Set losing dialog
        switch(outcome)
        {
            case GameOutcome.Lose_BombExplode:
                LoseDialog.Value = "The bomb exploded.. no big deal.. this is why we train in a simulation..";
                break;
            case GameOutcome.Lose_Inside_Object:
                LoseDialog.Value = "Looks like you traveled back into another object! That can't be good..";
                break;
            case GameOutcome.Lose_Paradox:
                LoseDialog.Value = "You've somehow created a paradox! Impressive but.. no bueno..";
                break;
            case GameOutcome.Lose_Past_Paradox:
                LoseDialog.Value = "A PARADOX! Your past selves must experience the world exactly as you did, don't forget that!";
                break;
            case GameOutcome.Lose_Past_SeeSelf:
                LoseDialog.Value = "You can't let your past-self see your future-self! That's Time Travel 101!";
                break;
        }

        switch (outcome)
        {
            case GameOutcome.Win_BombsDefused:
                OutcomeReachedEvent.Raise(1);
                SetLevelBeat();
                ShowWinMessage();
                break;
            case GameOutcome.Lose_BombExplode:
            case GameOutcome.Lose_Paradox:
            case GameOutcome.Lose_Inside_Object:
            case GameOutcome.Lose_Past_Paradox:
            case GameOutcome.Lose_Past_SeeSelf:
                OutcomeReachedEvent.Raise(1);
                Instantiate(UILose);//show our lose screen
                break;

        }
        return 1;
    }

    // Update is called once per frame
    void Update()
    {
       //do nothing
    }

    /// <summary>
    /// Check if all bombs are defused
    /// </summary>
    /// <returns></returns>
    private bool CheckBombsDefused()
    {
        var bombsLeft = unitList.Items.Where(m => m.type == UnitType.Bomb && m.state.armed == true).Count();
        var allDefused = bombsLeft == 0 ? true : false;

        return allDefused;
    }

    /// <summary>
    /// Saves player pref for winning a level
    /// </summary>
    private void SetLevelBeat()
    {
        var previousLevelBeated = PlayerPrefs.GetInt(LevelBeatenPref.Value, 0);
        var currentLevelNum = CurrentLevelNum.Value;
        if (currentLevelNum > previousLevelBeated)
        {
            PlayerPrefs.SetInt(LevelBeatenPref.Value, currentLevelNum);
            LevelBeatenNum.Value = currentLevelNum;
            PlayerPrefs.Save();
        }
    }

    private void ShowWinMessage()
    {
        if (ShowSpecialMessage() == false)
        {
            OnShowWinDialog(1);
        }
    }

    private bool ShowSpecialMessage()
    {
        var currentLevelNum = CurrentLevelNum.Value;
        //var showedBeatedChapter1Msg = PlayerPrefs.GetInt(ShowedBeatChapter1MsgPref.Value, 0);
        //var showedUnlockedAllMsg = PlayerPrefs.GetInt(ShowedUnlockAllLevelsMsgPref.Value, 0);
        var showedBeatAllMsg = PlayerPrefs.GetInt(ShowedBeatAllLevelsMsgPref.Value, 0);

        /*
        if (showedBeatedChapter1Msg == 0)
        {
            //Only trigger after beating first chapter
            if (currentLevelNum == Msg1Level.Value)
            {
                PlayerPrefs.SetInt(ShowedBeatChapter1MsgPref.Value, 1);
                PlayerPrefs.Save();
                //Instantiate(BeatChapter1Msg); //dont' want to show this message
                return true;
            }
        }

        if (showedUnlockedAllMsg == 0)
        {
            //Only trigger after beating second chapter
            if (currentLevelNum == Msg2Level.Value)
            {
                PlayerPrefs.SetInt(ShowedUnlockAllLevelsMsgPref.Value, 1);
                PlayerPrefs.Save();
                Instantiate(UnlockAllLevelsMsg);
                return true;
            }
        }
        */

        if (showedBeatAllMsg == 0)
        {
            //Only trigger after beating final level
            if (currentLevelNum == Msg3Level.Value)
            {
                PlayerPrefs.SetInt(ShowedBeatAllLevelsMsgPref.Value, 1);
                PlayerPrefs.Save();
                Instantiate(BeatAllLevelsMsg);
                return true;
            }
        }

        return false;
    }
}
