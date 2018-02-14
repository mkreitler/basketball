/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       13/02/2018 22:18
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	[System.Serializable]
	public class StageParameters {
		public string name;
		public string touchZone;
		public GameObject cameraProxy;
		public float flightTime;
		public float lowShotThreshold;
		public float highShotThreshold;
		public AnimationCurve lowShotCurve;
		public AnimationCurve highShotCurve;
		public float driftThreshold;
		public float lateralDriftScalar;
		public float rotationalImpulse;
		public float viewportDistZ;
	}

	public class BasketballDirector : GameDirector {
		// Types and Constants ////////////////////////////////////////////////////
		private const float END_OF_LEVEL_DELAY = 5f;

		private enum eGameMode {
			TUTORIAL,
			GAME
		}

		[System.Serializable]
		private enum eTutorialStepTrigger {
			FLICK_START,
			FLICK_END
		}

		private const int PASSING_SCORE = 10;

		private const int STREAK_MULTIPLIER = 2;

		[System.Serializable]
		private class TutorialData {
			public eTutorialStepTrigger triggerType = eTutorialStepTrigger.FLICK_START;
			public int flickCount = -1;
			public string transitionName = null;
			public bool isTransitionIn = true;
			public string triggerFunction = null;
		}

		[System.Serializable]
		private class ProgressionInfo {
			public string name = null;
			public bool hasBlocker = false;
		}

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private List<TutorialData> tutorialSteps = null;

		[SerializeField]
		private float secondsPerLevel = 30f;

		[SerializeField]
		private List<ProgressionInfo> levelProgression = null;

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

		[SerializeField]
		private List<StageParameters> stages = null;

		[SerializeField]
		private RectTransform touchZone = null;

		[SerializeField]
		private RectTransform touchZonePointLeft;

		[SerializeField]
		private RectTransform touchZonePointCenter;

		[SerializeField]
		private RectTransform touchZonePointRight;

		[SerializeField]
		private GameObject blocker = null;

		// Interface //////////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------

		// Instance ---------------------------------------------------------------

		// Implementation /////////////////////////////////////////////////////////
		private eGameMode GameMode {get; set;}

		private int FlickStartCounter {get; set;}

		private int FlickEndCounter {get; set;}

		private int Score {get; set;}

		private int StageScore {get; set;}

		private int TotalStageScore {
			get {
				return (StageScore + BestStreak * STREAK_MULTIPLIER);
			}
		}

		private int BestStreak {get; set;}

		private int Stage {get; set;}

		private float Timer {get; set;}

		private int Streak {get; set;}

		private int CurrentStageIndex {get; set;}

		private bool IsGameClockRunning {get; set;}

		private void InitializeRound() {
			Score = 0;
			Stage = 0;
			FlickStartCounter = 0;
			FlickEndCounter = 0;

			scoreText.Refresh();
			timeText.Refresh();
			streakText.Refresh();
		}
		private void BeginNormalRound() {
			EnableTouchInput();
			StartGameClock();
		}

		private void HideTouchZone() {
			Image image = touchZone.gameObject.GetComponent<Image>();
			Assert.That(image != null, "Image component notfound!", gameObject);

			Color color = image.color;
			color.a = 0f;
			image.color = color;
		}

		private void SetUpNextStage() {
			StageParameters stageParams = GetStageParameters(levelProgression[CurrentStageIndex].name);
			Assert.That(stageParams != null, "Invalid state params!", gameObject);
			Switchboard.Broadcast("InitStage", stageParams);
			ResetStage();
		}

		private void ResetStage() {
			SetGameTime(secondsPerLevel);

			blocker.SetActive(levelProgression[CurrentStageIndex].hasBlocker);
			SetStreak(0);
			BestStreak = 0;
			StageScore = 0;
		}

		// Interfaces /////////////////////////////////////////////////////////////
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

		private StageParameters GetStageParameters(string stageName) {
			StageParameters parameters = null;

			stageName = stageName.ToLower();
			for (int i=0; i<stages.Count; ++i) {
				if (stages[i] != null && stages[i].name != null && stages[i].name.ToLower() == stageName) {
					parameters = stages[i];
					break;
				}
			}
			
			return parameters;
		}

		protected void EndStage() {
			SoundSystem.PlaySound("whistle");

			DisableTouchInput();
			StopGameClock();
			SetGameTime(0f);
			Score += StageScore + BestStreak * 2;
			
			Switchboard.Broadcast("StageEnded", null);

			if (GameMode == eGameMode.TUTORIAL) {
				EndTutorial();
			}
			else {
				StartCoroutine("DisplayResultsAndWait");
			}
		}

		private IEnumerator DisplayResultsAndWait() {
			CurrentStageIndex += 1;
			UiDirector.Instance.StartTransition("GameHUD", false);
			UiDirector.Instance.StartTransition("ResultsPanel", true);

			yield return new WaitForSeconds(END_OF_LEVEL_DELAY);

			UiDirector.Instance.UndoMostRecentTransition();
		}

		public void ShowGameHud() {
			UiDirector.Instance.StartTransition("GameHUD", true);
		}

		public void GameOnOrGameOver() {
			if (CurrentStageIndex >= levelProgression.Count || TotalStageScore < PASSING_SCORE) {
				UiDirector.Instance.ResetHistory();

				// Show game over message and return to main menu.
				UiDirector.Instance.StartTransition("BlackoutPanels", true);
			}
			else {
				UiDirector.Instance.StartTransition("BlackoutLevelEnd", true);
			}
		}

		public void PrepNextStage() {
			// Show the next level.
			SetUpNextStage();
			UiDirector.Instance.StartTransition("BlackoutLevelEnd", false);
		}

		protected void EndTutorial() {
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

			Assert.That(touchZone != null, "Invalid touch zone!", gameObject);

			Assert.That(touchZonePointLeft != null, "Invalid touch zone point (left)!", gameObject);
			Assert.That(touchZonePointLeft != null, "Invalid touch zone point (center)!", gameObject);
			Assert.That(touchZonePointLeft != null, "Invalid touch zone point (right)!", gameObject);

			StringTable.RegisterChunkEvaluator("GetCurrentScore", GetCurrentScore);
			StringTable.RegisterChunkEvaluator("GetCurrentTime", GetCurrentTime);
			StringTable.RegisterChunkEvaluator("GetCurrentStreak", GetCurrentStreak);
			StringTable.RegisterChunkEvaluator("GetShotScore", GetShotScore);
			StringTable.RegisterChunkEvaluator("GetStreakBonus", GetStreakBonus);
			StringTable.RegisterChunkEvaluator("GetTotalScore", GetTotalScore);
			StringTable.RegisterChunkEvaluator("GetStageScore", GetStageScore);
			StringTable.RegisterChunkEvaluator("GetResultsMessage", GetResultsMessage);
		}

		public override void OnStartGame() {
			UiDirector.Instance.StartTransition("BlackoutPanels", true);

			Switchboard.AddListener("FlickStart", OnFlickStart);
			Switchboard.AddListener("FlickEnd", OnFlickEnd);
			Switchboard.AddListener("PlayerScored", OnPlayerScored);
			Switchboard.AddListener("PlayerMissed", OnPlayerMissed);
			Switchboard.AddListener("initStage", InitStage);

			HideTouchZone();			
		}

		// Coroutines /////////////////////////////////////////////////////////////
		IEnumerator CountDownToMainMenu() {
			yield return new WaitForSeconds(levelEndDelay);

			UiDirector.Instance.ResetHistory();
			UiDirector.Instance.StartTransition("BlackoutPanels", true);
		}

		// Button Handlers ////////////////////////////////////////////////////
		public void MainMenuStartGame() {
			cameraWorld.enabled = true;
			GameMode = eGameMode.GAME;
			CurrentStageIndex = 0;
			UiDirector.Instance.UndoMostRecentTransition();
			SoundSystem.FadeOutMusic();
			SetUpNextStage();
			EnableTouchInput();
		}

		public void MainMenuStartTutorial() {
			StageParameters stageParams = GetStageParameters("tutorial");
			Switchboard.Broadcast("InitStage", stageParams);
			
			DisableTouchInput();
			blocker.SetActive(false);
			cameraWorld.enabled = true;
			GameMode = eGameMode.TUTORIAL;
			UiDirector.Instance.UndoMostRecentTransition();
			ResetStage();
			SoundSystem.FadeOutMusic();
		}

		// Message Handlers ///////////////////////////////////////////////////
		public void InitStage(object objParams) {
			StageParameters parameters = objParams as StageParameters;
			Assert.That(parameters != null, "Invalid stage parameters!", gameObject);
			Assert.That(parameters.touchZone != null, "Invalid touch zone parameter!", gameObject);

			string zoneConfig = parameters.touchZone.ToLower();
			switch(zoneConfig) {
				case "left":
					MoveTouchZoneLeft();
				break;

				case "right":
					MoveTouchZoneRight();
				break;

				default:
					MoveTouchZoneCenter();
				break;
			}
		}

		public void OnPlayerMissed(object ignored) {
			if (Streak > 1) {
				SoundSystem.PlaySound("buzzer");
			}

			SetStreak(0);
		}

		private void SetStreak(int newStreak) {
			Streak = newStreak;
			streakText.Refresh();
		}

		private void SetScore(int newScore) {
			Score = newScore;
			scoreText.Refresh();
		}

		private void SetGameTime(float newTime) {
			Timer = newTime;
			timeText.Refresh();
		}

		public void OnPlayerScored(object ignored) {
			StageScore += 2;
			scoreText.Refresh();

			Streak += 1;
			Streak = Mathf.Min(Streak, maxStreak);
			BestStreak = Mathf.Max(Streak, BestStreak);
			streakText.Refresh();

			// TODO: play sound?
		}

		public void BeginRound() {
			Switchboard.Broadcast("BeginRound", GameMode == eGameMode.TUTORIAL);

			if (GameMode == eGameMode.TUTORIAL) {
				UiDirector.Instance.StartTransition("TutorialLesson01", true);
			}
			else {
				BeginNormalRound();
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

		public void StartGameMode(UiDirector.TransitionGroup group, bool transitionedIn) {
			InitializeRound();
			
			switch (GameMode) {
				case eGameMode.TUTORIAL:
					MoveTouchZoneLeft();
					DisableTouchInput();
					UiDirector.Instance.ResetHistory();
					UiDirector.Instance.StartTransition("GameHUD", true);
					IsGameClockRunning = false;
				break;

				case eGameMode.GAME:
					EnableTouchInput();
					UiDirector.Instance.ResetHistory();
					UiDirector.Instance.StartTransition("GameHUD", true);
					IsGameClockRunning = false;
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

		public void MoveTouchZoneCenter() {
			touchZone.localPosition = touchZonePointCenter.localPosition;
		}

		public void MoveTouchZoneLeft() {
			touchZone.localPosition = touchZonePointLeft.localPosition;
		}

		public void MoveTouchZoneRight() {
			touchZone.localPosition = touchZonePointRight.localPosition;
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
			return "" + StageScore;
		}

		private string GetCurrentTime(string chunk) {
			return "0:" + Mathf.RoundToInt(Timer);
		}

		private string GetCurrentStreak(string chunk) {
			return "x" + Streak;
		}

		private string GetShotScore(string chunk) {
			return StringTable.GetString("KEY_SHOT_SCORE") + ": " + StageScore;
		}

		private string GetStreakBonus(string chunk) {
			return StringTable.GetString("KEY_STREAK_BONUS") + ": " + (BestStreak * 2);
		}

		private string GetTotalScore(string chunk) {
			return StringTable.GetString("KEY_TOTAL_SCORE") + ": " + Score;
		}

		private string GetStageScore(string chunk) {
			return StringTable.GetString("KEY_STAGE_SCORE") + ": " + (StageScore + BestStreak * 2);
		}

		private string GetResultsMessage(string chunk) {
			string message = null;

			if (TotalStageScore < PASSING_SCORE) {
				message = StringTable.GetString("KEY_GAME_OVER");
			}
			else if (CurrentStageIndex >= levelProgression.Count) {
				message = StringTable.GetString("KEY_GAME_OVER");
			}
			else {
				message = StringTable.GetString("KEY_GOOD_JOB");
			}

			return message;
		}
	}
}
