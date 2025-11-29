using UnityEngine;

public class GroundGraphics : MonoBehaviour
{
    [Header("Graphics Reference")]
    [SerializeField] private Material groundMaterial;

    [Header("Size Configurations")]
    [SerializeField] private float groundWidth = 200f;
    [SerializeField] private float groundDepth = 200f;
    [SerializeField] private float groundHeight = 1f;

    [Header("Culling Settings")]
    [SerializeField] private float cullDistance = 250f;

    private Matrix4x4 groundMatrix;
    private int groundColliderID;
    private Vector3 originalScale;


    private void Start()
    {
        originalScale = new Vector3(groundWidth, groundHeight, groundDepth);

        // Register collider at the object's center
        groundColliderID = CollisionManager.Instance.RegisterCollider(transform.position, originalScale, false);

        // Initialize mesh matrix
        groundMatrix = Matrix4x4.TRS(transform.position, Quaternion.identity, originalScale);

        CollisionManager.Instance.UpdateMatrix(groundColliderID, groundMatrix);
    }

    private void Update()
    {
        bool inRange = IsInCameraRange();

        // If in camera range, show mesh and update collision
        Vector3 scale = inRange ? originalScale : Vector3.zero;
        groundMatrix = Matrix4x4.TRS(transform.position, Quaternion.identity, scale);

        // Draw mesh only if scale is not zero
        if (EnhanceMeshGenerator.Instance != null && scale != Vector3.zero)
        {
            Graphics.DrawMesh(
                EnhanceMeshGenerator.Instance.GetCubeMesh(),
                groundMatrix,
                groundMaterial,
                0
            );
        }

        // Only update collision if in range
        if (inRange)
        {
            CollisionManager.Instance.UpdateMatrix(groundColliderID, groundMatrix);
        }
    }

    // Check if object is in front of camera and within culling distance
    private bool IsInCameraRange()
    {
        if (Camera.main == null) return true;

        Vector3 dirToObject = transform.position - Camera.main.transform.position;
        float dot = Vector3.Dot(Camera.main.transform.forward, dirToObject.normalized);

        float distance = dirToObject.magnitude;
        return dot > 0f && distance <= cullDistance;
    }

    private void OnDrawGizmos()
    {
        Vector3 size = new Vector3(groundWidth, groundHeight, groundDepth);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
