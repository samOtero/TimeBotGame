using System.Linq;
using TMPro;
using UnityEngine;

public class LevelNameTxt : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public StringVariable CurrentLevel;
    public LevelLayoutCollection LevelList;
    public ChapterVariable currentChapter;
    // Start is called before the first frame update
    void Start()
    {
        var levelInfo = LevelList.Items.Where(m => m.Name == CurrentLevel.Value).FirstOrDefault();
        Text.text = "Chapters > " + currentChapter.Name + " > " + levelInfo.DisplayName;
    }
}
