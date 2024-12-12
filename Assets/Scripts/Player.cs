using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance { get; set; }

    [Header("Player Action Settings")]
    [SerializeField] private Transform weaponPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootCooldown = 0.5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D playerCollider;

    private Fence currentNearbyFence;
    private float moveSpeed;
    private float currentHealth;
    private float currentMoney;
    private float lastShootTime;
    private bool isAlive = true;
    private ContactFilter2D contactFilter;
    private readonly RaycastHit2D[] hitResults = new RaycastHit2D[4];

    public Fence CurrentNearbyFence { get; set; }
    public float CurrentPlayerHealth => currentHealth;
    public bool IsPlayerAlive => isAlive;

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

        // Repair or Build Fence
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Fence"))
            {
                Fence fence = hit.collider.GetComponent<Fence>();
                if (fence != null)
                {
                    ActionRepairOrBuildFence(fence);
                }
            }
        }
    }

    private void ActionRepairOrBuildFence(Fence fence)
    {
        if (fence.IsFenceDestroyed)
        {
            if (currentMoney >= GameManager.Instance.FenceBuildNewCost)
            {
                fence.RepairOrBuildFence();
                currentMoney -= GameManager.Instance.FenceBuildNewCost;
                UIManager.Instance.UpdateMoneyText(currentMoney);
            }
            else
            {
                Debug.Log("Not enough money to build new fence!");
            }
        }
        else
        {
            if (currentMoney >= GameManager.Instance.FenceRepairCost)
            {
                fence.RepairOrBuildFence();
                currentMoney -= GameManager.Instance.FenceRepairCost;
                UIManager.Instance.UpdateMoneyText(currentMoney);
            }
            else
            {
                Debug.Log("Not enough money to repair fence!");
            }
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