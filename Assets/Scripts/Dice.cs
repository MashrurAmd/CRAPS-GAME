using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private Sprite[] diceSides;
    private SpriteRenderer rend;

    public GameManager gameManager;

    [SerializeField] private int currentVisibleFace = 1;
    public int CurrentVisibleFace => currentVisibleFace;

    [Header("Audio")]
    public AudioSource audioSource;     // drag your AudioSource here
    public AudioClip rollSound;         // drag your dice roll sound here

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public IEnumerator Roll()
    {
        if (diceSides == null || diceSides.Length == 0)
        {
            Debug.LogError("No dice sprites loaded in Resources/DiceSides/");
            yield break;
        }

        // Play dice roll sound
        if (rollSound != null)
            audioSource.PlayOneShot(rollSound);

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
        gameManager.OnDiceClicked(this);
    }
}
