using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCableContainer : MonoBehaviour
{
    public int x;
    public int y;
    public int prevX;
    public int prevY;
    public int nextX;
    public int nextY;
    public bool isInit;
    public bool isActivated;
    public Animator animator;
    public Transform gfxTransform;

    public void Refresh(bool isActivated)
    {
        if (isInit == true && this.isActivated == isActivated)
            return;

        isInit = true;
        this.isActivated = isActivated;
        var ZDegrees = 0.0f;
        var animationName = "";
        var sufix = "Horizontal";
        if (prevX == x && nextX == x)
        {
            ZDegrees = 90.0f;
            sufix = "Horizontal";
        }
        else if (prevY == y && nextY == y)
        {
            ZDegrees = 0.0f;
            sufix = "Horizontal";
        }
        else
        {
            ZDegrees = 0.0f;
            sufix = "Bend";
            var entrance = "";
            var exit = "";

            if (prevY < y)
                entrance = "down";
            else if (prevY > y)
                entrance = "up";
            else if (prevX < x)
                entrance = "left";
            else if (prevX > x)
                entrance = "right";

            if (nextY < y)
                exit = "down";
            else if (nextY > y)
                exit = "up";
            else if (nextX < x)
                exit = "left";
            else if (nextX > x)
                exit = "right";

            if (entrance == "up" && exit == "right")
                ZDegrees = 270.0f;
            else if (entrance == "right" && exit == "up")
                ZDegrees = 270.0f;
            else if (entrance == "down" && exit == "right")
                ZDegrees = 180.0f;
            else if (entrance == "right" && exit == "down")
                ZDegrees = 180.0f;
            else if (entrance == "down" && exit == "left")
                ZDegrees = 90.0f;
            else if (entrance == "left" && exit == "down")
                ZDegrees = 90.0f;

        }
        var prefix = "CableOff_";
        if (isActivated == true)
            prefix = "CableOn_";

        animationName = prefix + sufix;

        animator.Play(animationName);
        gfxTransform.rotation = Quaternion.Euler(0, 0, ZDegrees);
    }
}
