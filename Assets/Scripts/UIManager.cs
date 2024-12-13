using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Text References")]
    [Tooltip("The text object for displaying the player's health")]
    [SerializeField] private TextMeshProUGUI healthText;
    [Tooltip("The text object for displaying the player's money")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [Tooltip("The text object for displaying the time survived")]
    [SerializeField] private TextMeshProUGUI timeSurvivedText;
    [Tooltip("The text object for displaying the zombies defeated")]
    [SerializeField] private TextMeshProUGUI zombiesDefeatedText;

    private float lastHealth = 0;
    private float lastMoney = 0;
    private float lastTimeSurvived = 0;
    private int lastZombiesDefeated = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persist across scene loads
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (healthText == null || moneyText == null || timeSurvivedText == null || zombiesDefeatedText == null)
        {
            Debug.LogError("UI Text references not set in the inspector!");
        }
        else
        {
            UpdateHealthText(lastHealth);
            UpdateMoneyText(lastMoney);
            UpdateTimeSurvivedText(lastTimeSurvived);
            UpdateZombiesDefeatedText(lastZombiesDefeated);
        }

    }

    public void UpdateHealthText(float currentHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}";
            lastHealth = currentHealth;
        }
    }

    public void UpdateMoneyText(float currentMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: ${currentMoney}";
            lastMoney = currentMoney;
        }
    }

    public void UpdateTimeSurvivedText(float timeSurvived)
    {
        int hours = Mathf.FloorToInt(timeSurvived / 3600);
        int minutes = Mathf.FloorToInt((timeSurvived % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeSurvived % 60);

        if (timeSurvivedText != null)
        {
            timeSurvivedText.text = $"Time: {hours:D2}:{minutes:D2}:{seconds:D2}";
        }
        lastTimeSurvived = timeSurvived;
    }

    public void UpdateZombiesDefeatedText(int zombiesDefeated)
    {
        if (zombiesDefeatedText != null)
        {
            zombiesDefeatedText.text = $"Kills: {zombiesDefeated}";
        }
        lastZombiesDefeated = zombiesDefeated;
    }
}