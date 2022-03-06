using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelsSelectManager : MonoBehaviour
{
    public LevelLayoutCollection LevelList;
    public LevelDataManager levelDataManager;
    public List<GameObject> LevelIcons;
    public StringVariable LevelBeatenPref;
    public TextMeshProUGUI TitleTxt;
    public ChapterVariable currentChapter;
    /// <summary>
    /// Have this here so reference is not lost
    /// </summary>
    public IntVariable LevelBeatenNum;

    // Start is called before the first frame update
    void Start()
    {
        TitleTxt.text = "Chapters > "+currentChapter.Name;
        resetIcons();
    }

    private void resetIcons()
    {
        //Ensure level data is set
        levelDataManager.Init();

        hideIcons();
        var maxLevelBeaten = PlayerPrefs.GetInt(LevelBeatenPref.Value, 0);

        var chapterLevels = LevelList.Items.Where(m => m.chapter == currentChapter.Index).ToList();

        var i = 0;
        foreach(var level in chapterLevels)
        {
            if (i >= LevelIcons.Count)
                break;

            var icon = LevelIcons[i];
            var text = icon.GetComponentInChildren<TextMeshProUGUI>();
            var lockGfx = icon.GetComponent<LevelIcon>();
            var lvlScript = icon.GetComponent<BtnGoToLevel>();
            icon.SetActive(true);
            var levelDisplayName = "LOCKED";
            lvlScript.levelToLoad = level.Name;
            lvlScript.isLocked = true;

            //Check for unlocking
            if (maxLevelBeaten >= level.UnlockRequirement)
            {
                lockGfx.LockGfx.SetActive(false);
                lvlScript.isLocked = false;
                levelDisplayName = level.DisplayName;

            }

            text.text = levelDisplayName;

            i++;
        }
    }

    private void hideIcons()
    {
        foreach(var icon in LevelIcons)
        {
            icon.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
