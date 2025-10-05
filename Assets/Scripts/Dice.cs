using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private Sprite[] diceSides;
    private SpriteRenderer rend;

    public GameManager gameManager;

    [SerializeField] private int currentVisibleFace = 1;
    public int CurrentVisibleFace => currentVisibleFace;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        Debug.Log($"Loaded diceSides: {diceSides?.Length ?? 0}");
    }

    // Roll coroutine that GameManager can wait for
    public IEnumerator Roll()
    {
        if (diceSides == null || diceSides.Length == 0)
        {
            Debug.LogError("No dice sprites loaded. Put sprites into Assets/Resources/DiceSides/");
            yield break;
        }

        if (rend == null)
        {
            Debug.LogError("No SpriteRenderer found on " + gameObject.name);
            yield break;
        }

        int randomDiceSide = 0;
        for (int i = 0; i <= 20; i++)
        {
            randomDiceSide = Random.Range(0, 6);
            rend.sprite = diceSides[randomDiceSide];
            currentVisibleFace = randomDiceSide + 1;
            yield return new WaitForSeconds(0.05f);
        }

        currentVisibleFace = randomDiceSide + 1;
        Debug.Log($"{gameObject.name} rolled {currentVisibleFace}");
    }

    private void OnMouseDown()
    {
        if (gameManager != null)
        {
            gameManager.OnDiceClicked(this);
        }
    }
}
