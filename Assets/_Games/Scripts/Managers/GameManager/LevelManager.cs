namespace PuzzleGames
{
	using System;
	using System.Collections;
	using com.ootii.Messages;
	using DG.Tweening;
	using BasePuzzle.FalconAnalytics.Scripts.Enum;
	using BasePuzzle.PuzzlePackages;
	using BasePuzzle.PuzzlePackages.Core;
	using UnityEngine;

	public class LevelManager : Singleton<LevelManager>
	{
		public InGameTracker tracker;

		public  int             currentLevelToLog;
		private string          currentLevelData;
		private LevelDifficulty currentLevelDifficulty;

		private bool _startGame;

		public void SetLevelData(int level,string levelData,LevelDifficulty levelDifficulty)
		{
			currentLevelData      =levelData;
			currentLevelToLog     =level;
			currentLevelDifficulty=levelDifficulty;
		}

		protected override void Awake() { base.Awake(); }

		private void Start()
		{
			Application.targetFrameRate=60;
			//AdsManager.Instance.ShowBanner();

			GameManager.OnGameStart +=StartLevel;
			GameManager.OnGameWon   +=CompleteLevel;
			GameManager.OnGameRevive+=ReviveLevel;
			GameManager.OnGameLost  +=LoseLevel;

			currentLevelData =TempDataHandler.Get<string>(TempDataKeys.CURRENT_LEVEL_JSON_DATA);
			currentLevelToLog=TempDataHandler.Get(TempDataKeys.CURRENT_LEVEL_FROM_HOME,0);

			/*if(currentLevelToLog==1)
			{
				DOVirtual.DelayedCall(2f,() => WindowManager.Instance.OpenWindow<TutorialPanel>());
			}*/

			StartCoroutine(CoSetLevelDiff());

			IEnumerator CoSetLevelDiff()
			{
				yield return null;

				MessageDispatcher.SendMessage(this,EventID.SET_LEVEL_DIFFICULTY,currentLevelDifficulty,0);
			}
		}

		protected override void OnDestroy()
		{
			GameManager.OnGameStart -=StartLevel;
			GameManager.OnGameWon   -=CompleteLevel;
			GameManager.OnGameRevive-=ReviveLevel;
			GameManager.OnGameLost  -=LoseLevel;
		}

		private void StartLevel()
		{
			if(!UserResourceController.instance.IsInfiHeart())
			{
				UserResourceController.instance.MinusHeart(1);
			}

			LevelDataController.instance.Play();
			tracker.StartTracking();
			//FirebaseLog.LogEventLevelStart(currentLevelToLog,UserResourceController.instance.UserResource.gold);
		}

		public void CompleteLevel()
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			bool fromEditor=TempDataHandler.Get(TempDataKeys.OPEN_LEVEL_FROM_EDITOR,false);

			if(fromEditor)
			{
				return;
			}
#endif

			tracker.StopTracking();

			if(currentLevelToLog>=LevelDataController.instance.Level)
			{
				LevelDataController.instance.LevelData.level=currentLevelToLog;
				LevelDataController.instance.CompleteLevel();
			}

			//AdsManager.Instance.HideBanner();
			WindowManager.Instance.OpenWindow<LevelCompletePanel>();
			LogToServer(false);
		}

		private void LoseLevel()
		{
			tracker.StopTracking();

			if(currentLevelToLog>=LevelDataController.instance.Level)
			{
				LevelDataController.instance.LevelData.level=currentLevelToLog;
				LevelDataController.instance.Lose();
			}

			StartCoroutine(Waits());

			IEnumerator Waits()
			{
				yield return new WaitForSecondsRealtime(1f);
				WindowManager.Instance.OpenWindow<LevelFailedPanel>(onLoaded: panel => { panel.SetFailedPanel(); });
				LogToServer(false);
			}
		}

		private void ReviveLevel() { tracker.ContinueTracking(); }


		private void LogToServer(bool win)
		{
			var numBooster      =InGameTracker.NumBooster;
			var totalTimeSession=InGameTracker.TotalTimeSession;
			var totalPurchased  =InGameTracker.TotalPurchased;

			LoadLevelManager.instance.OnLevelComplete(
			currentLevelToLog,
			currentLevelData,
			win,
			totalTimeSession,
			numBooster,
			totalPurchased);

		}
		public void ForceLoseLevel()
		{
			LogToServer(false);
			WindowManager.Instance.OpenWindow<LevelFailedPanel>(onLoaded: panel => { panel.SetFailedPanel(); });
		}
	}
}
