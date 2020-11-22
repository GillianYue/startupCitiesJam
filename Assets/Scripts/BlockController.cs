using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public MazeGeneration maze;

    public int blockOriginalCost;

    public int blockAdditionalCost;

    public float lossRate;

    public int blockOriginalEarn;

    /*
    block的cost = blockOriginalCost + blockAdditionalCost * 玩家拥有的区域数量*此区块到玩家初始区块的距离
    玩家生产值 = 区域数量惩罚 * blockOriginalEarn * 玩家拥有的区域数量
    区域数量惩罚 = 1 - lossRate * 玩家拥有的区域数量
    */
    public Tuple<int,int> getBlock(float mouseX,float mouseY) 
    {
        if (!maze.allowedArea.Contains(new Vector2(mouseX, mouseY))) return new Tuple<int, int>(-1, -1);

        return new Tuple<int, int>((int) ((mouseX-maze.allowedArea.xMin)/maze.blockSize),
            (int) ((mouseY-maze.allowedArea.yMin)/maze.blockSize));
    }

    public Tuple<Tuple<bool, bool, bool, bool>,
            List<Tuple<int, int>>,
            int
            > getNeighbors(List<Tuple<int, int>> myBlocks, int playerID)
    {
        Block[,] blockList = maze.blockList;
        List<Tuple<int, int>> outPut = new List<Tuple<int, int>>();
        Tuple<bool, bool, bool, bool> touchedNeighbors;
        bool player0 = false;
        bool player1 = false;
        bool player2 = false;
        bool player3 = false;
        Tuple<Tuple<bool, bool, bool, bool>, List<Tuple<int, int>>, int> outPutTuple;
        int playerNeighbors = 0;

        for (int i = 0; i < myBlocks.Count; i++)
        {
            int row = myBlocks[i].Item1;
            int col = myBlocks[i].Item2;
            if (!blockList[row, col].getUp())
            {
                if (blockList[row, col + 1].getOwner() == -1)
                {
                    outPut.Add(new Tuple<int, int>(row, col + 1));
                }
                else if (blockList[row, col + 1].getOwner() != playerID)
                {
                    //  Debug.Log("neighb"+blockList[row, col + 1].getOwner());
                    blockList[row, col].highLightGetNeighbors();
                    playerNeighbors++;
                    switch (blockList[row, col + 1].getOwner())
                    {
                        case 0:
                            player0 = true;
                            break;
                        case 1:
                            player1 = true;
                            break;
                        case 2:
                            player2 = true;
                            break;
                        case 3:
                            player3 = true;
                            break;
                    }
                }
            }
            if (!blockList[row, col].getDown())
            {
                if (blockList[row, col - 1].getOwner() == -1)
                {
                    outPut.Add(new Tuple<int, int>(row, col - 1));
                }
                else if (blockList[row, col - 1].getOwner() != playerID)
                {
                    //Debug.Log("neighb" + blockList[row, col - 1].getOwner());
                    blockList[row, col].highLightGetNeighbors();
                    playerNeighbors++;
                    switch (blockList[row, col - 1].getOwner())
                    {
                        case 0:
                            player0 = true;
                            break;
                        case 1:
                            player1 = true;
                            break;
                        case 2:
                            player2 = true;
                            break;
                        case 3:
                            player3 = true;
                            break;
                    }
                }
            }
            if (!blockList[row, col].getLeft())
            {
                if (blockList[row - 1, col].getOwner() == -1)
                {
                    outPut.Add(new Tuple<int, int>(row - 1, col));
                }
                else if (blockList[row - 1, col].getOwner() != playerID)
                {
                    //Debug.Log("neighb" + blockList[row-1, col].getOwner());
                    blockList[row, col].highLightGetNeighbors();
                    playerNeighbors++;
                    switch (blockList[row - 1, col].getOwner())
                    {
                        case 0:
                            player0 = true;
                            break;
                        case 1:
                            player1 = true;
                            break;
                        case 2:
                            player2 = true;
                            break;
                        case 3:
                            player3 = true;
                            break;
                    }
                }
            }
            if (!blockList[row, col].getRight())
            {
                if (blockList[row + 1, col].getOwner() == -1)
                {
                    outPut.Add(new Tuple<int, int>(row + 1, col));
                }
                else if (blockList[row + 1, col].getOwner() != playerID)
                {

                    //Debug.Log("neighb" + blockList[row+1, col].getOwner());
                    blockList[row, col].highLightGetNeighbors();
                    playerNeighbors++;
                    switch (blockList[row + 1, col].getOwner())
                    {
                        case 0:
                            player0 = true;
                            break;
                        case 1:
                            player1 = true;
                            break;
                        case 2:
                            player2 = true;
                            break;
                        case 3:
                            player3 = true;
                            break;
                    }
                }
            }
        }
        touchedNeighbors = new Tuple<bool, bool, bool, bool>(player0, player1, player2, player3);
        outPutTuple = new Tuple<Tuple<bool, bool, bool, bool>,
                          List<Tuple<int, int>>,
                          int>(touchedNeighbors, outPut, playerNeighbors);
        return outPutTuple;
    }

    public int getBlockCost(Tuple<int,int>costBlock,Tuple<int,int>originalBlock,int areaNum)
    {
        int x = costBlock.Item1 - originalBlock.Item1;
        int y = costBlock.Item2 - originalBlock.Item2;
        int dist = (int)Mathf.Sqrt(x * x + y * y);
        return blockOriginalCost + blockAdditionalCost * areaNum * dist;
    }

    public int getEarn(int areaNum, int neighbornum)
    {
        return (int) ((1 - (double) lossRate * (areaNum-neighbornum)) * blockOriginalEarn * areaNum);
    }

}
