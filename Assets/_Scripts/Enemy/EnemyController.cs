using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Main Configurations")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float moveRange = 3f;
    [SerializeField] private Vector3 colliderSize = new Vector3(1, 1, 1);


    [Header("Hit Settings")]
    [SerializeField] private float hitCooldown = 1f;

    private int colliderID;
    private Vector3 startPos;
    private bool movingRight = true;

    private EnemyGraphics gfx;

    // Trigger size multiplier to simulate isTrigger
    private float triggerMultiplier = 1.2f;
    private float hitTimer = 0f;

    private void Start()
    {
        gfx = GetComponent<EnemyGraphics>();
        startPos = transform.position;

        // Register collider
        colliderID = CollisionManager.Instance.RegisterCollider(transform.position, colliderSize, false);
    }

    private void Update()
    {
        Move();
        UpdateGraphics();
        CheckPlayerCollision();

        // countdown the hit timer
        if (hitTimer > 0f)
            hitTimer -= Time.deltaTime;
    }

    private void Move()
    {
        Vector3 pos = transform.position;
        float dir = movingRight ? 1f : -1f;
        Vector3 tryPos = pos + new Vector3(dir * speed * Time.deltaTime, 0, 0);

        // collision test at new X
        if (!CollisionManager.Instance.CheckCollision(colliderID, new Vector3(tryPos.x, pos.y, pos.z), out _))
            pos.x = tryPos.x;
        else
            movingRight = !movingRight; // reverse if blocked

        // reverse if out of range
        if (Mathf.Abs(pos.x - startPos.x) > moveRange)
            movingRight = !movingRight;

        transform.position = pos;

        CollisionManager.Instance.UpdateCollider(colliderID, pos, colliderSize);
    }

    private void UpdateGraphics()
    {
        gfx.UpdateMatrix(transform.position, Quaternion.identity);
    }

    private void CheckPlayerCollision()
    {
        if (hitTimer > 0f) return; // still on cooldown

        PlayerController player = PlayerController.Instance;
        if (player == null) return;

        AABBBounds playerBounds = CollisionManager.Instance.GetCollider(player.GetColliderID());
        if (playerBounds == null) return;

        Vector3 triggerSize = colliderSize * triggerMultiplier;
        AABBBounds enemyTrigger = new AABBBounds(transform.position, triggerSize, -1);

        if (enemyTrigger.Intersects(playerBounds))
        {
            if (GameManager.Instance.IsPInvincible)
            {
                DestroyEnemy();
            }
            else
            {
                GameManager.Instance.DamagePlayer(1);
                hitTimer = hitCooldown; // reset cooldown
                Debug.Log("Player Damaged");
            }
        }
    }

    // Called by fireball when hit
    public void DestroyEnemy()
    {
        CollisionManager.Instance.RemoveCollider(colliderID);
        Destroy(gameObject);
    }

    public int GetColliderID()
    {
        return colliderID;
    }

    private void OnDrawGizmos()
    {
        // Draw the actual collider
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, colliderSize);

        // Draw the trigger area
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireCube(transform.position, colliderSize * triggerMultiplier);
    }
}
