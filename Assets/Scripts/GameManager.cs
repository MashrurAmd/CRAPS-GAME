using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Dice playerDice;
    public Dice botDice;

    public GameObject FirstRulePannel;   // Assign the panel in Inspector
    public TMP_Text FirstRuleText;       // Assign the TMP_Text inside the panel

    private int playerRollValue;
    private int botRollValue;

    private bool playerCanRoll = true;
    private bool playerIsStriker = true; // keeps track who won previous round

    private void Start()
    {
        FirstRulePannel.SetActive(false); // panel initially inactive
        Debug.Log("🎲 Click the player dice to roll!");
    }

    public void PlayerRoll()
    {
        if (!playerCanRoll)
        {
            Debug.Log("❌ Wait for your turn!");
            return;
        }

        playerCanRoll = false;
        StartCoroutine(RollRound(playerIsStriker));
    }

    private IEnumerator RollRound(bool playerRollsBoth)
    {
        if (playerRollsBoth)
        {
            // Player rolls both dice
            yield return StartCoroutine(playerDice.Roll());
            playerRollValue = playerDice.CurrentVisibleFace;
            yield return StartCoroutine(botDice.Roll());
            botRollValue = botDice.CurrentVisibleFace;
        }
        else
        {
            // Bot rolls both dice automatically
            yield return StartCoroutine(botDice.Roll());
            botRollValue = botDice.CurrentVisibleFace;
            yield return StartCoroutine(playerDice.Roll());
            playerRollValue = playerDice.CurrentVisibleFace;
        }

        Debug.Log($"🎲 Player: {playerRollValue}, Bot: {botRollValue}");

        // Move dice to center
        yield return StartCoroutine(MoveDiceToCenter());

        // Show result
        ShowWinnerPanel();
    }

    private IEnumerator MoveDiceToCenter()
    {
        yield return new WaitForSeconds(0.5f); // small delay after roll

        Vector3 centerLeft = new Vector3(-1f, 0f, 0f);  // player dice
        Vector3 centerRight = new Vector3(1f, 0f, 0f);  // bot dice

        // Smooth animation
        playerDice.transform.DOMove(centerLeft, 1f);
        botDice.transform.DOMove(centerRight, 1f);

        yield return new WaitForSeconds(1f); // wait until movement finishes
    }

    private void ShowWinnerPanel()
    {
        FirstRulePannel.SetActive(true); // show panel

        if (playerRollValue > botRollValue)
        {
            FirstRuleText.text = "Player Wins! Player is now the Striker";
            playerIsStriker = true;
        }
        else if (botRollValue > playerRollValue)
        {
            FirstRuleText.text = "Bot Wins! Bot is now the Striker";
            playerIsStriker = false;
        }
        else
        {
            FirstRuleText.text = "It's a Tie!";
        }

        // Automatically hide panel after 3 seconds
        StartCoroutine(HidePanelAfterDelay(3f));
    }

    private IEnumerator HidePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        FirstRulePannel.SetActive(false); // hide panel

        // Decide next turn
        if (playerIsStriker)
        {
            Debug.Log("🎲 Player's turn: click dice to roll both!");
            playerCanRoll = true; // allow player to roll next round
        }
        else
        {
            Debug.Log("🎲 Bot's turn: rolling both dice automatically...");
            playerCanRoll = false;
            StartCoroutine(RollRound(false));
        }
    }
}

