using UnityEngine;

public class WinUIManager : MonoBehaviour
{
    public IntEvent DoResetLevelEvent;
    public IntEvent DoNextLevelEvent;
    public IntEvent DoLevelSelectEvent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            DoNextLevelEvent.Raise(1);
            return;
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            DoResetLevelEvent.Raise(1);
            return;
        }
        else if (Input.GetKeyUp(KeyCode.L))
        {
            DoLevelSelectEvent.Raise(1);
            return;
        }
    }
}
