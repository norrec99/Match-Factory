using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private EGameState currentGameState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
}
