using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Lives")]
    public int playerLives = 3;

    [Header("Ability Durations")]
    public float invincibilityDuration = 5f;
    public float fireballDuration = 5f;

    [Header("Ability States")]
    public bool IsPInvincible = false;
    public bool pHasFireball = false;

    [Header("Timer")]
    public float levelTime = 60f;
    public float currentTime = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentTime = levelTime;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        UpdateTimer();
        UpdateAbilityTimers();
    }

    public void DamagePlayer(int amount)
    {
        playerLives -= amount;

        if (playerLives <= 0)
        {
            playerLives = 0;
            GameOver();
        }
    }

    #region Game Methods
    private void GameOver()
    {
        UIManager.Instance.ShowGameOver();
        Time.timeScale = 0f;
    }

    public void GameWin()
    {
        UIManager.Instance.ShowGameWin();
        Time.timeScale = 0f;
    }

    private void UpdateTimer()
    {
        if (Time.timeScale == 0f) return;

        if (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime = 0f;
            GameOver();
        }
    }

    public void RestartGame()
    {
        CollisionManager.Instance.ClearAllColliders();
        Time.timeScale = 1f; // resume normal time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region PowerUp Methods

    private void UpdateAbilityTimers()
    {
        //INVINCIBILITY
        if (IsPInvincible)
        {
            invincibilityDuration -= Time.deltaTime;
            if (invincibilityDuration <= 0f)
            {
                IsPInvincible = false;
                invincibilityDuration = 0f;
            }
        }

        //FIREBALL
        if (pHasFireball)
        {
            fireballDuration -= Time.deltaTime;
            if (fireballDuration <= 0f)
            {
                pHasFireball = false;
                fireballDuration = 0f;
            }
        }
    }

    public void AddLife(int amount)
    {
        playerLives += amount;

        // clamp max lives
        if (playerLives > 10)
            playerLives = 10;
    }

    public void ToggleInvincibility()
    {
        IsPInvincible = !IsPInvincible;
    }

    public void ToggleFireball()
    {
        pHasFireball = !pHasFireball;
    }
    #endregion
}
