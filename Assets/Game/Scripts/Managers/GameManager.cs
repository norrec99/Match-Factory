using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private EGameState currentGameState;

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
    }

    void Start()
    {
        SetGameState(EGameState.Menu);
    }

    public void SetGameState(EGameState newState)
    {
        currentGameState = newState;

        IEnumerable<IGameStateListener> gameStateListeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGameStateListener>();

        foreach (var listener in gameStateListeners)
        {
            listener.OnGameStateChanged(newState);
        }
    }

    public void StartGame()
    {
        SetGameState(EGameState.Game);
    }

    public void RetryButtonCallback()
    {
        SceneManager.LoadScene(0);
    }

    public void NextButtonCallback()
    {
        SceneManager.LoadScene(0);
    }

    public void CompleteLevel()
    {
        SetGameState(EGameState.LevelComplete);
    }

    public void GameOver()
    {
        SetGameState(EGameState.GameOver);
    }

    public bool IsGame()
    {
        return currentGameState == EGameState.Game;
    }
}
