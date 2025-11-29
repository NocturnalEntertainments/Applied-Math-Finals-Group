using UnityEngine;

public class EnemyGraphics : MonoBehaviour
{
    [Header("Graphics Reference")]
    [SerializeField] private Material enemyMaterial;

    [Header("Culling Settings")]
    [SerializeField] private float cullDistance = 250f;

    [Header("Scale Settings")]
    [SerializeField] public Vector3 scale = new Vector3(1f, 1f, 1f);

    private Mesh cubeMesh;
    private Matrix4x4 matrix = Matrix4x4.identity;

    private void Start()
    {
        cubeMesh = EnhanceMeshGenerator.Instance.GetCubeMesh();
    }

    public void UpdateMatrix(Vector3 position, Quaternion rotation)
    {
        Vector3 currentScale = scale;

        // --- Camera culling ---
        if (Camera.main != null)
        {
            Vector3 dirToObj = transform.position - Camera.main.transform.position;
            float dot = Vector3.Dot(Camera.main.transform.forward, dirToObj.normalized);
            float distance = dirToObj.magnitude;

            // Instantly hide if behind camera or too far
            if (dot <= 0f || distance > cullDistance)
                currentScale = Vector3.zero;
        }

        matrix = Matrix4x4.TRS(position, rotation, currentScale);
    }

    private void LateUpdate()
    {
        if (cubeMesh == null || matrix.lossyScale == Vector3.zero) return;

        Graphics.DrawMesh(
            cubeMesh,
            matrix,
            enemyMaterial,
            0
        );
    }
}
