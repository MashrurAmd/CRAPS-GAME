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
    public AudioSource audioSource;
    public AudioClip rollSound;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Adjustable roll duration for realism
    public IEnumerator Roll(float rollDuration = 1.5f)
    {
        if (diceSides == null || diceSides.Length == 0)
        {
            Debug.LogError("No dice sprites loaded in Resources/DiceSides/");
            yield break;
        }

        if (rollSound != null)
            audioSource.PlayOneShot(rollSound);

        float elapsed = 0f;
        int randomDiceSide = 0;

        while (elapsed < rollDuration)
        {
            randomDiceSide = Random.Range(0, 6);
            rend.sprite = diceSides[randomDiceSide];
            currentVisibleFace = randomDiceSide + 1;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        currentVisibleFace = randomDiceSide + 1;
        Debug.Log($"{gameObject.name} rolled {currentVisibleFace}");
    }
}
