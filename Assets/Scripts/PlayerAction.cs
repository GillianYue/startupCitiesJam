using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//listens for player input and has access to all players
public class PlayerAction : MonoBehaviour
{
    public Player[] players;
    public GameObject playerPrefab;

    public TurnController turnController;
    public int numPlayers = 4;

    private bool currTurnToggleStatus, //true is earn money, false is pay debt
    currTurnDone = true;

    (int,int) currHoverBlock;

    public BlockController blockController;
    public Camera mainCamera;

    private bool myReady;

    public GameObject SendMoneyPannel;

    public GameObject[] PlayerNeighbors;

    Player currPlayer;
    
    string SendMoneyInput;
    int sendPlayerId;
    Text sendPlacementHolder;

    public UIManager uiManager;
    public EventController eventController;
    public float eventTriggerRate;

    int currNumContact;

    void Start()
    {


    }

    public bool ready()
    {
        return myReady;
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
        rr[0] = Random.Range(0, numRows / 2 ); rr[2] = Random.Range(0, numRows / 2);
        rr[1] = Random.Range(numRows / 2, numRows); rr[3] = Random.Range(numRows / 2, numRows);

        for (int p = 0; p < numPlayers; p++)
        {
            Player ply = Instantiate(playerPrefab).GetComponent<Player>();
            players[p] = ply;
            players[p].playerIndex = p;
            yield return new WaitUntil(() => ply.myBlocks != null);

            System.Tuple<int, int> tup = new System.Tuple<int, int>(rr[p], rc[p]);
            players[p].myBlocks.Add(tup); //a number tuple keeping track of data only, not actual blocks
            players[p].setStartingBlock(rr[p], rc[p]);

            turnController.blockList[rr[p], rc[p]].setMatColor(TurnController.playerColors[p]);
        }


        for (int p = 0; p < numPlayers; p++)
        {
            blocks[players[p].myBlocks[0].Item1, players[p].myBlocks[0].Item2].setOwner(p);
            blocks[players[p].myBlocks[0].Item1, players[p].myBlocks[0].Item2].clearFog();
        }



        yield return new WaitUntil(() => players[numPlayers - 1]);
        myReady = true;
        print(numPlayers + " players instantiated");
    }

    void Update()
    {
        if (!currTurnDone && !uiManager.inUI)
        {
            (int,int) hov = getBlockByMousePosition(); //update curr selected block
            if (!currHoverBlock.Equals(hov))
            {
                if (currHoverBlock.Item1 != -1 && currHoverBlock.Item2 != -1) 
                    turnController.blockList[currHoverBlock.Item1, currHoverBlock.Item2].highLightHoverOff();

                currHoverBlock = hov;
                if (hov.Item1 != -1) {
                    uiManager.displayDialogueForSeconds("Cost for this block is " + turnController.blockList[hov.Item1, hov.Item2].getCurrCost()
                        +" coins", 9);
                    turnController.blockList[hov.Item1, hov.Item2].SelectOn();
                        }
            }


            if ( Input.GetMouseButtonDown(0) && currHoverBlock.Item1 != -1 &&
                turnController.blockList[currHoverBlock.Item1, currHoverBlock.Item2].isBuyable()) //buy 
            {
                if (players[turnController.getCurrTurnPlayer()].unlockNewBlock(turnController.blockList, currHoverBlock.Item1,
                    currHoverBlock.Item2, turnController.blockList[currHoverBlock.Item1, currHoverBlock.Item2].getCurrCost()))
                {
                    turnController.blockList[currHoverBlock.Item1, currHoverBlock.Item2].highlightOff();
                    turnController.blockList[currHoverBlock.Item1, currHoverBlock.Item2].clearFog();
                    turnController.blockList[currHoverBlock.Item1, currHoverBlock.Item2].setBuyable(false);
                    turnController.blockList[currHoverBlock.Item1, currHoverBlock.Item2].setMatColorBasedOnOwner();
                    uiManager.displayDialogueForSeconds("Purchased a block!", 5);

                    if(Random.Range(0f, 1.0f) <= eventTriggerRate)
                    {
                        //trigger event
                        eventController.pickRandomEvent();
                    }

                }
                refreshCost();
                
            }
        }
    }

