using System.Collections.Generic;
using UnityEngine;

//I seperated this from EnhancedMeshGenerator.cs
public class EnhanceMeshGenerator : MonoBehaviour
{
    public Material worldMaterial;
    public int instanceCount = 100;

    private Mesh cubeMesh;
    private List<Matrix4x4> worldMatrices = new();
    private List<int> colliderIds = new();

    public float width = 1f, height = 1f, depth = 1f;
    public float constantZPosition = 0f;

    public float minX = -50f, maxX = 50f;
    public float minY = -50f, maxY = 50f;

    public static EnhanceMeshGenerator Instance;

    void Awake()
    {
        Instance = this;
        CreateCubeMesh();
    }

    public Mesh GetCubeMesh() => cubeMesh;

    void Start()
    {
        CreateCubeMesh();
        //GenerateWorldBoxes();
    }

    void CreateCubeMesh()
    {
        cubeMesh = new Mesh();

        // Vertices from -0.5 to +0.5, pivot at center
        Vector3[] vertices = new Vector3[]
        {
        new(-0.5f,-0.5f,-0.5f), new(0.5f,-0.5f,-0.5f), new(0.5f,-0.5f,0.5f), new(-0.5f,-0.5f,0.5f),
        new(-0.5f,0.5f,-0.5f), new(0.5f,0.5f,-0.5f), new(0.5f,0.5f,0.5f), new(-0.5f,0.5f,0.5f)
        };

        int[] triangles = {
        0,4,1, 1,4,5,
        2,6,3, 3,6,7,
        0,3,4, 4,3,7,
        1,5,2, 2,5,6,
        0,1,3, 3,1,2,
        4,7,5, 5,7,6
    };

        Vector2[] uvs = new Vector2[]
        {
        new(0,0), new(1,0), new(1,1), new(0,1),
        new(0,0), new(1,0), new(1,1), new(0,1)
        };

        cubeMesh.vertices = vertices;
        cubeMesh.triangles = triangles;
        cubeMesh.uv = uvs;

        cubeMesh.RecalculateNormals();
    }



    void GenerateWorldBoxes()
    {
        for (int i = 0; i < instanceCount; i++)
        {
            Vector3 pos = new(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                constantZPosition
            );

            Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            Vector3 scale = new(
                Random.Range(0.5f, 3f),
                Random.Range(0.5f, 3f),
                Random.Range(0.5f, 3f)
            );

            int id = CollisionManager.Instance.RegisterCollider(pos, scale, false);

            Matrix4x4 m = Matrix4x4.TRS(pos, rot, scale);

            worldMatrices.Add(m);
            colliderIds.Add(id);

            CollisionManager.Instance.UpdateMatrix(id, m);
        }
    }

    void Update()
    {
        RenderWorld();
    }

    void RenderWorld()
    {
        if (worldMatrices.Count == 0) return;

        Matrix4x4[] mats = worldMatrices.ToArray();

        for (int i = 0; i < mats.Length; i += 1023)
        {
            int batch = Mathf.Min(1023, mats.Length - i);
            Matrix4x4[] copy = new Matrix4x4[batch];

            System.Array.Copy(mats, i, copy, 0, batch);

            Graphics.DrawMeshInstanced(
                cubeMesh,
                0,
                worldMaterial,
                copy,
                batch
            );
        }
    }
}
