using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{

    public int turn = 0;
    public CameraMove cameraMove;
    public PlayerAction playerAction;

    public MazeGeneration mazeGen;
    public Block[,] blockList;

    public int globalDebt = 1000;
    public int gameOverDebt = 3000;
    public UIManager uiManager;

    public int debtRate = 50;

    public static Color[] playerColors = {new Color(255/255f,242/255f,61/255f), 
        new Color(9/255f, 161/255f,0), new Color(255/255f, 64/255f,83/255f), new Color(0,128/255f,255/255f) }; 

    void Start()
    {
        StartCoroutine(initializeEverything());
    }

    IEnumerator initializeEverything()
    {
        //wait until maze generation done
        yield return new WaitUntil(() => mazeGen.nullParent.transform.childCount != 0);

        blockList = mazeGen.getBlockList();
        yield return playerAction.initializeAssign(blockList);
        StartCoroutine(executeTurns());
    }

    IEnumerator executeTurns()
    {
        while (true)
        {
            int playerIndex = turn % playerAction.numPlayers;
            cameraMove.setDestination(playerAction.players[playerIndex].myCenter);
            uiManager.profiles[playerIndex].Play("playerProfile");
            if (turn != 0) uiManager.profiles[(turn - 1) % playerAction.numPlayers].Play("playerProfileShrink");
            uiManager.displayDialogueForSeconds("Player " + playerIndex + "'s turn", 8);
            yield return playerAction.executeTurn(playerIndex);
            yield return new WaitForSeconds(0.2f);
            globalDebt += debtRate;
            uiManager.displayDialogueForSeconds("more debt has been added...", 3);
            yield return new WaitForSeconds(1f);

            checkForResult();
           turn++;
           

        }
    }

    public void setAllHighlightAndBuyableOff()
    {
        for(int i=0; i<blockList.GetLength(0); i++)
        {
            for(int j=0; j<blockList.GetLength(1); j++)
            {
                blockList[i, j].highlightOff();
                blockList[i, j].setBuyable(false);
            }
            
        }
    }


    void Update()
    {
        
    }

    public void checkForResult()
    {
        if(globalDebt > gameOverDebt)
        {
            uiManager.gameOver();
        }else if(globalDebt <= 0)
        {
            uiManager.victory();
        }
    }


    public int getCurrTurnPlayer()
    {
        return turn % playerAction.numPlayers;
    }

}
