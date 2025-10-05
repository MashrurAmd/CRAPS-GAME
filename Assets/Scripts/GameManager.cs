using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Dice References")]
    public Dice playerDice;
    public Dice botDice;

    [Header("UI Panels")]
    public GameObject FirstRulePannel;
    public TMP_Text FirstRuleText;
    public GameObject SecondPanel;
    public TMP_Text SecondText;

    private int playerRollValue;
    private int botRollValue;

    private bool playerCanRoll = true;
    private bool playerIsStriker = true;
    private bool playerIsWinner = false;
    private bool gameEnded = false;

    private int manualRollCount = 0;
    private int[] playerManualResults = new int[2];

    private void Start()
    {
        FirstRulePannel.SetActive(true);
        SecondPanel.SetActive(false);
        FirstRuleText.text = "Click your dice to start!";
        Debug.Log("🎲 Game started! Player rolls first.");
    }

    public void OnDiceClicked(Dice clickedDice)
    {
        if (gameEnded) return;

        // --- FIRST ROUND: player rolls their dice ---
        if (playerCanRoll && !playerIsWinner)
        {
            if (clickedDice == playerDice)
            {
                playerCanRoll = false;
                StartCoroutine(FirstRound());
            }
            else
            {
                Debug.Log("❌ You can only roll your own dice this round!");
            }
        }
        // --- PLAYER WON: must roll both dice manually ---
        else if (playerIsWinner)
        {
            StartCoroutine(PlayerManualDoubleRoll(clickedDice));
        }
    }

    private IEnumerator FirstRound()
    {
        yield return StartCoroutine(playerDice.Roll());
        playerRollValue = playerDice.CurrentVisibleFace;

        yield return StartCoroutine(botDice.Roll());
        botRollValue = botDice.CurrentVisibleFace;

        Debug.Log($"🎲 Player: {playerRollValue}, Bot: {botRollValue}");

        yield return StartCoroutine(MoveDiceToCenter());
        DetermineWinner();
    }

    private void DetermineWinner()
    {
        FirstRulePannel.SetActive(true);

        if (playerRollValue > botRollValue)
        {
            FirstRuleText.text = "🎉 Player wins the round and becomes the STRIKER!\nClick both dice one by one!";
            playerIsStriker = true;
            playerIsWinner = true;
            manualRollCount = 0;
        }
        else if (botRollValue > playerRollValue)
        {
            FirstRuleText.text = "🤖 Bot wins the round and becomes the STRIKER!";
            playerIsStriker = false;
            playerIsWinner = false;
            manualRollCount = 0;
            StartCoroutine(BotDoubleRoll());
        }
        else
        {
            FirstRuleText.text = "😐 It's a tie! Roll again!";
            playerCanRoll = true;
        }
    }

    private IEnumerator MoveDiceToCenter()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 centerLeft = new Vector3(-1f, 0f, 0f);
        Vector3 centerRight = new Vector3(1f, 0f, 0f);

        playerDice.transform.DOMove(centerLeft, 1f);
        botDice.transform.DOMove(centerRight, 1f);

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PlayerManualDoubleRoll(Dice clickedDice)
    {
        if (manualRollCount >= 2)
            yield break;

        yield return StartCoroutine(clickedDice.Roll());
        int result = clickedDice.CurrentVisibleFace;

        playerManualResults[manualRollCount] = result;
        manualRollCount++;

        if (manualRollCount >= 2)
        {
            int sum = playerManualResults[0] + playerManualResults[1];
            ShowSumPanel($"🎯 You rolled both dice!\nSum = {sum}");
            yield return new WaitForSeconds(0.5f);
            CheckStrikerResult(sum, true);
            gameEnded = true;
        }
    }

    private IEnumerator BotDoubleRoll()
    {
        yield return StartCoroutine(botDice.Roll());
        int roll1 = botDice.CurrentVisibleFace;

        yield return StartCoroutine(playerDice.Roll());
        int roll2 = playerDice.CurrentVisibleFace;

        int sum = roll1 + roll2;
        ShowSumPanel($"🤖 Bot rolled both dice!\nSum = {sum}");
        yield return new WaitForSeconds(0.5f);
        CheckStrikerResult(sum, false);
        gameEnded = true;
    }

    private void ShowSumPanel(string message)
    {
        FirstRulePannel.SetActive(false);
        SecondPanel.SetActive(true);
        SecondText.text = message;
    }

    private void CheckStrikerResult(int sum, bool strikerIsPlayer)
    {
        string resultMsg;

        if (sum == 7 || sum == 11)
        {
            resultMsg = strikerIsPlayer ? "🎉 PLAYER (Striker) wins the game!" : "🤖 BOT (Striker) wins the game!";
        }
        else if (sum == 2 || sum == 3 || sum == 12)
        {
            resultMsg = strikerIsPlayer ? "❌ BOT wins the game!" : "❌ PLAYER wins the game!";
        }
        else
        {
            resultMsg = "😐 No one wins this round!";
        }

        SecondText.text += $"\n\n{resultMsg}";
        Debug.Log(resultMsg);
    }
}


