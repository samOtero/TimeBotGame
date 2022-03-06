using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    public IntEvent PlayerActionEvent;
    public IntEvent UndoEvent;
    public IntEvent ResetLevelEvent;
    public IntEvent DoLevelSelectEvent;
    public IntVariable PlayerInputActive;
    public IntVariable HideTimeTravel;

    // Start is called before the first frame update
    void Start()
    {
     //do nothing   
    }

    // Update is called once per frame
    void Update()
    {
        //Check to see if Player Input is Active
        if (PlayerInputActive.Value <= 0)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            PlayerActionEvent.Raise((int)PlayerActions.WalkNorth);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            PlayerActionEvent.Raise((int)PlayerActions.WalkSouth);
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            PlayerActionEvent.Raise((int)PlayerActions.WalkWest);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            PlayerActionEvent.Raise((int)PlayerActions.WalkEast);
        else if (Input.GetKeyDown(KeyCode.Z))
            UndoEvent.Raise(1);
        else if (Input.GetKeyDown(KeyCode.R))
            ResetLevelEvent.Raise(1);
        else if (Input.GetKeyDown(KeyCode.L))
            DoLevelSelectEvent.Raise(1);
        else if (Input.GetKeyDown(KeyCode.P))
            PlayerActionEvent.Raise((int)PlayerActions.Wait);
        else if (Input.GetKeyDown(KeyCode.T) && HideTimeTravel.Value == 0)
            PlayerActionEvent.Raise((int)PlayerActions.TravelToStart);
        else if (Input.GetKeyDown(KeyCode.G))
            PlayerActionEvent.Raise((int)PlayerActions.DoAction);
        else if (Input.GetKeyDown(KeyCode.Y))
          PlayerActionEvent.Raise((int)PlayerActions.ToggleTimeTravel);
        else if (Input.GetKeyDown(KeyCode.X))
          PlayerActionEvent.Raise((int)PlayerActions.TravelBackward);
        else if (Input.GetKeyDown(KeyCode.C))
            PlayerActionEvent.Raise((int)PlayerActions.TravelForward);
    }
}
