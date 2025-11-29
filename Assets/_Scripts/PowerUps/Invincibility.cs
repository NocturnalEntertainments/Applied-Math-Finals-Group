using UnityEngine;

public class Invincibility : MonoBehaviour
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
    private bool active = true;
    private Color originalColor;

    private void Start()
    {
        // Get mesh from the mesh generator
        if (EnhanceMeshGenerator.Instance != null)
            mesh = EnhanceMeshGenerator.Instance.GetCubeMesh();

        // Store original color
        if (material != null)
            originalColor = material.color;

        // Setup collider
        colliderID = CollisionManager.Instance.RegisterCollider(transform.position, colliderSize, false);
    }

    private void Update()
    {
        if (!active) return;

        // Update collider
        CollisionManager.Instance.UpdateCollider(colliderID, transform.position, colliderSize);

        CheckPlayerCollision();
        RenderMesh();
    }

    private void CheckPlayerCollision()
    {
        PlayerController player = PlayerController.Instance;
        if (player == null) return;

        AABBBounds playerBounds = CollisionManager.Instance.GetCollider(player.GetColliderID());
        if (playerBounds == null) return;

        // Trigger-style bounds
        AABBBounds triggerBounds = new AABBBounds(transform.position, colliderSize * triggerMultiplier, -1);

        if (triggerBounds.Intersects(playerBounds))
        {
            Collect();
        }
    }

    private void Collect()
    {
        GameManager.Instance.ToggleInvincibility();

        active = false;
        CollisionManager.Instance.RemoveCollider(colliderID);

        Destroy(gameObject);
    }

    private void RenderMesh()
    {
        if (!active || mesh == null || material == null) return;

        Vector3 scale = visualScale;

        // Camera culling
        if (Camera.main != null)
        {
            Vector3 dirToObj = transform.position - Camera.main.transform.position;
            float dot = Vector3.Dot(Camera.main.transform.forward, dirToObj.normalized);
            float distance = dirToObj.magnitude;
            if (dot <= 0f || distance > cullDistance)
                scale = Vector3.zero;
        }

        // Rainbow effect
        float t = Time.time * 5f;
        material.color = Color.HSVToRGB(t % 1f, 1f, 1f);

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

    // Reset color when disabled or stopped playing
    private void OnDisable()
    {
        if (material != null)
            material.color = originalColor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, colliderSize);
    }
}
