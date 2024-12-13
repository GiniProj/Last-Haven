using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    [Tooltip("The knockback force for any damage dealt to the fence")]
    [SerializeField] private float knockbackForce = 2f;
    [Tooltip("The duration of the knockback effect")]
    [SerializeField] private float knockbackDuration = 0.5f;

    private Transform currentTarget;  // The current target for the zombie to move towards
    private Rigidbody2D rb;  // Reference to the Rigidbody2D component
    private Vector2 blockedDirection = Vector2.zero;  // The direction that the zombie is blocked by the fence
    private Fence currentFence;  // The current fence that the zombie is colliding with
    private bool isKnockedBack = false;  // Whether the zombie is currently knocked back
    private bool isCollidingWithFence = false;  // Whether the zombie is currently colliding with a fence
    private float knockbackEndTime;  // The time when the knockback effect ends
    private float currentHealth;        // The current health of the zombie
    private float currentMoveSpeed;   // The current movement speed of the zombie
    private float damageWhenHit;  // The damage dealt to the zombie when hit


    public float ZombieSetHealth { set { currentHealth = value; } }  // for spawner modification option
    public float ZombieSetMoveSpeed { set { currentMoveSpeed = value; } }  //  for spawner modification option

    void Start()
    {
        currentHealth = GameManager.Instance.ZombieStartingHealth;
        currentMoveSpeed = GameManager.Instance.ZombieMovementSpeed;
        currentTarget = Player.Instance.transform;
        damageWhenHit = GameManager.Instance.ZombieDamageWhenHit;

        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void FixedUpdate()
    {
        if (Time.time >= knockbackEndTime)
        {
            isKnockedBack = false;
        }

        if (!isKnockedBack)
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;

            // Check for fence collision and adjust movement
            if (isCollidingWithFence && currentFence != null && !currentFence.IsFenceDestroyed)
            {
                float dotProduct = Vector2.Dot(direction, blockedDirection);
                if (dotProduct > 0)
                {
                    // Remove the blocked component from movement
                    direction -= blockedDirection * dotProduct;
                    direction = direction.normalized;
                }
            }

            // Apply movement
            Vector2 newPosition = rb.position + direction * currentMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            // Update rotation
            if (direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rb.MoveRotation(angle);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Fence"))
        {
            Fence fence = other.gameObject.GetComponent<Fence>();
            if (fence != null)
            {
                if (!fence.IsFenceDestroyed)
                {
                    fence.FenceTakeDamage();
                    isCollidingWithFence = true;
                    currentFence = fence;
                    blockedDirection = (transform.position - other.transform.position).normalized;

                    // Apply knockback only for intact fences
                    Vector2 knockbackDirection = blockedDirection;
                    ApplyKnockback(knockbackDirection);
                }
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            if (Player.Instance != null)
            {
                Player.Instance.PlayerTakeDamage();
                Vector2 knockbackDirection = (transform.position - other.transform.position).normalized;
                ApplyKnockback(knockbackDirection);
            }
        }
        else if (other.gameObject.CompareTag("Bullet"))
        {
            ZombieTakeDamage();
            Destroy(other.gameObject);
            Vector2 knockbackDirection = (transform.position - other.transform.position).normalized;
            ApplyKnockback(knockbackDirection);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Fence"))
        {
            isCollidingWithFence = false;
            currentFence = null;
            blockedDirection = Vector2.zero;
        }
    }

    public void ApplyKnockback(Vector2 knockbackDirection)
    {
        isKnockedBack = true;
        knockbackEndTime = Time.time + knockbackDuration;
        StartCoroutine(PerformKnockback(knockbackDirection));
    }

    private IEnumerator PerformKnockback(Vector2 knockbackDirection)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = rb.position;
        // Calculate target position based on knockback force
        Vector2 targetPosition = startPosition + (knockbackDirection * knockbackForce);

        while (elapsedTime < knockbackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / knockbackDuration;

            // Smooth easing for more natural knockback
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

            Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, smoothT);
            rb.MovePosition(newPosition);

            yield return null;
        }
    }

    public void ZombieTakeDamage()
    {
        currentHealth -= damageWhenHit;
        if (currentHealth <= 0)
        {
            GameManager.Instance.AddZombieDefeated();
            Destroy(gameObject);
        }
    }
}