    public IEnumerator executeTurn(int playerIndex)
    {
        currTurnToggleStatus = true;
        currTurnDone = false;

        currPlayer = players[playerIndex];

        System.Tuple<System.Tuple<bool, bool, bool, bool>, 
            List<System.Tuple<int, int>>, int>  res = blockController.getNeighbors(players[playerIndex].myBlocks, playerIndex);

        System.Tuple<bool, bool, bool, bool> bs = res.Item1;
        bool b1 = bs.Item1, b2 = bs.Item2, b3 = bs.Item3, b4 = bs.Item4;
        List<System.Tuple<int, int>> neighbors = res.Item2;

        int numTouch = res.Item3;
        currNumContact = numTouch;

        bool[] ct = currPlayer.inContact;

        if (playerIndex != 0 && ct[0] != b1) print("Player " + playerIndex + " joined with Player 0! ");
        //uiManager.displayDialogueForSeconds("Player " + playerIndex + "Joined with Player 0! ", 8);
        ct[0] = b1;

        if (playerIndex != 1 && ct[1] != b2) print("Player " + playerIndex + " joined with Player 1! ");
        //  uiManager.displayDialogueForSeconds("Player " + playerIndex + "Joined with Player 1! ", 8);
        ct[1] = b2;

        if (playerIndex != 2 && ct[2] != b3) print("Player " + playerIndex + " joined with Player 2! ");
        // uiManager.displayDialogueForSeconds("Player " + playerIndex + "Joined with Player 2! ", 8);
        ct[2] = b3;

        if (playerIndex != 3 && ct[3] != b4) print("Player " + playerIndex + " joined with Player 3! ");
        //  uiManager.displayDialogueForSeconds("Player " + playerIndex + "Joined with Player 3! ", 8);
        ct[3] = b4;

        currPlayer.inContact = ct;

        foreach (System.Tuple<int, int> n in neighbors)
        {
            
            int c = blockController.getBlockCost(n, currPlayer.returnStartingBlock(), currPlayer.myBlocks.Count);
            turnController.blockList[n.Item1, n.Item2].setCurrCost(c);

            if (c > currPlayer.getCoins())
            {
                turnController.blockList[n.Item1, n.Item2].asNeighborNotBuyableOn();
                turnController.blockList[n.Item1, n.Item2].setBuyable(false);
            }
            else
            {
                turnController.blockList[n.Item1, n.Item2].asNeighborBuyableOn();
                turnController.blockList[n.Item1, n.Item2].setBuyable(true);
            }
        }

        turnController.blockList[currPlayer.returnStartingBlock().Item1, currPlayer.returnStartingBlock().Item2].clearFog();

        yield return new WaitUntil(() => currTurnDone); //listen for player input and execute accordingly

    }

    public void sendMoneyButton()
    {
        uiManager.inUI = true;
        SendMoneyPannel.SetActive(true);
        SendMoneyPannel.GetComponent<Animator>().Play("bounceIn");

        for(int i = 0; i < 4; i++)
        {
            //Debug.Log("currPlayer.inContact["+i+"]"+ currPlayer.inContact[i]);
            if (currPlayer.inContact[i] && currPlayer.playerIndex!=i)
            {
                PlayerNeighbors[i].SetActive(true);
            }
        }

    }

    public void SendMoneyString(InputField thisInputField)
    {
        SendMoneyInput = thisInputField.text;
    }

    public void setSendId(int playerId)
    {
        sendPlayerId = playerId;
    }

    public void setPlaceHolder(Text placeHolder)
    {
        sendPlacementHolder = placeHolder;
    }


