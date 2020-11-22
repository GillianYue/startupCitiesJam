
using UnityEngine;

/*                 [i j+1]
 *                 (j+1 i)
 *  [i-1 j]  (i j)  [i j]   (i+1 j)  [i+1 j] 
 *                  (j i)
 *                 [i j-1]
 */
using UnityEngine.UI;

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

    public GameObject quadForColor;

    GameObject fog;
    bool fogOn;


    int highlightType; //0 is green, 1 is red
    int currCost;

    //GameObject costText;
    bool buyable; //can be bought this turn

    Color[] playerColor;

    public void initBlock(Vector3 position, bool upWall, bool downWall, bool leftWall, bool rightWall, float width, 
        GameObject quadobj,GameObject colorObj,GameObject fogobj, GameObject cText)
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

        quadForColor = colorObj;

        fog = fogobj;
        fogOn = true;

        highlightType = -1; //none
        //costText = cText;

        
        playerColor = new Color[] { Color.white, Color.yellow, Color.green, Color.red, Color.blue, new Color(255/255f, 0, 255/255f)};
        buyable = false;
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
        quadForColor.GetComponent<MeshRenderer>().material.color = playerColor[owner + 1];
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
          //  costText.SetActive(true);
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
            case 2:
                highLightGetNeighbors();
                break;
        }

        setMatColorBasedOnOwner();
    }

    public void highlightOff()
    {
        quad.GetComponent<HighlightableObject>().ConstantOff();
        highlightType = -1;
       // costText.SetActive(false);
    }

    public void asNeighborBuyableOn()
    {
        quad.GetComponent<HighlightableObject>().ConstantOn(Color.green);
        highlightType = 0;
     //   costText.SetActive(true);
    }

    public void highLightGetNeighbors()
    {
        quad.GetComponent<HighlightableObject>().ConstantOn(playerColor[5]);
        highlightType = 2;
    }

    public void SelectOn()
    {
        quad.GetComponent<HighlightableObject>().ConstantOn(Color.yellow);
    }

    public void setCurrCost(int c)
    {
        currCost = c;
    }

    public int getCurrCost()
    {
        return currCost;
    }

    public bool isBuyable() { return buyable;  }

    public void setBuyable(bool b) { buyable = b; }

    public void setMatColor(Color c)
    {
        quad.GetComponent<MeshRenderer>().material.color = c;
    }

    public void setMatColorBasedOnOwner()
    {
        if (owner != -1 && owner < TurnController.playerColors.Length)
        {
            quad.GetComponent<MeshRenderer>().material.color = TurnController.playerColors[owner];
            //Debug.Log("setting col to "+owner);
        }
    }

    public int getHighlightType()
    {
        return highlightType;
    }
}
