using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TurnController turnController;

    public Slider payEarnToggle;
    public Text debtText;

    void Start()
    {
        
    }

    void Update()
    {
        debtText.text = "Debt Left: " + turnController.globalDebt.ToString();
    }

    public void togglePayEarn(int value)
    {
        payEarnToggle.value = value;

    }
}
