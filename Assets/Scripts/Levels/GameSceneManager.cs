using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public IntEvent OnResetLevelEvent;
    public IntEvent OnNextLevelEvent;
    public IntEvent OnLevelSelectEvent;
    public StringVariable LevelLoaderScene;
    public StringVariable LevelSelectScene;
    public StringVariable CurrentLevel;
    public StringVariable NextLevel;
    public IntVariable CurrentLevelNum;
    public IntVariable MaxLevelNum;
    /// <summary>
    /// Need a reference in the level so we can persist the value
    /// </summary>
    public ChapterVariable currentChapter;
    public LevelLayoutCollection LevelList;
    public ChapterCollection chapterList;
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
        OnResetLevelEvent.RegisterListener(OnResetLevel);
        OnNextLevelEvent.RegisterListener(OnNextLevel);
        OnLevelSelectEvent.RegisterListener(OnLevelSelect);
    }

    private void UnregisterEvents()
    {
        OnResetLevelEvent.UnregisterListener(OnResetLevel);
        OnNextLevelEvent.UnregisterListener(OnNextLevel);
        OnLevelSelectEvent.UnregisterListener(OnLevelSelect);
    }

    /// <summary>
    /// Go to level select scene
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int OnLevelSelect(int value)
    {
        SceneManager.LoadScene(LevelSelectScene.Value, LoadSceneMode.Single);
        return value;
    }

    /// <summary>
    /// Reset level
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int OnResetLevel(int value)
    {
        //Load our level loader scene with same level
        SceneManager.LoadScene(LevelLoaderScene.Value, LoadSceneMode.Single);

        return value;
    }

    /// <summary>
    /// Reset level
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int OnNextLevel(int value)
    {
        if (CurrentLevelNum.Value >= MaxLevelNum.Value)
            return value;

        CurrentLevel.Value = NextLevel.Value;
        var levelInfo = LevelList.Items.Where(m => m.Name == CurrentLevel.Value).FirstOrDefault();
        //Update current chapter
        var chapterIndex = levelInfo.chapter - 1;
        currentChapter.SetValues(chapterList.Items[chapterIndex]);
        //Load our level loader scene with next level
        SceneManager.LoadScene(LevelLoaderScene.Value, LoadSceneMode.Single);

        return value;
    }
}
