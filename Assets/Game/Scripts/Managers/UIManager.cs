using UnityEngine;

public class UIManager : MonoBehaviour, IGameStateListener
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject gameOverPanel;

    public void OnGameStateChanged(EGameState newState)
    {
        mainMenuPanel.SetActive(newState == EGameState.Menu);
        gamePanel.SetActive(newState == EGameState.Game);
        levelCompletePanel.SetActive(newState == EGameState.LevelComplete);
        gameOverPanel.SetActive(newState == EGameState.GameOver);
    }
}
