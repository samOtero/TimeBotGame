using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BtnChapter : MonoBehaviour, IPointerClickHandler
{
    public ChapterVariable chapterInfo;
    public ChapterVariable currentChapter;
    public StringVariable LevelSelectSceneName;
    public TextMeshProUGUI Text;
    public IntVariable LevelBeated;
    public GameObject LockGfx;
    public bool isLocked;

    private void Start()
    {
        if (chapterInfo == null)
        {
            Destroy(gameObject);
            return;
        }

        var chapterDisplayName = "LOCKED";
        LockGfx.SetActive(true);
        isLocked = true;

        var neededLevel = chapterInfo.UnlockedLevel;
        if (LevelBeated.Value >= neededLevel)
        {
            isLocked = false;
            LockGfx.SetActive(false);
            chapterDisplayName = chapterInfo.Name;
        }
        Text.text = chapterDisplayName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLocked == true)
            return;

        //set chapter
        currentChapter.SetValues(chapterInfo);

        //Load our level select scene
        SceneManager.LoadScene(LevelSelectSceneName.Value, LoadSceneMode.Single);
    }
}
