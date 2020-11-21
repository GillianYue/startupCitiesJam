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

    public int globalDebt = 100;
    public UIManager uiManager;

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
           turn++;
           

        }
    }


    void Update()
    {
        
    }


    public int getCurrTurnPlayer()
    {
        return turn % playerAction.numPlayers;
    }
}
