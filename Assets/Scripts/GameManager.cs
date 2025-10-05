using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dice playerDice;
    public Dice botDice;

    private int playerRollValue;
    private int botRollValue;

    // Flag to control when player can roll
    private bool playerCanRoll = true;

    // Called at start to show instructions
    private void Start()
    {
        Debug.Log("🎲 Player, click the roll button to shoot the dice!");
    }

    // Call this from a UI Button or input to roll player dice
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
        Debug.Log("🧍 Player is rolling...");
        yield return StartCoroutine(playerDice.Roll());

        // Save player dice value
        playerRollValue = playerDice.CurrentVisibleFace;

        Debug.Log($"🎲 Player rolled {playerRollValue}");

        // Now let the bot roll
        yield return StartCoroutine(BotRollRoutine());
    }

    private IEnumerator BotRollRoutine()
    {
        Debug.Log("🤖 Bot is rolling...");
        yield return StartCoroutine(botDice.Roll());

        // Save bot dice value
        botRollValue = botDice.CurrentVisibleFace;
        Debug.Log($"🎲 Bot rolled {botRollValue}");

        // Compare both rolls
        CompareRolls();
    }

    private void CompareRolls()
    {
        if (playerRollValue > botRollValue)
            Debug.Log("🎯 Player wins! Player is now the Shooter\"");
        else if (botRollValue > playerRollValue)
            Debug.Log("🤖 Bot wins!, Bot is now the Shooter");
        else
            Debug.Log("⚖️ It's a tie!");

        
        playerCanRoll = true;

    }
}
