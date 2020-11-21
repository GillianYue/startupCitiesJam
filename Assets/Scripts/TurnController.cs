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
