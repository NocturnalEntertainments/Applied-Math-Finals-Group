using UnityEngine;

public class PlayerGraphics : MonoBehaviour
{
    public Material playerMaterial;//is used by PlayerController.cs

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
        matrix = Matrix4x4.TRS(position, rotation, scale);
    }

    private void LateUpdate()
    {
        Graphics.DrawMesh(
            cubeMesh,
            matrix,
            playerMaterial,
            0
        );
    }
}
