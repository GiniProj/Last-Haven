using UnityEngine;

public class Fence : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D fenceCollider;
    [SerializeField] private Rigidbody2D rb;

    private float maxFenceHealth;
    private float damageWhenHit;
    private float healthToAddWhenRepair;
    private float currentFenceHealth;
    private bool isFenceDestroyed = false;

    public bool IsFenceDestroyed => isFenceDestroyed;

    private void Awake()
    {
        // Setup components if not assigned
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (!fenceCollider) fenceCollider = GetComponent<BoxCollider2D>();
        if (!rb) rb = GetComponent<Rigidbody2D>();

        // Configure Rigidbody2D
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.useFullKinematicContacts = true;

        // Configure collider
        fenceCollider.isTrigger = true;
    }

    private void Start()
    {
        maxFenceHealth = GameManager.Instance.FenceMaxHealth;
        damageWhenHit = GameManager.Instance.FenceDamageWhenHit;
        healthToAddWhenRepair = GameManager.Instance.FenceHealthToAddWhenRepair;
        currentFenceHealth = maxFenceHealth;
    }


    public void RepairOrBuildFence()
    {
        if (isFenceDestroyed)
        {
            BuildFence();
        }
        else
        {
            RepairFence();
        }
    }

    private void BuildFence()
    {
        currentFenceHealth = maxFenceHealth;
        isFenceDestroyed = false;
        // rb.simulated = true;
        fenceCollider.enabled = true;
        Debug.Log("Fence is built!");
        ResetVisuals();
    }

    private void RepairFence()
    {
        if (currentFenceHealth >= maxFenceHealth) return;

        currentFenceHealth = Mathf.Min(currentFenceHealth + healthToAddWhenRepair, maxFenceHealth);
        Debug.Log("Fence is repaired!");
        IncreaseVisuals();
    }

    public void FenceTakeDamage()
    {
        if (isFenceDestroyed) return;

        currentFenceHealth -= damageWhenHit;
        Debug.Log($"Fence Health: {currentFenceHealth}");

        if (currentFenceHealth <= 0)
        {
            currentFenceHealth = 0;
            isFenceDestroyed = true;
            rb.simulated = false;
            fenceCollider.enabled = false;
            Debug.Log("Fence is destroyed!");
        }

        DecreaseVisuals();
    }

    private void IncreaseVisuals()
    {
        if (spriteRenderer && spriteRenderer.color.a < 1f)
        {
            Color newColor = spriteRenderer.color;
            newColor.a += 0.1f;
            spriteRenderer.color = newColor;
        }
    }

    private void DecreaseVisuals()
    {
        if (spriteRenderer)
        {
            Color newColor = spriteRenderer.color;
            if (isFenceDestroyed)
            {
                newColor.a = 0.3f;

            }
            else
            {
                newColor.a -= 0.05f;
            }
            spriteRenderer.color = newColor;
        }
    }

    private void ResetVisuals()
    {
        if (spriteRenderer)
        {
            Color newColor = spriteRenderer.color;
            newColor.a = 1f;
            spriteRenderer.color = newColor;
        }
    }
}