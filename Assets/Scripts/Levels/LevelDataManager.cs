using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Level Data/Manager")]
public class LevelDataManager : ScriptableObject
{
    public LevelData data;
    public LevelLayoutCollection LevelList;
    public TextAsset rawLevelData;

    public void Init()
    {
        var rawData = rawLevelData.text;
        var levelDataSplit = rawData.Split('~');
        var current_version = Convert.ToInt32(levelDataSplit[0]);

        if (LevelList.currentVersion < current_version)
        {
            var levelData = levelDataSplit[1];
            var levelListNew = new LevelDecoder().InitLevelList(levelData);
            LevelList.Items = levelListNew;
            var levelNameList = "";
            foreach (var level in levelListNew)
            {
                levelNameList += level.Name;
                levelNameList += ",";
            }
            levelNameList = levelNameList.Remove(levelNameList.Length - 1, 1);
            LevelList.List = levelNameList;
            LevelList.currentVersion = current_version;
        }
    }
}
