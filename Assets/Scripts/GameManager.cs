using System.Collections;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public Dice playerDice;
    public Dice botDice;

    private int playerRollValue;
    private int botRollValue;

    private bool playerCanRoll = true;

    private void Start()
    {
        Debug.Log("🎲 Click the player dice to roll!");
    }

    // Call this when player clicks dice
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

        // Move dice to center side by side
        yield return StartCoroutine(MoveDiceToCenter());

        // Compare and log winner
        if (playerRollValue > botRollValue)
            Debug.Log("🎯 Player wins!");
        else if (botRollValue > playerRollValue)
            Debug.Log("🤖 Bot wins!");
        else
            Debug.Log("⚖️ It's a tie!");

        // Allow next round
        playerCanRoll = true;
        Debug.Log("🎲 Click dice to roll again!");
    }

    private IEnumerator MoveDiceToCenter()
    {
        //Delay before moving
        yield return new WaitForSeconds(1f);


        Vector3 centerLeft = new Vector3(-1f, 0f, 0f);  // player dice position
        Vector3 centerRight = new Vector3(1f, 0f, 0f);  // bot dice position

        // Animate movement over 1 seconds
        playerDice.transform.DOMove(centerLeft, 1f);
        botDice.transform.DOMove(centerRight, 1f);

        yield return new WaitForSeconds(0.5f);
    }
}
