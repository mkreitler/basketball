using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.engine {
	public class GameDirector : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		private enum eGameMode {
			TUTORIAL
		}

		[System.Serializable]
		private enum eTutorialStepTrigger {
			FLICK_START,
			FLICK_END
		}

		[System.Serializable]
		private class TutorialData {
			public eTutorialStepTrigger triggerType = eTutorialStepTrigger.FLICK_START;
			public int flickCount = -1;
			public string transitionName = null;
			public bool isTransitionIn = true;
			public string triggerFunction = null;
		}

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private List<PausableBehaviour> initList = null;

		[SerializeField]
		private Camera cameraWorld = null;

		[SerializeField]
		private Camera cameraUI = null;

		[SerializeField]
		private Camera cameraSplash = null;

		[SerializeField]
		private List<TutorialData> tutorialSteps = null;

		[SerializeField]
		private float secondsPerLevel = 30f;

		[SerializeField]
		private Localizer scoreText = null;

		[SerializeField]
		private Localizer timeText = null;

		[SerializeField]
		private Localizer streakText = null;

		[SerializeField]
		private float levelEndDelay = 3f;

		[SerializeField]
		private int maxStreak = 5;

		// Interface //////////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------
		public static GameDirector Instance = null;

		public static bool AddToInitList(PausableBehaviour pausable) {
		 	Assert.That(Instance != null, "GameDirector instance not found!");			
			return Instance._AddToInitList(pausable);
		}

		public static void PauseGame() {
			Time.timeScale = 0f;

			Object[] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
			for (int i=0; i<allObjects.Length; ++i) {
				(allObjects[i] as GameObject).SendMessage("OnPauseGame", SendMessageOptions.DontRequireReceiver);
			}
		}

		public static void ResumeGame() {
			Object[] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
			for (int i=0; i<allObjects.Length; ++i) {
				(allObjects[i] as GameObject).SendMessage("OnResumeGame", SendMessageOptions.DontRequireReceiver);
			}

			Time.timeScale = 1f;
		}

		// Instance ---------------------------------------------------------------
		public bool _AddToInitList(PausableBehaviour pausable) {
			bool bAlreadyAdded = initList.Contains(pausable);

			if (!bAlreadyAdded) {
				initList.Add(pausable);
			}

			return bAlreadyAdded;
		}

		// Implementation /////////////////////////////////////////////////////////
		private eGameMode GameMode {get; set;}

		private int FlickStartCounter {get; set;}

		private int FlickEndCounter {get; set;}

		private int Score {get; set;}

		private int Stage {get; set;}

		private float Timer {get; set;}

		private int Streak {get; set;}

		private bool IsGameClockRunning {get; set;}

		private void InitializeRound() {
			Score = 0;
			Timer = secondsPerLevel;
			Stage = 0;
			Streak = 0;
			IsGameClockRunning = true;
			FlickStartCounter = 0;
			FlickEndCounter = 0;

			scoreText.Refresh();
			timeText.Refresh();
			streakText.Refresh();
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();
			
			Assert.That(Instance == null, "Awake: Found multiple GameDirectors!", gameObject);

			Instance = this;
		}

		protected void OnDestroy() {
			Instance = null;
		}

		protected void FixedUpdate() {
			if (IsGameClockRunning) {
				Timer -= Time.fixedDeltaTime;

				if (Timer <= 0f) {
					// TODO: end the round.
					EndStage();
				}

				timeText.Refresh();
			}
		}

		protected void EndStage() {
			DisableTouchInput();
			StopGameClock();
			Timer = 0f;
			Switchboard.Broadcast("StageEnded", null);

			if (GameMode == eGameMode.TUTORIAL) {
				EndRound();
			}
			else {
				// TODO: Check score and move on to the next stage, if appropriate.
			}
		}

		protected void EndRound() {
			UiDirector.Instance.StartTransition("GameHUD", false);
			StartCoroutine("CountDownToMainMenu");
		}

		protected override void Start() {
			base.Start();

			Assert.That(cameraWorld != null, "World camera not found!", gameObject);
			Assert.That(cameraUI != null, "UI camera not found!", gameObject);
			Assert.That(cameraSplash != null, "Splash camera not found!", gameObject);

			Assert.That(scoreText != null, "Score text not defined!", gameObject);
			Assert.That(timeText != null, "Time text not defined!", gameObject);
			Assert.That(streakText != null, "Streak text not defined!", gameObject);

			StartCoroutine("PrepForInit");

			StringTable.RegisterChunkEvaluator("GetCurrentScore", GetCurrentScore);
			StringTable.RegisterChunkEvaluator("GetCurrentTime", GetCurrentTime);
			StringTable.RegisterChunkEvaluator("GetCurrentStreak", GetCurrentStreak);
		}

		public override void OnStartGame() {
			UiDirector.Instance.StartTransition("BlackoutPanels", true);

			Switchboard.AddListener("FlickStart", OnFlickStart);
			Switchboard.AddListener("FlickEnd", OnFlickEnd);
			Switchboard.AddListener("PlayerScored", OnPlayerScored);
			Switchboard.AddListener("PlayerMissed", OnPlayerMissed);
		}

		// Coroutines /////////////////////////////////////////////////////////////
		IEnumerator PrepForInit() {
			bool isWaitingForInit = true;

			while (isWaitingForInit) {
				isWaitingForInit = false;
				for (int i=0; i<initList.Count; ++i) {
					if (!initList[i].ReadyToStart) {
						isWaitingForInit = true;
						break;
					}
				}

				yield return new WaitForEndOfFrame();
			}

			for (int i=0; i<initList.Count; ++i) {
				initList[i].OnStartGame();
			}

			initList.Clear();
		}

		IEnumerator CountDownToMainMenu() {
			yield return new WaitForSeconds(levelEndDelay);

			UiDirector.Instance.ResetHistory();
			UiDirector.Instance.StartTransition("BlackoutPanels", true);
		}

		// Button Handlers ////////////////////////////////////////////////////
		public void MainMenuStartTutorial() {
			cameraWorld.enabled = true;
			GameMode = eGameMode.TUTORIAL;
			UiDirector.Instance.UndoMostRecentTransition();
			SoundSystem.FadeOutMusic();
		}

		// Message Handlers ///////////////////////////////////////////////////
		public void OnPlayerMissed(object ignored) {
			if (Streak > 1) {
				SoundSystem.PlaySound("buzzer");
			}

			Streak = 0;
			streakText.Refresh();
		}

		public void OnPlayerScored(object ignored) {
			Score += 1 + Mathf.Max(1, Streak);
			scoreText.Refresh();

			Streak += 1;
			Streak = Mathf.Min(Streak, maxStreak);
			streakText.Refresh();

			// TODO: play sound?
		}

		public void BeginRoundOne() {
			Switchboard.Broadcast("BeginRound", GameMode == eGameMode.TUTORIAL);

			if (GameMode == eGameMode.TUTORIAL) {
				UiDirector.Instance.StartTransition("TutorialLesson01", true);
			}
		}

		public void ShowTitle(UiDirector.TransitionGroup group, bool transitionedIn) {
			UiDirector.Instance.StartTransition("TitlePanels", true);
			cameraSplash.enabled = false;
			cameraWorld.enabled = false;
			SoundSystem.PlayMusic("title_track");
		}

		public void ShowMainMenu(UiDirector.TransitionGroup group, bool transitionedIn) {
			UiDirector.Instance.StartTransition("MainMenuOptions", true);
		}

		public void UndoPreviousTransition(UiDirector.TransitionGroup group, bool transitionedIn) {
			UiDirector.Instance.UndoMostRecentTransition();
		}

		public void StartGameMode(UiDirector.TransitionGroup group, bool transitionedIn) {
			InitializeRound();
			
			switch (GameMode) {
				case eGameMode.TUTORIAL:
					// TODO:
					// Select the correct world configuration.
					UiDirector.Instance.ResetHistory();
					UiDirector.Instance.StartTransition("GameHUD", true);
					IsGameClockRunning = false;
					DisableTouchInput();
				break;
			}
		}

		public void OnFlickStart(object ignored) {
			if (GameMode == eGameMode.TUTORIAL) {
				FlickStartCounter += 1;

				for (int i=0; i<tutorialSteps.Count; ++i) {
					if (tutorialSteps[i].triggerType == eTutorialStepTrigger.FLICK_START && FlickStartCounter == tutorialSteps[i].flickCount) {
						UiDirector.Instance.StartTransition(tutorialSteps[i].transitionName, tutorialSteps[i].isTransitionIn);
						if (tutorialSteps[i].triggerFunction != null && tutorialSteps[i].triggerFunction.Length > 0) {
							this.Invoke(tutorialSteps[i].triggerFunction, 0f);
						}
					}
				}
			}
		}

		public void OnFlickEnd(object ignored) {
			if (GameMode == eGameMode.TUTORIAL) {
				FlickEndCounter += 1;

				for (int i=0; i<tutorialSteps.Count; ++i) {
					if (tutorialSteps[i].triggerType == eTutorialStepTrigger.FLICK_END && FlickEndCounter == tutorialSteps[i].flickCount) {
						UiDirector.Instance.StartTransition(tutorialSteps[i].transitionName, tutorialSteps[i].isTransitionIn);
						if (tutorialSteps[i].triggerFunction != null && tutorialSteps[i].triggerFunction.Length > 0) {
							this.Invoke(tutorialSteps[i].triggerFunction, 0f);
						}
					}
				}
			}
		}

		public void PlayShortWhistle() {
			SoundSystem.PlaySound("short_whistle");
		}

		public void StartGameClock() {
			IsGameClockRunning = true;
			SoundSystem.PlaySound("whistle");
		}

		public void StopGameClock() {
			IsGameClockRunning = false;
		}

		public void EnableTouchInput() {
			Switchboard.Broadcast("EnableTouchInput", null);
		}

		public void DisableTouchInput() {
			Switchboard.Broadcast("DisableTouchInput", null);
		}

		// Chunk Evaluators ///////////////////////////////////////////////////
		private string GetCurrentScore(string chunk) {
			return "" + Score;
		}

		private string GetCurrentTime(string chunk) {
			return "0:" + Mathf.RoundToInt(Timer);
		}

		private string GetCurrentStreak(string chunk) {
			return "x" + Streak;
		}
	}
}
