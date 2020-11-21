using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MazeGeneration : MonoBehaviour
{
    public GameObject wall;

    public GameObject nullParent;

    public GameObject quadPrefab;

    public GameObject fogPrefab;

    public float blockSize;

    public float fogHeight;

    //public float clearPara;

    [Range(0,0.99f)]
    public float breakPara;


    [SerializeField]
    public Rect allowedArea = new Rect(-10f,-10f,20f,20f);

    Tuple<bool,bool> [ , ] rowWalls;
    Tuple<bool, bool>[ , ] colWalls;

    public Block[,] blockList;
    

    float wallSize;
    int colSize;
    int rowSize;
    //int blockNum;

    void Start()
    {
        initParameter();
        GenerateBlocks();
        PrimeAlgorism();
        GenerateMaze();
    }

    private void GenerateMaze()
    {
        int item1 = 0;
        int item2 = 0;
        for(float i = allowedArea.xMin; i <= allowedArea.xMax; i+=wallSize)
        {
            
            for(float j = allowedArea.yMin + wallSize/2; j < allowedArea.yMax; j+=wallSize)
            {
                
                if (!colWalls[item1, item2].Item1)
                {
                    item2++;
                    continue;
                }
                GameObject wallCol = Instantiate(wall);
                wallCol.transform.position = new Vector3(i, 0.5f, j);
                wallCol.transform.localScale = new Vector3(0.1f, 1+Mathf.PerlinNoise(i, j), 1);
                wallCol.transform.parent = nullParent.transform;
                item2++;
            }
            item1++;
            item2 = 0;
        }

        item1 = 0;
        item2 = 0;
        for (float i = allowedArea.yMin; i <= allowedArea.yMax; i++)
        {
            
            for (float j = allowedArea.xMin + wallSize/2; j < allowedArea.xMax; j++)
            {
                
                if (!rowWalls[item1, item2].Item1)
                {
                    item2++;
                    continue;
                } 
                GameObject wallRow = Instantiate(wall);
                wallRow.transform.position = new Vector3(j, 0.5f, i);
                wallRow.transform.localScale = new Vector3(0.1f, 1+Mathf.PerlinNoise(j, i), 1);
                wallRow.transform.rotation = Quaternion.Euler(0, 90, 0);
                wallRow.transform.parent = nullParent.transform;
                item2++;
            }
            item1++;
            item2 = 0;
        }

        //生成区块信息
        for(int i = 0; i < rowSize; i++)
        {
            for(int j = 0; j < colSize; j++)
            {
                Vector3 position = new Vector3(wallSize/2+allowedArea.xMin+wallSize*i,0.5f,allowedArea.yMin+wallSize/2+wallSize*j);
                GameObject quad = Instantiate(quadPrefab);
                quad.transform.parent = nullParent.transform;
                quad.transform.position = position;

                GameObject fog = Instantiate(fogPrefab);
                fog.transform.parent = nullParent.transform;
                fog.transform.position = new Vector3(wallSize / 2 + allowedArea.xMin + wallSize * i, fogHeight, allowedArea.yMin + wallSize / 2 + wallSize * j);

                blockList[i, j].initBlock(position,
                    rowWalls[i + 1, j].Item1, rowWalls[i, j].Item1, colWalls[j, i].Item1, colWalls[j + 1, i].Item1,
                    wallSize,
                    quad,fog);
                
            }
        }
    }

    private void initParameter()
    {
        //blockNum = ((int)allowedArea.width * (int)allowedArea.height) / (int)blockSize;
        wallSize = Mathf.Sqrt(blockSize);
        rowSize = ((int)allowedArea.width / (int)wallSize);
        colSize = ((int)allowedArea.height / (int)wallSize);


        rowWalls = new Tuple<bool, bool>[colSize + 1, rowSize];//此块板子在[第几行，第几列]
        colWalls = new Tuple<bool, bool>[rowSize + 1, colSize];//此块板子在[第几列，第几行]

        blockList = new Block[rowSize,colSize];//此区块在[第几行，第几列]
    }

     private void PrimeAlgorism()
    {
        //生成一个存元组的队列，元组模版（行/列，行编号，列编号）
        Queue<Tuple<int, int, int>> randomQueue = new Queue<Tuple<int, int, int>>();
        randomQueue.Enqueue(new Tuple<int, int, int>
            (0,
            UnityEngine.Random.Range(1, colSize-1),
            UnityEngine.Random.Range(1, rowSize-1)));
        //int time = 0;
        while (randomQueue.Count != 0)
        {
            //time++;
           // Debug.Log(randomQueue.Count);
            //if (time >= 800) return;

            Tuple<int, int, int> node;
            if (UnityEngine.Random.Range(0, 1f) > breakPara)
            {
                node = randomQueue.Dequeue();
            }
            else
            {
                randomQueue.Enqueue(randomQueue.Dequeue());
                continue;
            }
            
            if (node.Item1 == 0)
            {
                if (node.Item2 == 0 || node.Item2 >= colSize || !rowWalls[node.Item2,node.Item3].Item1)
                {
                    
                    continue;
                    
                }
                int upWallCount = 0;
                int downWallCount = 0;
                if (rowWalls[node.Item2 - 1, node.Item3].Item1)
                {
                    downWallCount++;
                    if(!rowWalls[node.Item2 - 1, node.Item3].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            rowWalls[node.Item2 - 1, node.Item3] = new Tuple<bool, bool>(true, true);
                        //else
                            //rowWalls[node.Item2 - 1, node.Item3] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (0, node.Item2 - 1, node.Item3));
                    }
                    
                }
                if (rowWalls[node.Item2 + 1, node.Item3].Item1)
                {
                    upWallCount++;
                    if(!rowWalls[node.Item2 + 1, node.Item3].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            rowWalls[node.Item2 + 1, node.Item3] = new Tuple<bool, bool>(true, true);
                        //else
                            //rowWalls[node.Item2 + 1, node.Item3] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (0, node.Item2 + 1, node.Item3));
                    }
                    
                }
                if (colWalls[node.Item3, node.Item2].Item1 )
                {
                    upWallCount++;
                    if(!colWalls[node.Item3, node.Item2].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            colWalls[node.Item3, node.Item2] = new Tuple<bool, bool>(true, true);
                        //else
                            //colWalls[node.Item3, node.Item2] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (1, node.Item3, node.Item2));
                    }
                    
                }
                if (colWalls[node.Item3+1, node.Item2].Item1)
                {
                    upWallCount++;
                    if(!colWalls[node.Item3 + 1, node.Item2].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            colWalls[node.Item3 + 1, node.Item2] = new Tuple<bool, bool>(true, true);
                        //else
                            //colWalls[node.Item3 + 1, node.Item2] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (1, node.Item3 + 1, node.Item2));
                    }
                    
                }
                if (colWalls[node.Item3, node.Item2-1].Item1 )
                {
                    downWallCount++;
                    if(!colWalls[node.Item3, node.Item2 - 1].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            colWalls[node.Item3, node.Item2 - 1] = new Tuple<bool, bool>(true, true);
                        //else
                            //colWalls[node.Item3, node.Item2 - 1] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (1, node.Item3, node.Item2 - 1));
                    }
                    
                }
                if (colWalls[node.Item3+1 , node.Item2-1].Item1 )
                {
                    downWallCount++;
                    if(!colWalls[node.Item3 + 1, node.Item2 - 1].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            colWalls[node.Item3 + 1, node.Item2 - 1] = new Tuple<bool, bool>(true, true);
                        //else
                            //colWalls[node.Item3 + 1, node.Item2 - 1] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (1, node.Item3 + 1, node.Item2 - 1));
                    }
                }
                if (upWallCount == 3 || downWallCount==3)
                {
                    rowWalls[node.Item2, node.Item3] = new Tuple<bool, bool>(false, true);
                }
                //else
                //{
                    //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                        //rowWalls[node.Item2, node.Item3] = new Tuple<bool, bool>(true, true);
                    //else
                        //rowWalls[node.Item2, node.Item3] = new Tuple<bool, bool>(false, true);
                //}
            }
            else
            {
                if (node.Item2 == 0 || node.Item2 >= rowSize || !colWalls[node.Item2, node.Item3].Item1)
                {
                    
                    continue;
                } 
                int leftWallCount = 0;
                int rightWallCount = 0;
                if (colWalls[node.Item2 - 1, node.Item3].Item1 )
                {
                    leftWallCount++;
                    if(!colWalls[node.Item2 - 1, node.Item3].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            colWalls[node.Item2 - 1, node.Item3] = new Tuple<bool, bool>(true, true);
                        //else
                            //colWalls[node.Item2 - 1, node.Item3] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (1, node.Item2 - 1, node.Item3));
                    }
                    
                }
                if (colWalls[node.Item2 + 1, node.Item3].Item1 )
                {
                    rightWallCount++;
                    if(!colWalls[node.Item2 + 1, node.Item3].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            colWalls[node.Item2 + 1, node.Item3] = new Tuple<bool, bool>(true, true);
                        //else
                            //colWalls[node.Item2 + 1, node.Item3] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (1, node.Item2 + 1, node.Item3));
                    }
                    
                }
                if (rowWalls[node.Item3,node.Item2-1].Item1 )
                {
                    leftWallCount++;
                    if(!rowWalls[node.Item3, node.Item2 - 1].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            rowWalls[node.Item3, node.Item2 - 1] = new Tuple<bool, bool>(true, true);
                        //else
                            //rowWalls[node.Item3, node.Item2 - 1] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (0, node.Item3, node.Item2 - 1));
                    }
                    
                }
                if (rowWalls[node.Item3+1, node.Item2 - 1].Item1 )
                {
                    leftWallCount++;
                    if(!rowWalls[node.Item3 + 1, node.Item2 - 1].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            rowWalls[node.Item3 + 1, node.Item2 - 1] = new Tuple<bool, bool>(true, true);
                        //else
                            //rowWalls[node.Item3 + 1, node.Item2 - 1] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (0, node.Item3 + 1, node.Item2 - 1));
                    }
                    
                }
                if (rowWalls[node.Item3+1, node.Item2].Item1 )
                {
                    rightWallCount++;
                    if(!rowWalls[node.Item3 + 1, node.Item2].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            rowWalls[node.Item3 + 1, node.Item2] = new Tuple<bool, bool>(true, true);
                       // else
                            //rowWalls[node.Item3 + 1, node.Item2] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (0, node.Item3 + 1, node.Item2));
                    }
                    
                }
                if (rowWalls[node.Item3, node.Item2].Item1 )
                {
                    rightWallCount++;
                    if(!rowWalls[node.Item3, node.Item2].Item2)
                    {
                        //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                            rowWalls[node.Item3, node.Item2] = new Tuple<bool, bool>(true, true);
                        //else
                            //rowWalls[node.Item3, node.Item2] = new Tuple<bool, bool>(false, true);
                        randomQueue.Enqueue(new Tuple<int, int, int>
                            (0, node.Item3, node.Item2));
                    }
                    
                }
                if (leftWallCount == 3 || rightWallCount==3)
                {
                    colWalls[node.Item2, node.Item3] = new Tuple<bool, bool>(false, true);
                }
                //else
                //{
                    //if (UnityEngine.Random.Range(0, 1.0f) > clearPara)
                        //colWalls[node.Item2, node.Item3] = new Tuple<bool, bool>(true, true);
                    //else
                        //colWalls[node.Item2, node.Item3] = new Tuple<bool, bool>(false, true);
                //}
            }
        }
    }

    private void GenerateBlocks()
    {
        
        for(int i = 0; i <= colSize; i++)//行迭代
        {
            for(int j = 0; j <= rowSize; j++)//列迭代
            {
                if (i == colSize)
                {
                    if (j != rowSize)
                    {
                        rowWalls[i, j] = new Tuple<bool, bool> (true,false);
                    }
                    
                }
                else if (j==rowSize)
                {
                    colWalls[j, i] = new Tuple<bool, bool>(true, false);
                }
                else
                {
                    rowWalls[i, j] = new Tuple<bool, bool>(true, false);
                    colWalls[j, i] = new Tuple<bool, bool>(true, false);
                }
            }
        }
    }


    public Block[,] getBlockList()
    {
        return blockList;
    }

    public float getRowSize()
    {
        return rowSize;
    }

    public float getColSize()
    {
        return colSize;
    }
}
