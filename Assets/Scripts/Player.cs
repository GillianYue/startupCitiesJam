using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Vector3 myCenter; //position for camera to locate this player's control view
    public List<System.Tuple<int, int>> myBlocks; //spaces that I currently own; format is (num of col (x; item1), num of row(z; item2))
    private int coins = 50;
    public int playerIndex;
    System.Tuple<int,int> startingBlock;

    public bool[] inContact; //whether my blocks are in contact with other players' blocks

    void Start()
    {
        myBlocks = new List<System.Tuple<int, int>>();
        inContact = new bool[4];
    }

    void Update()
    {
        
    }


    public void sendMoney(Player toPlayer)
    {

    }


    public void endTurn()
    {

    }


   // public void

    public bool unlockNewBlock(Block[,] blocks, int row, int col, int cost)
    {
        if(blocks[row, col].getOwner() == -1 && coins >= cost) {
            print(playerIndex + " unlocked new block at "+row+" "+col);
            coins -= cost;
            blocks[row, col].setOwner(playerIndex);
            System.Tuple<int, int> tup = new System.Tuple<int, int>(row, col);
            myBlocks.Add(tup);

            return true;
        }
        else
        {
            return false;
        }
    }

    public void checkForInContact()
    {

    }

    public void setStartingBlock(int row, int col)
    {
        System.Tuple<int, int> tup = new System.Tuple<int, int>(row, col);
        startingBlock = tup;
    }

    public System.Tuple<int, int> returnStartingBlock()
    {
        return startingBlock;
    }

    public void setCoins(int c)
    {
        coins = c;
    }

    public int getCoins()
    {
        return coins;
    }

    public void addCoins(int c)
    {
        coins += c;
    }
}
