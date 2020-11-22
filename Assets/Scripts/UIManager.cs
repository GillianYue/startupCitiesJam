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

    public bool myReady;

    public Text dialogueText;

    public Animator[] profiles;

    public GameObject successPanel, gameOverPanel, eventPanel, startGamePanel, tutorialPanel;

    public Text eventTitle, eventBody, eventEnd;

    public bool inUI;

    void Start()
    {
        //startGameAnim();
        StartCoroutine(waitReady());
    }

    IEnumerator waitReady()
    {
        yield return new WaitUntil(() => playerAction.ready());
        myReady = true;
    }

    public void startGameAnim()
    {
        inUI = true;
        startGamePanel.SetActive(true);
        startGamePanel.GetComponent<Animator>().Play("StartGame");
        StartCoroutine(disableStartGamePanel());
    }

    public IEnumerator disableStartGamePanel() {
        yield return new WaitForSeconds(27.3f);
        startGamePanel.SetActive(false);
        inUI = false;
        StartCoroutine(displayTutorialPanel());
    }

    public IEnumerator displayTutorialPanel()
    {
        inUI = true;
        tutorialPanel.SetActive(true);
        tutorialPanel.GetComponent<Animator>().Play("StartTutorial");
        yield return new WaitForSeconds(10f);
        tutorialPanel.SetActive(false);
        inUI = false;
    }

    public void setInUI(bool b) { inUI = b;  }
    void Update()
    {
        if (myReady)
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
        //StopAllCoroutines();
        StartCoroutine(displayForSeconds(dialogueText, s, content));
    }

    IEnumerator displayForSeconds(Text t, float s, string content)
    {
        t.text = content;
        yield return new WaitForSeconds(s);
        t.text = "";
    }

    public void victory()
    {
        inUI = true;
        successPanel.SetActive(true);
        successPanel.GetComponent<Animator>().Play("CardOut");
    }

    public void gameOver()
    {
        inUI = true;
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<Animator>().Play("CardOut");
    }

    public void eventOut(string title, string body, string end)
    {
        if (title != "") eventTitle.text = title;
        if (body != "") eventBody.text = body;
        if (end != "") eventEnd.text = end;

        inUI = true;
        eventPanel.SetActive(true);
        eventPanel.GetComponent<Animator>().Play("CardOut");
        StartCoroutine(eventIn());
    }

    IEnumerator eventIn()
    {
        yield return new WaitForSeconds(6);
        eventPanel.GetComponent<Animator>().Play("CardIn");
        yield return new WaitForSeconds(1);
        eventPanel.SetActive(false);
        inUI = false;
    }
}
