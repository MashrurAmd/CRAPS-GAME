using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dice dice1; 
    public Dice dice2; 

    private void Start()
    {
        
        StartCoroutine(StartGame());
    }

    private System.Collections.IEnumerator StartGame()
    {
        // Roll both dice
        dice1.RollTheDice();
        dice2.RollTheDice();

        
        yield return new WaitForSeconds(1.5f);

        CompareDice();
    }

    private void CompareDice()
    {
        int dice1Value = dice1.CurrentVisibleFace;
        int dice2Value = dice2.CurrentVisibleFace;

        Debug.Log($"🎲 Dice 1 rolled: {dice1Value}");
        Debug.Log($"🎲 Dice 2 rolled: {dice2Value}");

        if (dice1Value > dice2Value)
        {
            Debug.Log("✅ Dice 1 is higher!");
        }
        else if (dice2Value > dice1Value)
        {
            Debug.Log("✅ Dice 2 is higher!");
        }
        else
        {
            Debug.Log("⚖️ It’s a tie!");
        }
    }
}
