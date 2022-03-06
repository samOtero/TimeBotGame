using TMPro;
using UnityEngine;

public class LoseUIManager : MonoBehaviour
{
    public IntEvent DoResetLevelEvent;
    public IntEvent DoUndoEvent;
    public IntEvent DoLevelSelectEvent;
    public GameObject UIGameObject;
    public StringVariable LoseDialog;
    public string ShowDialogAni;
    public Animator DialogTopAnimator;
    public TextMeshProUGUI DialogText;
    public GameObject DialogButtonText;
    public GameObject DialogArrowKeyButton;


    // Start is called before the first frame update
    void Start()
    {
        //Hide buttons
        DialogArrowKeyButton.SetActive(false);
        DialogButtonText.SetActive(false);
        DialogText.text = LoseDialog.Value;
        Invoke("ShowDialog", 2.0f);
    }

    private void ShowDialog()
    {
        //Play Dialog
        DialogTopAnimator.Play(ShowDialogAni);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            DoUndoEvent.Raise(1);
            Destroy(UIGameObject);
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
