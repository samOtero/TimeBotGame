using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BombTxt : MonoBehaviour
{
    public TextMeshProUGUI text;
    public IntEvent BombDefusedEvent;
    public IntEvent LevelLoadedEvent;
    public UnitCollection UnitList;
    // Start is called before the first frame update
    void Start()
    {
        RegisterEvents();
    }

    private void Update()
    {
        //do nothing
    }

    public int OnLevelLoaded(int value)
    {
        UpdateStatus();
        return value;
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        BombDefusedEvent.RegisterListener(OnBombDefused);
        LevelLoadedEvent.RegisterListener(OnLevelLoaded);
    }

    private void UnregisterEvents()
    {
        BombDefusedEvent.UnregisterListener(OnBombDefused);
        LevelLoadedEvent.RegisterListener(OnLevelLoaded);
    }

    public int OnBombDefused(int count)
    {
        UpdateStatus();
        return count;
    }

    private void UpdateStatus()
    {
        var bombsLeft = UnitList.Items.Where(m => m.type == UnitType.Bomb && m.state.armed == true).Count();
        var txt = string.Format("Bombs left: {0}", bombsLeft);

        text.SetText(txt);
    }
}
