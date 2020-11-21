﻿
using UnityEngine;

public struct Block
{
    Vector3 pos;

    bool up;
    bool down;
    bool left;
    bool right;

    float blockWidth;
    Rect blockArea;

    int owner;

    public GameObject quad;

    GameObject fog;
    bool fogOn;

    int highlightType; //0 is green, 1 is red
    

    public void initBlock(Vector3 position, bool upWall, bool downWall, bool leftWall, bool rightWall, float width, GameObject quadobj,GameObject fogobj)
    {
        pos = new Vector3(position.x, position.y, position.z);
        up = upWall;
        down = downWall;
        left = leftWall;
        right = rightWall;

        blockWidth = width;

        blockArea = new Rect(pos.x - blockWidth / 2, pos.z - blockWidth / 2, blockWidth, blockWidth);

        owner = -1;

        quad = quadobj;

        fog = fogobj;
        fogOn = true;

        highlightType = -1; //none
    }

    public Vector3 getPos()
    {
        return new Vector3(pos.x, pos.y, pos.z);
    }

    public bool getUp()
    {
        return up;
    }

    public bool getDown()
    {
        return down;
    }

    public bool getLeft()
    {
        return left;
    }

    public bool getRight()
    {
        return right;
    }

    public Rect getArea()
    {
        return new Rect(blockArea.xMin, blockArea.yMin, blockArea.width, blockArea.height);
    }

    public int getOwner()
    {
        return owner;
    }

    public void setOwner(int own)
    {
        owner = own;
    }

    public void clearFog()
    {
        fog.SetActive(false);
        fogOn = false;
    }

    public void asNeighborNotBuyableOn()
    {
        if (fogOn)
        {
            quad.GetComponent<HighlightableObject>().ConstantOn(Color.red);
            highlightType = 1;
        }
    }
    public void highLightHoverOff()
    {
        // quad.GetComponent<HighlightableObject>().ConstantOff();\
        switch (highlightType)
        {
            case -1:
                quad.GetComponent<HighlightableObject>().ConstantOff();
                break;
            case 0:
                asNeighborBuyableOn();
                break;
            case 1:
                asNeighborNotBuyableOn();
                break;
        }
    }

    public void highlightOff()
    {
        quad.GetComponent<HighlightableObject>().ConstantOff();
        highlightType = -1;
    }

    public void asNeighborBuyableOn()
    {
        quad.GetComponent<HighlightableObject>().ConstantOn(Color.green);
        highlightType = 0;
    }

    public void SelectOn()
    {
        quad.GetComponent<HighlightableObject>().ConstantOn(Color.yellow);
    }

    

}
