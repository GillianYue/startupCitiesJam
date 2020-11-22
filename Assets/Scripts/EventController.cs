using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventController : MonoBehaviour
{
    public PlayerAction playerAction;

    //more debt
    public string[] moreDebtTexts;
    public int[] addedDebt;

    public string[] loseBlockTexts;


    public string[] blockPriceChangeTexts;
    public int[] blockPriceChange;

    public string[] changeCoinsTexts;
    public int[] changeCoinsAmount;



    public UIManager uiManager;
    public TurnController turnController;
    public BlockController blockController;

    public void pickRandomEvent()
    {
        StartCoroutine(pickRandomEventIE());
    }

    public IEnumerator pickRandomEventIE()
    {
        yield return new WaitForSeconds(0.2f);

        int eventType = UnityEngine.Random.Range(0, 4); //0, 1, 2, 3 //TODO change
        int eventIndex;

        switch (eventType)
        {
            case 0:
                eventIndex = UnityEngine.Random.Range(0, moreDebtTexts.Length);
                Tuple<string, int> tup = MoreDebet(eventIndex);
                //
                string tempTxt = ((tup.Item2 > 0) ? "increased" : "decreased");

                uiManager.eventOut("", tup.Item1, "Global debt "+tempTxt+" by " + Mathf.Abs(tup.Item2) + " !");
                turnController.globalDebt += tup.Item2;
                break;

            case 1:
                eventIndex = UnityEngine.Random.Range(0, loseBlockTexts.Length);

                List<int> house = new List<int>();

                for (int i = 0; i < 4; i++)
                    house.Add(playerAction.players[i].myBlocks.Count);

                Tuple<string, List<Tuple<int, int>>> tuple = LoseBlock(house, eventIndex);
                //
                string res = "";
                foreach (Tuple<int, int> t in tuple.Item2)
                {
                    int pID = t.Item1;
                    int numLost = t.Item2;
                    if (numLost != 0 && house[pID] != 1)
                    {
                        res += "Player " + pID + " lost " + numLost + " block(s) of land!\n";
                        //
                        for (int l = 0; l < numLost; l++)
                        {
                            //randomly lose one block
                            int ran = UnityEngine.Random.Range(0, playerAction.players[pID].myBlocks.Count);
                            Tuple<int, int> rant = playerAction.players[pID].myBlocks[ran];
                            turnController.blockList[rant.Item1, rant.Item2].setOwner(-1);
                            turnController.blockList[rant.Item1, rant.Item2].setMatColor(new Color(1, 1, 1));
                            playerAction.refreshCost();
                            playerAction.players[pID].myBlocks.RemoveAt(ran);
                        }
                    }
                }
                if(res!="")
                uiManager.eventOut("", tuple.Item1, res);
                break;
            case 3:
                eventIndex = UnityEngine.Random.Range(0, blockPriceChangeTexts.Length);

                Tuple<string, int> tt = Toodear(blockController.blockOriginalCost, eventIndex);
                string rr = "The base pricing of block(s) has changed by "+tt.Item2.ToString();
                uiManager.eventOut("", tt.Item1, rr);
                blockController.blockOriginalCost += tt.Item2;

                break;
            case 4:
                eventIndex = UnityEngine.Random.Range(0, changeCoinsTexts.Length);
                Tuple<string, int, int> tu = loseCoins(eventIndex, turnController.getCurrTurnPlayer());
                string temp = ((tu.Item3 > 0) ? "gained" : "lost");
                string result = "You " + temp + " " + Mathf.Abs(tu.Item3) + " coins.";
                uiManager.eventOut("", tu.Item1, result);

                playerAction.players[turnController.getCurrTurnPlayer()].addCoins(tu.Item3);

                break;

        }
    }


    public Tuple<string, int> MoreDebet(int eventIndex)
    {
        string info = moreDebtTexts[eventIndex];
        return new Tuple<string, int>(info, addedDebt[eventIndex]);
    }

    //返回一个list，里面是玩家丢失区域的信息， （玩家id，丢失的区域数量）
    //需要一个list，里面是所有玩家拥有的区域数量
    public Tuple<string, List<Tuple<int, int>>> LoseBlock(List<int> houseCond, int eventIndex)
    {
        string info = loseBlockTexts[eventIndex];

        int unfortunateness = UnityEngine.Random.Range(0, 4);
        List<Tuple<int, int>> outPut = new List<Tuple<int, int>>();
        Tuple<int, int> loseList;
        List<bool> list = new List<bool> { false, false, false, false };
        for (int i = 0; i < unfortunateness; i++)
        {
            list[UnityEngine.Random.Range(0, 4)] = true;
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i])
            {
                loseList = new Tuple<int, int>(i, UnityEngine.Random.Range(0, Mathf.Max(1, houseCond[i] / 2) + 1));

               // Debug.Log(loseList.Item2 + " " + houseCond[i]);
                outPut.Add(loseList);
            }

        }
        return new Tuple<string, List<Tuple<int, int>>>(info, outPut);
    }

    public Tuple<string, int> Toodear(int blockOriginalCost, int eventIndex)
    {
        string info = blockPriceChangeTexts[eventIndex];
        return new Tuple<string, int>(info, blockOriginalCost + blockPriceChange[eventIndex]);
    }


    //返回破产玩家的id，没收财产
    public Tuple<string, int, int> loseCoins(int eventIndex, int playerID)
    {
        if(playerID == -1) playerID = UnityEngine.Random.Range(0, 4);
        string info = changeCoinsTexts[eventIndex];
        int amount = changeCoinsAmount[eventIndex];
        return new Tuple<string, int, int>(info, playerID, amount);
    }

}
