using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance { get; set; }

    [Header("Player Action Settings")]
    [Tooltip("The transform point where the weapon is located")]
    [SerializeField] private Transform weaponPoint;
    [Tooltip("The bullet prefab for shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [Tooltip("The cooldown time between each shot")]
    [SerializeField] private float shootCooldown = 0.5f;
    [Tooltip("Reference to the Rigidbody2D component")]
    [SerializeField] private Rigidbody2D rb;
    [Tooltip("Reference to the Collider2D component")]
    [SerializeField] private Collider2D playerCollider;

    private float moveSpeed;  // The movement speed of the player
    private float currentHealth;  // The current health of the player
    private float currentMoney;  // The current money of the player
    private float lastShootTime;  // The time of the last shot
    private bool isAlive = true;  // Whether the player is alive

    public float CurrentPlayerHealth => currentHealth;
    public bool IsPlayerAlive => isAlive;

    // Singleton pattern
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Setup Rigidbody
        rb = GetComponent<Rigidbody2D>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }

        currentHealth = GameManager.Instance.PlayerHealth;
        moveSpeed = GameManager.Instance.PlayerSpeed;
        currentMoney = GameManager.Instance.PlayerStartingMoney;

        UIManager.Instance.UpdateHealthText(currentHealth);
        UIManager.Instance.UpdateMoneyText(currentMoney);
    }

    void Update()
    {
        if (!isAlive) return;
        HandleAiming();
        HandleActions();
    }

    void FixedUpdate()
    {
        if (!isAlive) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(horizontal, vertical);

        if (movement.magnitude > 0.1f)
        {
            movement = movement.normalized;

            // Calculate the desired position
            Vector2 desiredPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;

            // Check for potential collisions before moving
            RaycastHit2D hit = Physics2D.Raycast(rb.position, movement, Vector2.Distance(rb.position, desiredPosition), LayerMask.GetMask("Fence"));

            if (hit.collider == null || hit.collider.gameObject.GetComponent<Fence>().IsFenceDestroyed)
            {
                // No fence collision or fence is destroyed, allow movement
                rb.MovePosition(desiredPosition);
            }
        }
    }

    private void HandleAiming()
    {
        // Get mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Calculate direction to mouse
        Vector2 direction = (mousePosition - transform.position).normalized;

        // Calculate angle for rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void HandleActions()
    {
        // Shooting
        if (Input.GetMouseButton(0) && Time.time > lastShootTime + shootCooldown)
        {
            ActionShoot();
            lastShootTime = Time.time;
        }

        // Repair Fence
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Fence"))
            {
                Fence fence = hit.collider.GetComponent<Fence>();
                if (fence != null)
                {
                    ActionRepairFence(fence);
                }
            }
        }
    }

    private void ActionRepairFence(Fence fence)
    {
        if (!fence.IsFenceDestroyed)
        {
            if (currentMoney >= GameManager.Instance.FenceRepairCost)
            {
                if (fence.RepairFence())
                {
                    currentMoney -= GameManager.Instance.FenceRepairCost;
                    UIManager.Instance.UpdateMoneyText(currentMoney);
                }
            }
        }
        else
        {
            Debug.Log("Fence is already destroyed!");
        }
    }

    public void PlayerTakeDamage()
    {
        currentHealth -= GameManager.Instance.PlayerDamageWhenHit;
        if (currentHealth <= 0)
        {
            isAlive = false;
            SceneManager.LoadScene("GameOver");
            GameManager.Instance.GameOver();

        }
        Debug.Log($"Player Health: {currentHealth}");
        UIManager.Instance.UpdateHealthText(currentHealth);
    }

    public void AddPlayerMoney()
    {
        currentMoney += GameManager.Instance.MoneyForZombieKill;
        Debug.Log($"Player Money: {currentMoney}");
        UIManager.Instance.UpdateMoneyText(currentMoney);
    }

    private void ActionShoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, weaponPoint.position, weaponPoint.rotation);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = weaponPoint.right * 10f;
        Destroy(bullet, 2f);
    }

}