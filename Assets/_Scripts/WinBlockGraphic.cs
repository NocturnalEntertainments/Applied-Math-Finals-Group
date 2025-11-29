using UnityEngine;

public class WinBlockGraphic : MonoBehaviour
{
    [Header("Graphics Reference")]
    [SerializeField] private Material winBlockMaterial;

    [Header("Size Configurations")]
    [SerializeField] private float width = 2f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float depth = 2f;

    [Header("Culling Settings")]
    [SerializeField] private float cullDistance = 250f;

    private Matrix4x4 blockMatrix;
    private int colliderID;
    private Vector3 originalScale;

    private float triggerMultiplier = 1.1f;


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

        // Draw mesh only if in range
        if (EnhanceMeshGenerator.Instance != null && scale != Vector3.zero)
        {
            Graphics.DrawMesh(
                EnhanceMeshGenerator.Instance.GetCubeMesh(),
                blockMatrix,
                winBlockMaterial,
                0
            );
        }

        // Only update collision if in range
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

        if (triggerBounds.Intersects(playerBounds))
            GameManager.Instance.GameWin();
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, originalScale);
    }
}
