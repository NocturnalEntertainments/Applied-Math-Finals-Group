using UnityEngine;

public class FireballManager : MonoBehaviour
{
    public static FireballManager Instance;

    [SerializeField] private GameObject fireballPrefab;          
    [SerializeField] private Material fireballProjectileMaterial;

    private Mesh cubeMesh;                                     

    private void Awake()
    {
        Instance = this;
    }


    //Spawns a fireball projectile at the given position and direction.
    public void SpawnFireballProjectile(Vector3 startPos, Vector3 direction)
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("Fireball prefab not assigned!");
            return;
        }

        // Assign cubeMesh if not yet assigned
        if (cubeMesh == null && EnhanceMeshGenerator.Instance != null)
        {
            cubeMesh = EnhanceMeshGenerator.Instance.GetCubeMesh();
        }

        // Instantiate the fireball prefab
        GameObject obj = Instantiate(fireballPrefab, startPos, Quaternion.identity);

        // Initialize the projectile component
        FireballProjectile projectile = obj.GetComponent<FireballProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(startPos, direction, cubeMesh, fireballProjectileMaterial);
        }
    }
}
