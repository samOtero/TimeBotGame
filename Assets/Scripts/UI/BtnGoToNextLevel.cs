using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnGoToNextLevel : MonoBehaviour
{
    public string levelLoaderScene;
    public StringVariable LevelToLoad;
    public StringVariable CurrentLevelContainer;
    public IntVariable CurrentLevelNum;
    public IntVariable MaxLevelNum;
    public GameObject BtnGfx;

    private void Start()
    {
        if (CurrentLevelNum.Value >= MaxLevelNum.Value)
            BtnGfx.SetActive(false);
    }

    public void OnPressed()
    {
        if (CurrentLevelNum.Value >= MaxLevelNum.Value)
            return;

        //Update our current level variable
        if (LevelToLoad && string.IsNullOrWhiteSpace(LevelToLoad.Value) == false)
            CurrentLevelContainer.Value = LevelToLoad.Value;

        //Load our level loader scene
        SceneManager.LoadScene(levelLoaderScene, LoadSceneMode.Single);
    }
}