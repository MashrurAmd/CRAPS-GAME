using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{


    private Sprite[] diceSides;

  
    private SpriteRenderer rend;


    [SerializeField] private int currentVisibleFace = 1;


    public int CurrentVisibleFace => currentVisibleFace;


    private void Start()
    {
        // Assign Renderer component
        rend = GetComponent<SpriteRenderer>();


        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        Debug.Log($"Loaded diceSides: {diceSides?.Length ?? 0}");
    }

    private void OnMouseDown()
    {
        StartCoroutine(RollTheDice());
    }

    // Coroutine that rolls the dice
    public IEnumerator RollTheDice()
    {
        // Safety checks
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
        int finalSide = 0;

        for (int i = 0; i <= 20; i++)
        {
            randomDiceSide = Random.Range(0, 6); 
            rend.sprite = diceSides[randomDiceSide];


            currentVisibleFace = randomDiceSide + 1;

            yield return new WaitForSeconds(0.05f);
        }

        finalSide = randomDiceSide + 1;
        currentVisibleFace = finalSide; 
        Debug.Log(finalSide);
    }
}

