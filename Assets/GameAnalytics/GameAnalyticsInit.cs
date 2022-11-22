using UnityEngine;
using GameAnalyticsSDK;
using UnityEngine.SceneManagement;
using System;

namespace Main
{
    public class GameAnalyticsInit : MonoBehaviour,IGameAnalyticsATTListener
    {
        private void Awake()
        {

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GameAnalytics.RequestTrackingAuthorization(this);
            }
            else
            {
                GameAnalytics.Initialize();
            }
        }

        public void GameAnalyticsATTListenerNotDetermined()
        {
            GameAnalytics.Initialize();
        }

        public void GameAnalyticsATTListenerRestricted()
        {
            GameAnalytics.Initialize();
        }

        public void GameAnalyticsATTListenerDenied()
        {
            GameAnalytics.Initialize();
        }

        public void GameAnalyticsATTListenerAuthorized()
        {
            GameAnalytics.Initialize();
        }



        private void OnEnable()
        {
            GameManager.OnLevelCompleted += LevelCompleted;
            GameManager.OnGameStarted += GameStated;
            GameManager.OnGameOver += GameOver;
        }

        private void OnDisable()
        {
            GameManager.OnLevelCompleted -= LevelCompleted;
            GameManager.OnGameStarted -= GameStated;
            GameManager.OnGameOver -= GameOver;
        }
        private void GameStated()
        {
            SendLevelStartedEvent(SceneManager.GetActiveScene().buildIndex);
        }


        private void LevelCompleted()
        {
            SendLevelCompletedEvent(SceneManager.GetActiveScene().buildIndex);
        }

        private void GameOver()
        {
            SendLevelFailedEvent(SceneManager.GetActiveScene().buildIndex);
        }

        public void SendLevelStartedEvent(int level)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $"Level{level - 1}started!");
        }

        public void SendLevelRestartedEvent(int level)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $"Level{level - 1}restarted!");
        }

        public void SendLevelFailedEvent(int level)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $"Level{level - 1}failed!");
        }

        public void SendLevelCompletedEvent(int level)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"Level{level - 1}completed!");
        }

    }
}


