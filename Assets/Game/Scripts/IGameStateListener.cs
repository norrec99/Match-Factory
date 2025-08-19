public interface IGameStateListener
{
    /// <summary>
    /// Called when the game state changes.
    /// </summary>
    /// <param name="newState">The new game state.</param>
    void OnGameStateChanged(EGameState newState);
}
