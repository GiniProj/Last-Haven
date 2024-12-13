using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Resource management
    [Header("Player")]
    [Tooltip("The movement speed of the player")]
    [SerializeField] private float playerSpeedMovement = 5f;
    [Tooltip("The starting health of the player")]
    [SerializeField] private float playerStartingHealth = 100f;
    [Tooltip("The damage dealt to the player when hit")]
    [SerializeField] private float playerDamageWhenHit = 10f;
    [Tooltip("The starting money of the player")]
    [SerializeField] private float playerStartingMoney = 100f;

    [Header("Fences")]
    [Tooltip("Array of fence objects")]
    [SerializeField] private Fence[] fences; // Array of fence objects
    [Tooltip("The maximum health of the fence")]
    [SerializeField] private float fenceMaxHealth = 20;
    [Tooltip("The damage dealt to the fence when hit")]
    [SerializeField] private float fenceDamageWhenHit = 10f;
    [Tooltip("The health added to the fence when repaired")]
    [SerializeField] private float fenceHealthToAddWhenRepair = 10f;
    [Tooltip("The cost to repair the fence")]
    [SerializeField] private float fenceRepairHealthCost = 5f;

    // Game state
    [Header("Zombie")]
    [Tooltip("The starting health of the zombie")]
    [SerializeField] private float zombieStartingHealth = 10f;
    [Tooltip("The damage dealt to the zombie when hit")]
    [SerializeField] private float zombieDamageWhenHit = 5f;
    [Tooltip("The money earned for each zombie kill")]
    [SerializeField] private float zombieMoneyForEachKill = 1f;
    [Tooltip("The movement speed of the zombie")]
    [SerializeField] private float zombieSpeed = 1f;
    [Tooltip("The spawn speed of the zombie")]
    [SerializeField] private float zombieSpawnSpeed = 1f;

    // Game state
    private int numberOfZombiesDefeated = 0;  // Number of zombies defeated
    private float survivalTime = 0f;

    // Getters
    //Player
    public float PlayerSpeed => playerSpeedMovement;
    public float PlayerHealth => playerStartingHealth;
    public float PlayerStartingMoney => playerStartingMoney;
    public float PlayerDamageWhenHit => playerDamageWhenHit;

    //Fences
    public Fence[] Fences => fences;
    public float FenceMaxHealth => fenceMaxHealth;
    public float FenceDamageWhenHit => fenceDamageWhenHit;
    public float FenceHealthToAddWhenRepair => fenceHealthToAddWhenRepair;
    public float FenceRepairCost => fenceRepairHealthCost;

    //Zombie
    public float ZombieMovementSpeed => zombieSpeed;
    public float ZombieSpawnSpeed => zombieSpawnSpeed;
    public float ZombieDamageWhenHit => zombieDamageWhenHit;
    public float ZombieStartingHealth => zombieStartingHealth;
    public int ZombiesDefeated => numberOfZombiesDefeated;
    public float MoneyForZombieKill => zombieMoneyForEachKill;
    //Game
    public float SurvivalTime => survivalTime;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if (fences.Length == 0) Debug.LogError("Fences array is empty in GameManager");
    }

    private void Start()
    {
        UIManager.Instance.UpdateZombiesDefeatedText(numberOfZombiesDefeated);
        UIManager.Instance.UpdateTimeSurvivedText(survivalTime);
    }

    private void Update()
    {
        survivalTime += Time.deltaTime;
        UIManager.Instance.UpdateTimeSurvivedText(survivalTime);
        UIManager.Instance.UpdateZombiesDefeatedText(numberOfZombiesDefeated);
        if (Player.Instance.CurrentPlayerHealth <= 0)
        {
            GameOver();
        }
    }

    public void AddZombieDefeated()
    {
        numberOfZombiesDefeated += 1;
        UIManager.Instance.UpdateZombiesDefeatedText(numberOfZombiesDefeated);
        Player.Instance.AddPlayerMoney();
    }


    public void GameOver()
    {
        Debug.Log($"Game Over! Survived for {survivalTime} seconds. Zombies defeated: {numberOfZombiesDefeated}");
    }
}