using System;
using UnityEngine.SceneManagement;

namespace Main
{
    public enum GameState { Start, GameOver, LevelCompleted, RoomCleaned }

    public static class GameManager
    {
        public static Action OnGameStarted;
        public static Action OnGameOver;
        public static Action OnLevelCompleted;
        public static Action OnRoomCleaned;

        public static void ChangeGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Start:
                    OnGameStarted?.Invoke();
                    break;
                case GameState.GameOver:
                    OnGameOver?.Invoke();
                    break;
                case GameState.LevelCompleted:
                    OnLevelCompleted?.Invoke();
                    break;
                case GameState.RoomCleaned:
                    OnRoomCleaned?.Invoke();
                    break;
            }
        }

        public static void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


    }
}

