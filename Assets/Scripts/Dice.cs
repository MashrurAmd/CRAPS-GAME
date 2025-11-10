using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [Header("References")]
    private SpriteRenderer rend;
    private Sprite[] faceSprites;       // 1–6 dice faces
    private Sprite[] spinFrames;        // rotation animation frames

    public GameManager gameManager;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip rollSound;

    [Header("Config")]
    [SerializeField] private float rollDuration = 1.5f; // total time per roll

    [SerializeField] private int currentVisibleFace = 1;
    public int CurrentVisibleFace => currentVisibleFace;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        faceSprites = Resources.LoadAll<Sprite>("DiceSides/");
        spinFrames = Resources.LoadAll<Sprite>("DiceSpinFrames/");

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (faceSprites.Length == 0)
            Debug.LogError("❌ No dice face sprites found in Resources/DiceSides/");
        if (spinFrames.Length == 0)
            Debug.LogWarning("⚠️ No spin animation frames found in Resources/DiceSpinFrames/");
    }

    public IEnumerator Roll(float duration = -1f)
    {
        if (faceSprites.Length == 0)
        {
            Debug.LogError("❌ Missing face sprites");
            yield break;
        }

        if (duration < 0) duration = rollDuration;

        if (rollSound != null)
            audioSource.PlayOneShot(rollSound);

        float elapsed = 0f;
        int finalFace = Random.Range(0, faceSprites.Length);

        while (elapsed < duration)
        {
            // 🔄 Play rotation animation frames if available
            if (spinFrames.Length > 0)
            {
                foreach (var frame in spinFrames)
                {
                    rend.sprite = frame;
                    yield return new WaitForSeconds(0.05f);
                    elapsed += 0.05f;
                    if (elapsed >= duration) break;
                }
            }
            else
            {
                // fallback: just randomize static dice sides
                int randomSide = Random.Range(0, faceSprites.Length);
                rend.sprite = faceSprites[randomSide];
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }
        }

        // 🧊 Stop on a random dice face
        rend.sprite = faceSprites[finalFace];
        currentVisibleFace = finalFace + 1;

        Debug.Log($"{gameObject.name} rolled {currentVisibleFace}");
    }
}
