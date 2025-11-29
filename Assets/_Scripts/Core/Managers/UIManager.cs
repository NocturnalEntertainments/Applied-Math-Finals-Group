using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Lives UI")]
    [SerializeField] private GameObject lifeIconPrefab;
    [SerializeField] private Transform lifeContainer;

    [Header("Timer UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text invincibilityTimerText;
    [SerializeField] private TMP_Text fireballTimerText;


    [Header("Game UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameWinUI;


    private List<GameObject> lifeIcons = new();
    private bool stopTimer = false;

    private void Awake()
    {
        Instance = this;

        gameOverUI.SetActive(false);
        timerText.gameObject.SetActive(true);

        invincibilityTimerText.gameObject.SetActive(false);
        fireballTimerText.gameObject.SetActive(false);
    }


    private void Update()
    {
        UpdateLives(GameManager.Instance.playerLives);
        if (!stopTimer) UpdateTimerUI();
        UpdateAbilityTimers();
    }

    private void UpdateLives(int playerLives)
    {
        while (lifeIcons.Count < playerLives)
        {
            GameObject icon = Instantiate(lifeIconPrefab, lifeContainer);
            lifeIcons.Add(icon);
        }

        while (lifeIcons.Count > playerLives)
        {
            GameObject icon = lifeIcons[lifeIcons.Count - 1];
            lifeIcons.RemoveAt(lifeIcons.Count - 1);
            Destroy(icon);
        }
    }

    private void UpdateTimerUI()
    {
        float time = GameManager.Instance.currentTime;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"Timer: {minutes:00}:{seconds:00}";
    }

    private void UpdateAbilityTimers()
    {
        var gm = GameManager.Instance;

        //INVINCIBILITY TIMER
        if (gm.IsPInvincible)
        {
            invincibilityTimerText.gameObject.SetActive(true);
            invincibilityTimerText.text =
                $"Invincibility: {Mathf.CeilToInt(gm.invincibilityDuration):00}";
        }
        else invincibilityTimerText.gameObject.SetActive(false);

        //FIREBALL TIMER
        if (gm.pHasFireball)
        {
            fireballTimerText.gameObject.SetActive(true);
            fireballTimerText.text =
                $"Fireball: {Mathf.CeilToInt(gm.fireballDuration):00}";
        }
        else fireballTimerText.gameObject.SetActive(false);
    }

    #region UI Show/Hide Methods
    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
        stopTimer = true;               // stop updating the timer
        timerText.gameObject.SetActive(false); // hide timer
    }


    public void ShowGameWin()
    {
        gameWinUI.SetActive(true);
        stopTimer = true;
        timerText.gameObject.SetActive(false);
    }

    public void ShowInvincibilityTimer(float timeRemaining)
    {
        invincibilityTimerText.gameObject.SetActive(true);

        int seconds = Mathf.CeilToInt(timeRemaining);
        invincibilityTimerText.text = $"Invincibility: {seconds:00}";
    }

    public void HideInvincibilityTimer()
    {
        invincibilityTimerText.gameObject.SetActive(false);
    }

    #endregion

    //for gameover buttons
    public void InitiateRestartGame()
    {
        GameManager.Instance.RestartGame();
    }
}
