using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public bool isActive;
    public string ShowDialogAni;
    public string HideDialogAni;
    public string NextButtonText;
    public string ProfessorAni;
    public string TimebotAni;
    public string NoneAni;
    public Animator DialogTopAnimator;
    public Animator DialogCharacterAnimator;
    public TextMeshProUGUI DialogText;
    public GameObject DialogButtonText;
    public GameObject DialogArrowKeyButton;
    public List<DialogItemGroup> groups;
    public List<DialogItemGroup> finishedGroups;

    public IntEvent DoStage4Outcomes;
    public IntEvent PlayerActionEvent;

    public StringVariable CurrentLevel;
    public IntVariable PlayerInputActive;
    public UnitVariable PlayerUnit;
    public IntVariable CurrentTime;

    public DialogItemGroup currentGroup;
    public DialogItem currentItem;
    public IntVariable HideTimeTravelDialog;

    protected double exitTimer;

    private void RegisterEvents()
    {
        DoStage4Outcomes.RegisterListener(OnDoDialog);
    }

    private void UnregisterEvents()
    {
        DoStage4Outcomes.UnregisterListener(OnDoDialog);
    }

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        finishedGroups = new List<DialogItemGroup>();
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive == false)
            return;

        var doNext = false;

        if (currentItem.waitPlayerMoveAction == true)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
            {
                doNext = true;
                PlayerActionEvent.Raise((int)PlayerActions.WalkNorth);
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            {
                PlayerActionEvent.Raise((int)PlayerActions.WalkSouth);
                doNext = true;
            }

            else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                PlayerActionEvent.Raise((int)PlayerActions.WalkWest);
                doNext = true;
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                PlayerActionEvent.Raise((int)PlayerActions.WalkEast);
                doNext = true;
            }

        }
        else if (currentItem.waitPlayerTravelAction == true)
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                PlayerActionEvent.Raise((int)PlayerActions.TravelToStart);
                HideTimeTravelDialog.Value = 0; //Let the player use the move after this event
                doNext = true;
            }
        }
        else if (currentItem.exitAfterTime == true)
        {
            exitTimer -= Time.deltaTime;
            if (exitTimer < 0)
            {
                exitTimer = 0;
                doNext = true;
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.N))
                doNext = true;
        }

        if (doNext == true)
            DoNext();

    }

    private void DoNext()
    {
        currentItem = currentGroup.GetNextItem();
        if (currentItem == null)
        {
            isActive = false;
            DialogTopAnimator.Play(HideDialogAni);
            PlayerInputActive.Value = 1;
        }
        else
        {
            SetItem();
        }
    }

    public int OnDoDialog(int value)
    {
        if (isActive == true)
        {
            PlayerInputActive.Value = 0;
        }
        else
        {
            currentGroup = GetGroupForLevel();
            if (currentGroup != null)
            {
                isActive = true;
                currentGroup.Reset();
                DoNext();
                DialogTopAnimator.Play(ShowDialogAni);
                PlayerInputActive.Value = 0;
                finishedGroups.Add(currentGroup); //Add to our finished list so we don't pull it out again
            }
        }
        
        
        return value;
    }

    private void SetItem()
    {
        if (currentItem == null)
            return;
        var characterAnimation = ProfessorAni;
        if (currentItem.character == DialogCharacters.TimeBot)
            characterAnimation = TimebotAni;
        else if (currentItem.character == DialogCharacters.None)
            characterAnimation = NoneAni;

        DialogCharacterAnimator.Play(characterAnimation);
        DialogText.text = currentItem.text;
        if (currentItem.waitPlayerMoveAction || currentItem.waitPlayerTravelAction || currentItem.exitAfterTime)
            DialogButtonText.SetActive(false);
        else
            DialogButtonText.SetActive(true);

        if (currentItem.waitPlayerMoveAction)
            DialogArrowKeyButton.SetActive(true);
        else
            DialogArrowKeyButton.SetActive(false);

        if (currentItem.exitAfterTime)
            exitTimer = currentItem.exitTime;
    }

    private DialogItemGroup GetGroupForLevel()
    {
        var currentLevelName = CurrentLevel.Value;
        var matchedGroups = groups.Where(m => finishedGroups.Contains(m) == false && m.levelName == currentLevelName).ToList();
        DialogItemGroup group = null;
        foreach(var g in matchedGroups)
        {
            if (g.TriggerOnSpecificSpot == true)
            {
                if (PlayerUnit.value.LocX == g.col && PlayerUnit.value.LocY == g.row)
                {
                    group = g;
                    break;
                }
            }
            else if (g.TriggerOnSpecificTime == true)
            {
                if (CurrentTime.Value == g.time)
                {
                    group = g;
                    break;
                }
            }
            else
            {
                group = g;
                break;
            }
        }

        return group;
    }
}
