using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TurnController turnController;

    public Slider payEarnToggle;
    public Text debtText;
    public PlayerAction playerAction;
    public Text[] playerCoinTexts;

    private bool ready;

    public Text dialogueText;

    public Animator[] profiles;

    void Start()
    {
        StartCoroutine(waitReady());
    }

    IEnumerator waitReady()
    {
        yield return new WaitUntil(() => playerAction.ready);
        ready = true;
    }

    void Update()
    {
        if (ready)
        {
            debtText.text = "Debt Left: " + turnController.globalDebt.ToString();

            for (int p = 0; p < playerAction.numPlayers; p++)
            {
                playerCoinTexts[p].text = "coins: " + playerAction.players[p].getCoins().ToString();
            }

        }
    }

    public void togglePayEarn(int value)
    {
        payEarnToggle.value = value;

    }

    public void displayDialogue(string content)
    {
        dialogueText.text = content;
    }
    public void displayDialogueForSeconds(string content, float s)
    {
        StopAllCoroutines();
        StartCoroutine(displayForSeconds(dialogueText, s, content));
    }

    IEnumerator displayForSeconds(Text t, float s, string content)
    {
        t.text = content;
        yield return new WaitForSeconds(s);
        t.text = "";
    }
}
