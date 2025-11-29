using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject extraLifePrefab;
    [SerializeField] private GameObject invincibilityPrefab;
    [SerializeField] private GameObject fireballPrefab;

    private List<GameObject> extraLives = new();
    private List<GameObject> invincibilities = new();
    private List<GameObject> fireballs = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Clean up inactive ExtraLives
        extraLives.RemoveAll(e => e == null);
        invincibilities.RemoveAll(i => i == null);
        fireballs.RemoveAll(f => f == null);
    }

    #region Spawn Methods
    public void SpawnExtraLife(Vector3 position)
    {
        if (extraLifePrefab == null)
        {
            Debug.LogWarning("ExtraLife prefab is null");
            return;
        }

        GameObject obj = Instantiate(extraLifePrefab, position, Quaternion.identity);
        extraLives.Add(obj);
    }

    public void SpawnInvincibility(Vector3 position)
    {
        if (invincibilityPrefab == null)
        {
            Debug.LogWarning("Invincibility prefab is null");
            return;
        }

        GameObject obj = Instantiate(invincibilityPrefab, position, Quaternion.identity);
        invincibilities.Add(obj);
    }

    public void SpawnFireball(Vector3 position)
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("Fireball prefab is null");
            return;
        }

        GameObject obj = Instantiate(fireballPrefab, position, Quaternion.identity);
        fireballs.Add(obj);
    }
    #endregion
}
