using System;
using UnityEngine;

public class LevelManager : MonoBehaviour, IGameStateListener
{
    [Header("Level Settings")]
    [SerializeField] private Level[] levels;

    private int levelIndex = 0;

    private Level currentLevel;
    public Item[] Items => currentLevel.GetItems();

    private const string LEVEL_KEY = "CurrentLevel";

    public static Action<Level> OnLevelSpawned;

    public static LevelManager Instance;

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
        LoadData();
    }

    void Start()
    {
    }

    private void SpawnLevel()
    {
        transform.Clear();

        int validatedLevelIndex = levelIndex % levels.Length;

        currentLevel = Instantiate(levels[validatedLevelIndex], transform);

        OnLevelSpawned?.Invoke(currentLevel);
    }

    private void LoadData()
    {
        levelIndex = PlayerPrefs.GetInt(LEVEL_KEY);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(LEVEL_KEY, levelIndex);
    }

    public void OnGameStateChanged(EGameState newState)
    {
        if (newState == EGameState.Game)
        {
            SpawnLevel();
        }
        else if (newState == EGameState.LevelComplete)
        {
            levelIndex++;
            SaveData();
        }
    }
}
