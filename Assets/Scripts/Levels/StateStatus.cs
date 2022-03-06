using UnityEngine;

public class StateStatus : MonoBehaviour
{
    public IntEvent DoStage0Loading;
    public IntEvent DoStage1Location;
    public IntEvent DoStage2ChangesAndVisions;
    public IntEvent DoStage3LevelVision;
    public IntEvent DoStage4Outcome;
    public IntEvent DoStage5ControlAndAutoTravel;

    public IntEvent TimeChangedEvent;
    public IntEvent DoTimeTravelEvent;
    public IntEvent OutcomeReachedEvent;
    public UnitCollection UnitList;
    public GameStateStage currentStage;
    public IntVariable PlayerInputActive;
    public IntVariable TimeChangeFromUndo;
    public IntVariable TimeChangeFromReturnTimeTravel;

    private void RegisterEvents()
    {
        TimeChangedEvent.RegisterListener(OnTimeChanged);
        DoTimeTravelEvent.RegisterListener(OnTimeTravel);
        OutcomeReachedEvent.RegisterListener(OnOutcomeReached);
    }

    private void UnregisterEvents()
    {
        TimeChangedEvent.UnregisterListener(OnTimeChanged);
        DoTimeTravelEvent.UnregisterListener(OnTimeTravel);
        OutcomeReachedEvent.RegisterListener(OnOutcomeReached);
    }

    // Start is called before the first frame update
    void Start()
    {
        RegisterEvents();
        //Init our state for loading
        currentStage = GameStateStage.Stage_0_Loading;
        PlayerInputActive.Value = 0; //Remove player input when we first start
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentStage)
        {
            case GameStateStage.Stage_0_Loading:
                DoStage0Loading.Raise(1);
                currentStage = GameStateStage.Stage_1_Location;
                break;
            case GameStateStage.Stage_1_Location:
                DoStage1Location.Raise(1);
                currentStage = GameStateStage.Stage_2_Changes_And_Vision;
                break;
            case GameStateStage.Stage_2_Changes_And_Vision:
                DoStage2ChangesAndVisions.Raise(1);
                currentStage = GameStateStage.Stage_3_Level_Vision;
                break;
            case GameStateStage.Stage_3_Level_Vision:
                DoStage3LevelVision.Raise(1);
                currentStage = GameStateStage.Stage_4_Outcomes;
                break;
            case GameStateStage.Stage_4_Outcomes:
                currentStage = GameStateStage.Stage_5_Control_And_Auto_Travel;
                PlayerInputActive.Value = 1; //Give player back some input
                TimeChangeFromUndo.Value = 0; //Reset our Undo tag
                TimeChangeFromReturnTimeTravel.Value = 0; //Reset our Time Travel tag
                DoStage4Outcome.Raise(1);                
                break;
            case GameStateStage.Stage_5_Control_And_Auto_Travel:                
                DoStage5ControlAndAutoTravel.Raise(1);  
                break;
            case GameStateStage.Stage_6_Level_Done:
                //do nothing just wait for next screen to load
                break;
        }
    }

    public int OnTimeChanged(int value)
    {
        RefreshGameState();
        return value;
    }

    public int OnTimeTravel(int value)
    {
        RefreshGameState();
        return value;
    }

    public int OnOutcomeReached(int value)
    {
        PlayerInputActive.Value = 0; //Remove player inputs after outcome is reached
        currentStage = GameStateStage.Stage_6_Level_Done;
        return value;
    }

    private void RefreshGameState()
    {
        currentStage = GameStateStage.Stage_1_Location; //Game will do a game state update
        //Stop player input since we are updating the state
        PlayerInputActive.Value = 0;
    }
}