    public void sendMoneyTo()
    {
        int money = (int)float.Parse(SendMoneyInput);
        if (money > currPlayer.getCoins())
        {
            sendPlacementHolder.text = "No enough money!";
            return;
        }
        currPlayer.setCoins(currPlayer.getCoins() - money);
        players[sendPlayerId].setCoins(players[sendPlayerId].getCoins() + money);
    }

    public void closeSendMoneyButton()
    {
        uiManager.inUI = false;
        for (int i = 0; i < 4; i++)
        {
              PlayerNeighbors[i].SetActive(false);
        }

        SendMoneyPannel.SetActive(false);
    }


    public void endTurnButton()
    {
        StartCoroutine(endTurn());
    }

    private IEnumerator endTurn()
    {

        if (uiManager.payEarnToggle.value == 0)
        {
            //earn money

            Player p = players[turnController.getCurrTurnPlayer()];

            int profit = blockController.getEarn(p.myBlocks.Count,
      currNumContact);

            players[turnController.getCurrTurnPlayer()].addCoins(profit);

            print("curr contact " + currNumContact + " " + profit);
        }
        else
        {
            //pay debt
            int c = players[turnController.getCurrTurnPlayer()].getCoins();
            turnController.globalDebt -= c / 2;
            players[turnController.getCurrTurnPlayer()].setCoins(c / 2);
        }

        turnController.setAllHighlightAndBuyableOff();


        yield return new WaitForSeconds(0.2f);

        currTurnDone = true;
    }


    public (int, int) getBlockByMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit)) return (-1, -1);
        //Debug.Log(hit.collider.gameObject.transform.position);
        Vector3 pos = hit.collider.gameObject.transform.position;


        System.Tuple<int, int> tup = blockController.getBlock(pos.x, pos.z);

        int r = tup.Item1, c = tup.Item2;
        return (r, c);
    }

    public void refreshCost()
    {

        foreach (Block b in turnController.blockList)
        {
            b.highlightOff();
            b.setBuyable(false);
        }

        Player currPlayer = players[turnController.getCurrTurnPlayer()];

        System.Tuple<System.Tuple<bool, bool, bool, bool>,
            List<System.Tuple<int, int>>, int> res = blockController.getNeighbors(currPlayer.myBlocks, currPlayer.playerIndex);

        System.Tuple<bool, bool, bool, bool> bs = res.Item1;
        bool b1 = bs.Item1, b2 = bs.Item2, b3 = bs.Item3, b4 = bs.Item4;
        List<System.Tuple<int, int>> neighbors = res.Item2;

        int numTouch = res.Item3;
        currNumContact = numTouch;

        bool[] ct = currPlayer.inContact;
        int playerIndex = currPlayer.playerIndex;


        if (playerIndex != 0 && ct[0] != b1) print("Player " + playerIndex + " joined with Player 0! ");
        ct[0] = b1;

        if (playerIndex != 1 && ct[1] != b2) print("Player " + playerIndex + " joined with Player 1! ");
        ct[1] = b2;

        if (playerIndex != 2 && ct[2] != b3) print("Player " + playerIndex + " joined with Player 2! ");
        ct[2] = b3;

        if (playerIndex != 3 && ct[3] != b4) print("Player " + playerIndex + " joined with Player 3! ");
        ct[3] = b4;

        currPlayer.inContact = ct;


        foreach (System.Tuple<int, int> n in neighbors)
        {

            
            int c = blockController.getBlockCost(n, currPlayer.returnStartingBlock(), currPlayer.myBlocks.Count);
            turnController.blockList[n.Item1, n.Item2].setCurrCost(c);

            if (c > currPlayer.getCoins())
            {
                turnController.blockList[n.Item1, n.Item2].asNeighborNotBuyableOn();
                turnController.blockList[n.Item1, n.Item2].setBuyable(false);
            }
            else
            {
                turnController.blockList[n.Item1, n.Item2].asNeighborBuyableOn();
                turnController.blockList[n.Item1, n.Item2].setBuyable(true);
            }
        }
    }


}
