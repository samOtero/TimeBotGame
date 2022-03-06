using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnTitleScreen : MonoBehaviour
{
    public StringVariable LevelBeatedPref;
    public StringVariable LevelLoaderSceneName;
    public StringVariable LevelName;
    public StringVariable CurrentLevelContainer;
    public StringVariable chapterSelectScreen;
    public IntVariable LevelBeatenNum;
    public ChapterVariable currentChapter;
    public ChapterVariable chapter1;

    public void OnPressed()
    {
        var levelBeaten = PlayerPrefs.GetInt(LevelBeatedPref.Value, 0);
        var sceneName = chapterSelectScreen.Value;

        //Set global level beaten variable
        LevelBeatenNum.Value = levelBeaten;

        //If we haven't beaten the first level then let's go there
        if (levelBeaten < 1)
        {
            CurrentLevelContainer.Value = LevelName.Value;
            sceneName = LevelLoaderSceneName.Value;
            currentChapter.SetValues(chapter1); //Set chapter 1 as the current chapter since we are going straight to level
        }

        //Load our level loader scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
