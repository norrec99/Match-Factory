using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private Level[] levels;

    private int levelIndex = 0;

    private Level currentLevel;

    private const string LEVEL_KEY = "CurrentLevel";

    public static Action<Level> OnLevelSpawned;

    private void Awake()
    {
        LoadData();
    }

    void Start()
    {
        SpawnLevel();
    }

    private void SpawnLevel()
    {
        transform.Clear();

        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }

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
}
