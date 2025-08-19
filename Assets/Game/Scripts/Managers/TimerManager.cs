using System;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour, IGameStateListener
{
    [SerializeField] private TMP_Text timerText;

    private float currentTimer;
    private bool isTimerActive;

    private void Awake()
    {
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

    private void UnsubscribeEvents()
    {
        LevelManager.OnLevelSpawned -= OnLevelSpawned;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
