using System;
using System.Collections.Generic;

public class LevelDecoder
{
    public List<LevelData> InitLevelList(string levelData)
    {
        var levelList = new List<LevelData>();

        var levels = levelData.Split('!');

        foreach (var level in levels)
        {
            levelList.Add(FillLevelData(level));
        }

        return levelList;
    }

    private LevelData FillLevelData(string level)
    {
        var newLevel = new LevelData();

        var levelData = level.Split('?');
        newLevel.Name = levelData[0];
        newLevel.Layout = levelData[1];
        newLevel.Actions = levelData[2];
        newLevel.StartDirection = Convert.ToInt32(levelData[3]);
        newLevel.DisplayName = levelData[4];
        newLevel.UnlockRequirement = Convert.ToInt32(levelData[5]);
        newLevel.chapter = Convert.ToInt32(levelData[6]);

        return newLevel;
    }
}
