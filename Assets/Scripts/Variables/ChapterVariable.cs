using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Chapter")]
public class ChapterVariable : ScriptableObject
{
    public string Name;
    public int Index;
    public int UnlockedLevel;

    public void SetValues(ChapterVariable newChapter)
    {
        Index = newChapter.Index;
        Name = newChapter.Name;
        UnlockedLevel = newChapter.UnlockedLevel;
    }
}
