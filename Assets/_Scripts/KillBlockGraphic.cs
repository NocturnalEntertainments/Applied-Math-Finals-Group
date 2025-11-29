using UnityEngine;

public class KillBlockGraphic : MonoBehaviour
{
    [Header("Graphics Reference")]
    [SerializeField] private Material killBlockMaterial;

    [Header("Size Configurations")]
    [SerializeField] private float width = 2f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float depth = 2f;

    [Header("Culling Settings")]
    [SerializeField] private float cullDistance = 250f;

    [Header("Kill State")]
    [SerializeField] private bool isInstaKill = false;

    private Matrix4x4 blockMatrix;
    private int colliderID;
    private Vector3 originalScale;

    private float triggerMultiplier = 1.1f;
    private float hitCooldown = 1f;
    private float lastHitTime = -10f;


    private void Start()
    {
        originalScale = new Vector3(width, height, depth);
        colliderID = CollisionManager.Instance.RegisterCollider(transform.position, originalScale, false);
        blockMatrix = Matrix4x4.TRS(transform.position, Quaternion.identity, originalScale);
        CollisionManager.Instance.UpdateMatrix(colliderID, blockMatrix);
    }

    private void Update()
    {
        bool inRange = IsInCameraRange();
        Vector3 scale = inRange ? originalScale : Vector3.zero;

        blockMatrix = Matrix4x4.TRS(transform.position, Quaternion.identity, scale);

        if (EnhanceMeshGenerator.Instance != null && scale != Vector3.zero)
        {
            Graphics.DrawMesh(
                EnhanceMeshGenerator.Instance.GetCubeMesh(),
                blockMatrix,
                killBlockMaterial,
                0
            );
        }

        if (inRange)
            CollisionManager.Instance.UpdateMatrix(colliderID, blockMatrix);

        CheckPlayerCollision();
    }

    private void CheckPlayerCollision()
    {
        PlayerController player = PlayerController.Instance;
        if (player == null) return;

        var playerBounds = CollisionManager.Instance.GetCollider(player.GetColliderID());
        if (playerBounds == null) return;

        var triggerBounds = new AABBBounds(transform.position, originalScale * triggerMultiplier, -1);

        if (!GameManager.Instance.IsPInvincible &&
            Time.time - lastHitTime >= hitCooldown &&
            triggerBounds.Intersects(playerBounds))
        {
            lastHitTime = Time.time;

            if (!isInstaKill)
                GameManager.Instance.DamagePlayer(1);
            else
                GameManager.Instance.DamagePlayer(99999);
        }
    }

    private bool IsInCameraRange()
    {
        if (Camera.main == null) return true;

        Vector3 dirToObj = transform.position - Camera.main.transform.position;
        float dot = Vector3.Dot(Camera.main.transform.forward, dirToObj.normalized);
        float distance = dirToObj.magnitude;

        return dot > 0f && distance <= cullDistance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, originalScale);
    }
}
