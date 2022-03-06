using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnGoToScene : MonoBehaviour
{
    public string levelLoaderScene;
    public StringVariable LevelToLoad;
    public StringVariable CurrentLevelContainer;

    public void OnPressed()
    {
        //Update our current level variable
        if (LevelToLoad && string.IsNullOrWhiteSpace(LevelToLoad.Value) == false)
         CurrentLevelContainer.Value = LevelToLoad.Value;

        //Load our level loader scene
        SceneManager.LoadScene(levelLoaderScene, LoadSceneMode.Single);
    }
}