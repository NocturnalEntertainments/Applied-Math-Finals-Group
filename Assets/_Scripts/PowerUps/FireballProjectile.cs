using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{

    [Header("Configurations")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float cullDistance = 250f;
    [SerializeField] private Vector3 scale = Vector3.one * 0.5f;

    private Mesh mesh;
    private Material material;
    private Vector3 direction;
    private int colliderID;
    private bool active = true;

    // Initialize projectile
    public void Initialize(Vector3 startPos, Vector3 dir, Mesh mesh, Material mat)
    {
        transform.position = startPos;
        direction = dir.normalized;
        this.mesh = mesh;
        this.material = mat;

        // Register collider
        colliderID = CollisionManager.Instance.RegisterCollider(transform.position, scale, false);
    }

    private void Update()
    {
        if (!active) return;

        Move();
        CheckCollision();
        RenderMesh();
    }

    private void Move()
    {
        transform.position += direction * speed * Time.deltaTime;
        CollisionManager.Instance.UpdateCollider(colliderID, transform.position, scale);
    }

    private void CheckCollision()
    {
        if (CollisionManager.Instance.CheckCollision(colliderID, transform.position, out List<int> hitIDs))
        {
            active = false;
            CollisionManager.Instance.RemoveCollider(colliderID);

            foreach (int id in hitIDs)
            {
                EnemyController enemy = FindEnemyByColliderID(id);
                if (enemy != null)
                {
                    enemy.DestroyEnemy();
                }
            }

            Destroy(gameObject);
        }
    }

    private void RenderMesh()
    {
        if (!active || mesh == null || material == null) return;

        Vector3 drawScale = scale;

        // --- Camera culling ---
        if (Camera.main != null)
        {
            Vector3 dirToObj = transform.position - Camera.main.transform.position;
            float dot = Vector3.Dot(Camera.main.transform.forward, dirToObj.normalized);
            float distance = dirToObj.magnitude;
            if (dot <= 0f || distance > cullDistance)
                drawScale = Vector3.zero;
        }

        if (drawScale != Vector3.zero)
        {
            Graphics.DrawMesh(mesh, Matrix4x4.TRS(transform.position, Quaternion.identity, drawScale), material, 0);
        }
    }

    private EnemyController FindEnemyByColliderID(int id)
    {
        EnemyController[] enemies = GameObject.FindObjectsOfType<EnemyController>();
        foreach (var e in enemies)
        {
            if (e.GetColliderID() == id)
                return e;
        }
        return null;
    }

}
