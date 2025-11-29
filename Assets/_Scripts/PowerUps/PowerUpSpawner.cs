using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public enum PowerUpType
    {
        ExtraLife,
        Invincibility,
        Fireball
    }

    [Header("Which power-up to spawn")]
    public PowerUpType typeToSpawn = PowerUpType.ExtraLife;

    private void Start()
    {
        SpawnPowerUp();
    }

    private void SpawnPowerUp()
    {
        Vector3 pos = transform.position;

        switch (typeToSpawn)
        {
            case PowerUpType.ExtraLife:
                PowerUpManager.Instance.SpawnExtraLife(pos);
                break;
            case PowerUpType.Invincibility:
                PowerUpManager.Instance.SpawnInvincibility(pos);
                break;
            case PowerUpType.Fireball:
                PowerUpManager.Instance.SpawnFireball(pos);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        switch (typeToSpawn)
        {
            case PowerUpType.ExtraLife:
                Gizmos.color = Color.green;
                break;
            case PowerUpType.Invincibility:
                Gizmos.color = Color.yellow;
                break;
            case PowerUpType.Fireball:
                Gizmos.color = Color.red;
                break;
        }

        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
