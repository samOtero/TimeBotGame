using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int row;
    public int col;
    public string animationName;
    public bool walkable;
    public bool isBottomWall;
    public bool isVisible;
    public Animator animator;
    public bool init;
    public IntVariable IsTimeTraveling;
    private string WALL_NAME = "FloorTileWall";
    private string WALL_BOTTOM_NAME = "FloorTileWallBottom";
    private string FLOOR_1_SHADOW = "FloorTile1_Shadow";
    private string FLOOR_1 = "FloorTile1";
    private string FLOOR_2_SHADOW = "FloorTile2_Shadow";
    private string FLOOR_2 = "FloorTile2";

    public void Start()
    {
        init = false;
    }

    public void Update()
    {
        if (init == false)
        {
            init = true;
            animationName = FLOOR_1;
            if (walkable == false)
            {
                if (isBottomWall == true)
                {
                    animationName = WALL_BOTTOM_NAME;
                }
                else
                {
                    animationName = WALL_NAME;
                }
            }
                
            else if (IsTimeTraveling.Value > 0)
            {
                animationName = FLOOR_2;
                if (isVisible == false)
                    animationName = FLOOR_2_SHADOW;
            }
            else
            {
                animationName = FLOOR_1;
                if (isVisible == false)
                    animationName = FLOOR_1_SHADOW;
            }
            animator.Play(animationName);
        }
    }

    public void SetVisible(bool toggle)
    {
        isVisible = toggle;
        init = false;
    }

    public void SetWalkable(bool toggle)
    {
        walkable = toggle;
        init = false;
    }
}
