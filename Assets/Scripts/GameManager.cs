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

    [Header("Roll Button")]
    public GameObject RollButton; // assign your UI button here

    private int playerRollValue;
    private int botRollValue;

    private bool playerCanRoll = true;
    private bool playerIsStriker = true;
    private bool playerIsWinner = false;
    private bool gameEnded = false;

    private int strikerGoal = 0;
    private bool goalPhaseActive = false;

    private void Start()
    {
        FirstRulePannel.SetActive(true);
        SecondPanel.SetActive(false);
        RollButton.SetActive(true);

        FirstRuleText.text = "Click ROLL to start!";
        Debug.Log("🎲 Game started! Player rolls first.");
    }

    // 🔘 Called when the roll button is pressed
    public void OnRollButtonPressed()
    {
        if (gameEnded) return;

        // First round
        if (playerCanRoll && !playerIsWinner && !goalPhaseActive)
        {
            playerCanRoll = false;
            StartCoroutine(FirstRound());
        }
        // Player is striker (manual phase)
        else if (playerIsWinner && !goalPhaseActive && playerIsStriker)
        {
            StartCoroutine(PlayerDoubleRoll());
        }
        // Goal phase
        else if (goalPhaseActive && playerIsStriker)
        {
            StartCoroutine(PlayerGoalPhaseRoll());
        }
    }

    private IEnumerator FirstRound()
    {
        // Roll both dice (player vs bot)
        yield return StartCoroutine(playerDice.Roll(1.5f));
        playerRollValue = playerDice.CurrentVisibleFace;

        yield return StartCoroutine(botDice.Roll(1.2f));
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
            FirstRuleText.text = "🎉 Player wins the round and becomes STRIKER!\nPress ROLL to roll both dice!";
            playerIsStriker = true;
            playerIsWinner = true;
        }
        else if (botRollValue > playerRollValue)
        {
            FirstRuleText.text = "🤖 Bot wins the round and becomes STRIKER!";
            playerIsStriker = false;
            playerIsWinner = false;
            StartCoroutine(BotDoubleRoll());
        }
        else
        {
            FirstRuleText.text = "😐 It's a tie! Press ROLL again!";
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

    private IEnumerator PlayerDoubleRoll()
    {
        // Player rolls both dice simultaneously
        yield return StartCoroutine(RollBothDice(playerDice, botDice, 1.6f));

        int sum = playerDice.CurrentVisibleFace + botDice.CurrentVisibleFace;
        ShowSumPanel($"🎯 You rolled both dice!\nSum = {sum}");
        yield return new WaitForSeconds(0.5f);

        CheckStrikerResult(sum, true);
    }

    private IEnumerator RollBothDice(Dice dice1, Dice dice2, float duration = 1.5f)
    {
        Coroutine c1 = StartCoroutine(dice1.Roll(duration));
        Coroutine c2 = StartCoroutine(dice2.Roll(duration));
        yield return c1;
        yield return c2;
    }

    private IEnumerator BotDoubleRoll()
    {
        yield return StartCoroutine(RollBothDice(botDice, playerDice, 1.2f));
        int sum = botDice.CurrentVisibleFace + playerDice.CurrentVisibleFace;

        ShowSumPanel($"🤖 Bot rolled both dice!\nSum = {sum}");
        yield return new WaitForSeconds(0.5f);

        CheckStrikerResult(sum, false);
    }

    private void ShowSumPanel(string message)
    {
        FirstRulePannel.SetActive(false);
        SecondPanel.SetActive(true);
        SecondText.text = message;
    }

    private void CheckStrikerResult(int sum, bool strikerIsPlayer)
    {
        if (sum == 7 || sum == 11)
        {
            EndGame(strikerIsPlayer, true);
        }
        else if (sum == 2 || sum == 3 || sum == 12)
        {
            EndGame(strikerIsPlayer, false);
        }
        else
        {
            strikerGoal = sum;
            goalPhaseActive = true;
            SecondText.text += $"\n\n🎯 Goal is set to {strikerGoal}!";
            Debug.Log($"Goal phase started. Striker needs {strikerGoal}, opponent needs 7.");

            if (strikerIsPlayer)
            {
                SecondText.text += "\nPress ROLL to keep rolling!";
            }
            else
            {
                StartCoroutine(BotGoalPhase());
            }
        }
    }

    private IEnumerator PlayerGoalPhaseRoll()
    {
        yield return StartCoroutine(RollBothDice(playerDice, botDice, 1.5f));
        int sum = playerDice.CurrentVisibleFace + botDice.CurrentVisibleFace;

        SecondText.text = $"🎯 You rolled {sum}";

        if (sum == strikerGoal)
        {
            EndGame(true, true);
        }
        else if (sum == 7)
        {
            EndGame(true, false);
        }
        else
        {
            SecondText.text += $"\nKeep rolling for {strikerGoal}!";
        }
    }

    private IEnumerator BotGoalPhase()
    {
        yield return new WaitForSeconds(1f);

        while (!gameEnded)
        {
            yield return StartCoroutine(RollBothDice(botDice, playerDice, 1.2f));
            int sum = botDice.CurrentVisibleFace + playerDice.CurrentVisibleFace;

            SecondText.text = $"🤖 Bot rolled {sum}";

            if (sum == strikerGoal)
            {
                EndGame(false, true);
                yield break;
            }
            else if (sum == 7)
            {
                EndGame(false, false);
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void EndGame(bool strikerIsPlayer, bool strikerWon)
    {
        gameEnded = true;
        goalPhaseActive = false;

        string resultMsg;
        if (strikerWon)
        {
            resultMsg = strikerIsPlayer ? "🎉 PLAYER (Striker) wins the game!" : "🤖 BOT (Striker) wins the game!";
        }
        else
        {
            resultMsg = strikerIsPlayer ? "❌ BOT wins the game!" : "❌ PLAYER wins the game!";
        }

        SecondText.text += $"\n\n{resultMsg}";
        Debug.Log(resultMsg);
        RollButton.SetActive(false);
    }
}

