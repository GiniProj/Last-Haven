using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }



    // Resource management
    [Header("Player")]
    [SerializeField] private float playerSpeedMovement = 5f;
    [SerializeField] private float playerStartingHealth = 100f;
    [SerializeField] private float playerDamageWhenHit = 10f;
    [SerializeField] private float playerStartingMoney = 100f;

    [Header("Fences")]
    [SerializeField] private Fence[] fences; // Array of fence objects
    [SerializeField] private float fenceMaxHealth = 20;
    [SerializeField] private float fenceDamageWhenHit = 10f;
    [SerializeField] private float fenceHealthToAddWhenRepair = 10f;
    [SerializeField] private float fenceRepairHealthCost = 5f;
    [SerializeField] private float fenceBuildNewCost = 40f;

    // Game state
    [Header("Zombie")]
    [SerializeField] private float zombieStartingHealth = 10f;
    [SerializeField] private float zombieDamageWhenHit = 5f;
    [SerializeField] private float zombieMoneyForEachKill = 1f;
    [SerializeField] private float zombieSpeed = 1f;
    [SerializeField] private float zombieSpawnSpeed = 1f;

    private int numberOfZombiesDefeated = 0;
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
    public float FenceBuildNewCost => fenceBuildNewCost;
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

    private void Update()
    {
        survivalTime += Time.deltaTime;

        if (Player.Instance.CurrentPlayerHealth <= 0)
        {
            GameOver();
        }
    }

    public void AddZombieDefeated()
    {
        numberOfZombiesDefeated += 1;
        Player.Instance.AddPlayerMoney();
    }


    public void GameOver()
    {
        Debug.Log($"Game Over! Survived for {survivalTime} seconds. Zombies defeated: {numberOfZombiesDefeated}");
    }
}