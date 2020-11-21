using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//listens for player input and has access to all players
public class PlayerAction : MonoBehaviour
{
    public Player[] players;
    public GameObject playerPrefab;

    public TurnController turnController;
    public int numPlayers = 1;

    private bool currTurnToggleStatus, //true is earn money, false is pay debt
    currTurnDone = true;

    (int,int) currHoverBlock;

    public BlockController blockController;
    public Camera mainCamera;

    void Start()
    {


    }

    public IEnumerator initializeAssign(Block[,] blocks)
    {

        players = new Player[numPlayers];

        int numCols = (int)turnController.mazeGen.getRowSize();
        int numRows = (int)turnController.mazeGen.getColSize();

        //assign random starting block for each player
        int[] rc = new int[4], rr = new int[4];
        rc[0] = Random.Range(0, numCols / 2 ); rc[2] = Random.Range(numCols / 2, numCols);
        rc[1] = Random.Range(0, numCols / 2 ); rc[3] = Random.Range(numCols / 2, numCols);
        rr[0] = Random.Range(0, numRows / 2 ); rr[2] = Random.Range(numRows / 2, numRows);
        rr[1] = Random.Range(0, numRows / 2 ); rr[3] = Random.Range(numRows / 2, numRows);

        for (int p = 0; p < numPlayers; p++)
        {
            Player ply = Instantiate(playerPrefab).GetComponent<Player>();
            players[p] = ply;
            players[p].playerIndex = p;
            yield return new WaitUntil(() => ply.myBlocks != null);
            //            print(rr[p] + " " + rc[p] + " " + p + " " + numCols + " " + numRows + " " +players[p].myBlocks.Count);
            System.Tuple<int, int> tup = new System.Tuple<int, int>(rr[p], rc[p]);
            players[p].myBlocks.Add(tup); //a number tuple keeping track of data only, not actual blocks
            players[p].setStartingBlock(rr[p], rc[p]);
        }


        for (int p = 0; p < numPlayers; p++)
        {
            blocks[players[p].myBlocks[0].Item1, players[p].myBlocks[0].Item2].setOwner(p);
        }

       // currTurnDone = false;
    }

    void Update()
    {
        if (!currTurnDone)
        {
            (int,int) hov = getBlockByMousePosition(); //update curr selected block
            if (!currHoverBlock.Equals(hov))
            {
                if (currHoverBlock.Item1 != -1) turnController.blockList[currHoverBlock.Item1, currHoverBlock.Item2].highLightHoverOff();
                currHoverBlock = hov;
                if (hov.Item1 != -1) {

                    turnController.blockList[hov.Item1, hov.Item2].SelectOn();
                        }
            }
        }
    }

    public IEnumerator executeTurn(int playerIndex)
    {
        currTurnToggleStatus = true;
        currTurnDone = false;

        Player currPlayer = players[playerIndex];

        System.Tuple<System.Tuple<bool, bool, bool, bool>, 
            List<System.Tuple<int, int>>>  res = blockController.getNeighbors(players[playerIndex].myBlocks);

        System.Tuple<bool, bool, bool, bool> bs = res.Item1;
        bool b1 = bs.Item1, b2 = bs.Item2, b3 = bs.Item3, b4 = bs.Item4;
        List<System.Tuple<int, int>> neighbors = res.Item2;

        currPlayer.inContact[0] = b1; currPlayer.inContact[1] = b2;
        currPlayer.inContact[2] = b3; currPlayer.inContact[3] = b4;

        foreach(System.Tuple<int, int> n in neighbors)
        {

            
            int c = blockController.getBlockCost(n, currPlayer.returnStartingBlock(), currPlayer.myBlocks.Count);
            print("neighbor " + n.Item1 + ", " + n.Item2 + ": " + c);

            if (c > currPlayer.coins)
            {
                turnController.blockList[n.Item1, n.Item2].asNeighborNotBuyableOn();
            }
            else
            {
                turnController.blockList[n.Item1, n.Item2].asNeighborBuyableOn();
            }
        }

        turnController.blockList[currPlayer.returnStartingBlock().Item1, currPlayer.returnStartingBlock().Item2].clearFog();

        yield return new WaitUntil(() => currTurnDone); //listen for player input and execute accordingly

    }

    public void sendMoneyButton()
    {

    }

    public void togglePayAndEarnButton()
    {
        currTurnToggleStatus = !currTurnToggleStatus;
        //update visual 

    }

    public void endTurnButton()
    {

        foreach(Block b in turnController.blockList)
        {
            b.highlightOff();
        }
        currTurnDone = true;
    }

    //button event
    public void clickOnBuyableBlockButton()
    {
        int r = 0, c = 0, cost; //TODO
        if (players[turnController.getCurrTurnPlayer()].unlockNewBlock(turnController.blockList, r, c, 0))//TODO cost function
        {
            //make visual changes
        }

    }


    public (int,int) getBlockByMousePosition()
    {
        Vector3 temp = Input.mousePosition;
        temp.z = mainCamera.transform.position.y;
        Vector3 pos = mainCamera.ScreenToWorldPoint(temp);

        System.Tuple<int, int> tup = blockController.getBlock(pos.x, pos.z);
        //print("curr hover block: " + tup + " " + pos);
        int r = tup.Item1, c = tup.Item2;
        return (r, c);
    }

}
