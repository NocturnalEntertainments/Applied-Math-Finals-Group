using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    [Header("Graphics Reference")]
    [SerializeField] private Material material;

    [Header("Configurations")]
    [SerializeField] private Vector3 visualScale = Vector3.one * 0.7f;
    [SerializeField] private Vector3 colliderSize = Vector3.one;
    [SerializeField] private float triggerMultiplier = 1.5f;
    [SerializeField] private float cullDistance = 250f;

    private Mesh mesh;
    private int colliderID;

    private void Start()
    {
        // Get mesh from the mesh generator
        if (EnhanceMeshGenerator.Instance != null)
            mesh = EnhanceMeshGenerator.Instance.GetCubeMesh();

        // Register collider
        colliderID = CollisionManager.Instance.RegisterCollider(transform.position, colliderSize, false);
    }

    private void Update()
    {
        if (mesh == null || material == null) return;

        // Update collider
        CollisionManager.Instance.UpdateCollider(colliderID, transform.position, colliderSize);

        // Check player collision
        PlayerController player = PlayerController.Instance;
        if (player != null)
        {
            var playerBounds = CollisionManager.Instance.GetCollider(player.GetColliderID());
            if (playerBounds != null)
            {
                var triggerBounds = new AABBBounds(transform.position, colliderSize * triggerMultiplier, -1);
                if (triggerBounds.Intersects(playerBounds))
                {
                    GameManager.Instance.AddLife(1);
                    CollisionManager.Instance.RemoveCollider(colliderID);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        RenderMesh();
    }

    private void RenderMesh()
    {
        Vector3 scale = visualScale;

        if (Camera.main != null)
        {
            // Cull mesh if outside camera range
            Vector3 dirToObj = transform.position - Camera.main.transform.position;
            float dot = Vector3.Dot(Camera.main.transform.forward, dirToObj.normalized);
            float distance = dirToObj.magnitude;
            if (dot <= 0f || distance > cullDistance)
                scale = Vector3.zero;
        }

        if (scale != Vector3.zero)
        {
            Graphics.DrawMesh(
                mesh,
                Matrix4x4.TRS(transform.position, Quaternion.identity, scale),
                material,
                0
            );
        }
    }

    private void OnDrawGizmos()
    {
        // Always draw the collider bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, colliderSize);
    }
}
