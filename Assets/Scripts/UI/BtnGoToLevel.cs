using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BtnGoToLevel : MonoBehaviour, IPointerClickHandler
{
    public string levelLoaderScene;
    public string levelToLoad;
    public StringVariable CurrentLevelContainer;
    public bool isLocked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLocked == true)
            return;

        //Update our current level variable
        CurrentLevelContainer.Value = levelToLoad;

        //Load our level loader scene
        SceneManager.LoadScene(levelLoaderScene, LoadSceneMode.Single);
    }
}
