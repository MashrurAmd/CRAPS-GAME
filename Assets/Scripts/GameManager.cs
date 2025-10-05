using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Dice playerDice;
    public Dice botDice;

    public GameObject resultPanel;   // Assign the panel in Inspector
    public TMP_Text resultText;      // Assign the TMP_Text inside the panel

    private int playerRollValue;
    private int botRollValue;

    private bool playerCanRoll = true;

    private void Start()
    {
        resultPanel.SetActive(false); // panel initially inactive
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
        StartCoroutine(PlayerRollRoutine());
    }

    private IEnumerator PlayerRollRoutine()
    {
        // Player rolls
        yield return StartCoroutine(playerDice.Roll());
        playerRollValue = playerDice.CurrentVisibleFace;
        Debug.Log($"🎲 Player rolled {playerRollValue}");

        // Bot rolls
        yield return StartCoroutine(BotRollRoutine());
    }

    private IEnumerator BotRollRoutine()
    {
        yield return StartCoroutine(botDice.Roll());
        botRollValue = botDice.CurrentVisibleFace;
        Debug.Log($"🎲 Bot rolled {botRollValue}");

        // Move dice to center
        yield return StartCoroutine(MoveDiceToCenter());

        // Activate the result panel
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
        resultPanel.SetActive(true); // show panel

        if (playerRollValue > botRollValue)
            resultText.text = "Player Wins! Player is now the Striker";
        else if (botRollValue > playerRollValue)
            resultText.text = "Bot Wins! Bot is now the Striker";
        else
            resultText.text = "It's a Tie!";

        // Automatically hide panel after 3 seconds
        StartCoroutine(HidePanelAfterDelay(3f));
    }

    private IEnumerator HidePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        resultPanel.SetActive(false); // hide panel
        playerCanRoll = true;         // allow next round
        Debug.Log("🎲 Click dice to roll for the next round!");
    }

}
