using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Dice References")]
    public Dice playerDice;
    public Dice botDice;

    [Header("Panels & Texts")]
    public GameObject FirstRulePanel;
    public TMP_Text FirstRuleText;
    public GameObject SecondRulePanel;
    public TMP_Text SecondRuleText;

    private int playerRollValue;
    private int botRollValue;

    private bool playerCanRoll = true;
    private bool playerIsWinner = false;
    private bool gameEnded = false;

    private int manualRollCount = 0; // player manual roll counter
    private int[] playerManualResults = new int[2]; // stores dice results

    private void Start()
    {
        FirstRulePanel.SetActive(true);
        SecondRulePanel.SetActive(false);
        FirstRuleText.text = "Click your dice to roll!";
        Debug.Log("🎲 Game start — Player clicks dice to roll.");
    }

    // Called when a dice is clicked
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
        FirstRulePanel.SetActive(false);

        // Player rolls their dice
        yield return StartCoroutine(playerDice.Roll());
        playerRollValue = playerDice.CurrentVisibleFace;

        // Bot rolls its own dice
        yield return StartCoroutine(botDice.Roll());
        botRollValue = botDice.CurrentVisibleFace;

        Debug.Log($"🎲 Player rolled {playerRollValue}, Bot rolled {botRollValue}");
        yield return StartCoroutine(MoveDiceToCenter());

        // Decide who wins
        if (playerRollValue > botRollValue)
        {
            Debug.Log("✅ Player wins the round!");
            playerIsWinner = true;
            manualRollCount = 0;
            FirstRulePanel.SetActive(true);
            FirstRuleText.text = "You won! Roll both dice one by one.";
            playerCanRoll = true;
        }
        else if (botRollValue > playerRollValue)
        {
            Debug.Log("🤖 Bot wins the round!");
            StartCoroutine(BotRollsBothDice());
        }
        else
        {
            Debug.Log("⚖️ It's a tie! Roll again.");
            FirstRulePanel.SetActive(true);
            FirstRuleText.text = "It's a Tie! Click your dice to roll again.";
            playerCanRoll = true;
        }
    }

    private IEnumerator PlayerManualDoubleRoll(Dice clickedDice)
    {
        if (manualRollCount >= 2)
            yield break;

        yield return StartCoroutine(clickedDice.Roll());
        int result = clickedDice.CurrentVisibleFace;

        playerManualResults[manualRollCount] = result;
        manualRollCount++;

        Debug.Log($"🎯 Manual roll {manualRollCount}: {result}");

        if (manualRollCount >= 2)
        {
            int sum = playerManualResults[0] + playerManualResults[1];
            ShowSumPanel($"You rolled both dice!\nSum: {sum}");
            Debug.Log($"🎯 Player's total sum: {sum}");
            gameEnded = true;
        }
    }

    private IEnumerator BotRollsBothDice()
    {
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(botDice.Roll());
        int dice1 = botDice.CurrentVisibleFace;
        yield return StartCoroutine(playerDice.Roll());
        int dice2 = playerDice.CurrentVisibleFace;

        int sum = dice1 + dice2;
        ShowSumPanel($"Bot rolled both dice!\nSum: {sum}");
        Debug.Log($"🎯 Bot's total sum: {sum}");
        gameEnded = true;
    }

    private IEnumerator MoveDiceToCenter()
    {
        yield return new WaitForSeconds(0.3f);

        Vector3 playerTarget = new Vector3(-1f, 0f, 0f);
        Vector3 botTarget = new Vector3(1f, 0f, 0f);

        playerDice.transform.DOMove(playerTarget, 1f);
        botDice.transform.DOMove(botTarget, 1f);

        yield return new WaitForSeconds(1f);
    }

    private void ShowSumPanel(string message)
    {
        FirstRulePanel.SetActive(false);
        SecondRulePanel.SetActive(true);
        SecondRuleText.text = message;
    }
}

