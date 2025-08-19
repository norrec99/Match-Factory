using System;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour, IGameStateListener
{
    [SerializeField] private TMP_Text timerText;

    private float currentTimer;
    private float freezeTimer;

    private bool isTimerActive;
    private bool isFreezeTimerActive;

    public static TimerManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        ListenEvents();
    }

    private void ListenEvents()
    {
        LevelManager.OnLevelSpawned += OnLevelSpawned;
    }

    private void OnLevelSpawned(Level level)
    {
        currentTimer = level.LevelDuration;
        isTimerActive = true;
    }

    public void OnGameStateChanged(EGameState newState)
    {
        if (newState == EGameState.LevelComplete || newState == EGameState.GameOver)
        {
            isTimerActive = false;
        }
    }

    private void Update()
    {
        if (isFreezeTimerActive)
        {
            if (freezeTimer > 0f)
            {
                freezeTimer -= Time.deltaTime;
                if (freezeTimer < 0f)
                {
                    freezeTimer = 0f;
                    isTimerActive = true;
                    isFreezeTimerActive = false;
                }
            }
        }
        if (!isTimerActive)
            {
                return;
            }
        if (currentTimer > 0f)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer < 0f)
            {
                currentTimer = 0f;
            }
            UpdateTimerUI(currentTimer);
        }
        else
        {
            isTimerActive = false;
            OnTimerCompleted();
        }
    }

    private void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnTimerCompleted()
    {
        GameManager.Instance.GameOver();
    }

    public void FreezeTimer(float freezeDuration)
    {
        isTimerActive = false;
        isFreezeTimerActive = true;

        freezeTimer = freezeDuration;
    }

    private void UnsubscribeEvents()
    {
        LevelManager.OnLevelSpawned -= OnLevelSpawned;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
