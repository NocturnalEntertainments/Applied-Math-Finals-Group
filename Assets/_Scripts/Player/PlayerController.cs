using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;           // horizontal speed
    [SerializeField] private float jumpForce = 10f;      // initial jump velocity
    [SerializeField] private float gravity = 20f;        // gravity strength

    [Header("Fireball Settings")]
    [SerializeField] private Vector3 fireballOffset = new Vector3(0f, 1f, 0f);

    [SerializeField] private Vector3 colliderSize = new Vector3(1, 1, 1);

    private int lastHorizontalDir = 1;
    private int colliderID;
    private Vector3 velocity;
    private bool grounded;

    private PlayerGraphics gfx;
    private PlayerCameraFollow cameraFollow;

    private float coyoteTime = 0.1f;
    private float coyoteTimer = 0f;

    private Color originalColor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //Get the essential components here
        gfx = GetComponent<PlayerGraphics>();

        if (gfx != null && gfx.playerMaterial != null)
            originalColor = gfx.playerMaterial.color;

        //Setup collider correctly from the collision manager
        colliderID = CollisionManager.Instance.RegisterCollider(
            transform.position,
            colliderSize,
            true
        );

        // Find the main camera and get the PlayerCameraFollow component
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            cameraFollow = mainCam.GetComponent<PlayerCameraFollow>();
        }
    }

    private void Update()
    {
        Move();
        UpdateGraphics();
        UpdateCamera();
        ShootFireball();
        UpdateEffects(); // Updates player material color based on current effect
    }

    private void OnDisable()
    {
        if (gfx != null && gfx.playerMaterial != null)
            gfx.playerMaterial.color = originalColor;
    }


    private void Move()
    {
        Vector3 pos = transform.position;

        // --- Update coyote timer ---
        if (grounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        //Jump Input
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        if (jumpPressed && coyoteTimer > 0f)
        {
            velocity.y = jumpForce;
            grounded = false;
            coyoteTimer = 0f; // prevent double jump during coyote
        }

        //Horizontal Movement
        float h = 0;
        if (Input.GetKey(KeyCode.A)) h -= 1;
        if (Input.GetKey(KeyCode.D)) h += 1;
        if (h != 0) lastHorizontalDir = (int)Mathf.Sign(h);

        float currentSpeed = grounded ? speed : speed * 0.5f;
        Vector3 tryPos = pos;
        tryPos.x += h * currentSpeed * Time.deltaTime;

        if (!CollisionManager.Instance.CheckCollision(colliderID, new Vector3(tryPos.x, pos.y, pos.z), out _))
            pos.x = tryPos.x;

        //Apply Gravity
        if (!grounded)
        {
            float gravityScale = velocity.y > 0 ? gravity * 0.6f : gravity;
            velocity.y -= gravityScale * Time.deltaTime;
        }

        //Vertical Movement
        tryPos = pos;
        tryPos.y += velocity.y * Time.deltaTime;

        if (CollisionManager.Instance.CheckCollision(colliderID, new Vector3(pos.x, tryPos.y, pos.z), out _))
        {
            if (velocity.y < 0) grounded = true;
            velocity.y = 0;
        }
        else
        {
            pos.y = tryPos.y;
            grounded = false;
        }

        //Apply final position and update collider
        transform.position = pos;
        CollisionManager.Instance.UpdateCollider(colliderID, pos, colliderSize);
    }


    #region PowerUP Methods
    private void ShootFireball()
    {
        if (!GameManager.Instance.pHasFireball) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 fireDir = new Vector3(lastHorizontalDir, 0, 0);
            Vector3 spawnPos = transform.position +
                new Vector3(fireballOffset.x * lastHorizontalDir, fireballOffset.y, fireballOffset.z);

            FireballManager.Instance.SpawnFireballProjectile(spawnPos, fireDir);
        }
    }

    private void UpdateEffects()
    {
        if (!gfx || !gfx.playerMaterial) return;

        Color col = originalColor;

        if (GameManager.Instance.IsPInvincible)
        {
            float t = Time.time * 5f;
            col = Color.HSVToRGB(t % 1f, 1f, 1f);
        }
        else if (GameManager.Instance.pHasFireball)
        {
            float t = Time.time * 5f;
            col = (Mathf.FloorToInt(t) % 2 == 0)
                ? Color.yellow
                : new Color(1f, 0.65f, 0f);
        }

        gfx.playerMaterial.color = col;
    }

    #endregion

    private void UpdateGraphics()
    {
        gfx.UpdateMatrix(transform.position, Quaternion.identity);
    }

    private void UpdateCamera()
    {
        if (cameraFollow != null)
        {
            cameraFollow.SetPlayerPosition(transform.position);
        }
    }

    public int GetColliderID()
    {
        return colliderID;
    }


    private void OnDrawGizmos()
    {
        if (colliderSize == null) return;

        // Draw collider wire cube
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, colliderSize);

#if UNITY_EDITOR
        // Draw label above the player
        UnityEditor.Handles.color = Color.white;
        Vector3 labelPos = transform.position + Vector3.up * (colliderSize.y * 0.5f + 0.2f);
        UnityEditor.Handles.Label(labelPos, "Player");
#endif
    }
}
