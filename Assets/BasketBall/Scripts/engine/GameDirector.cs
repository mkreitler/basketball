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

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private List<PausableBehaviour> initList = null;

		[SerializeField]
		private Camera cameraWorld = null;

		[SerializeField]
		private Camera cameraUI = null;

		[SerializeField]
		private Camera cameraSplash = null;

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
		private eGameMode NextGameMode {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();
			
			Assert.That(Instance == null, "Awake: Found multiple GameDirectors!", gameObject);

			Instance = this;
		}

		protected void OnDestroy() {
			Instance = null;
		}

		protected override void Start() {
			base.Start();

			Assert.That(cameraWorld != null, "World camera not found!", gameObject);
			Assert.That(cameraUI != null, "UI camera not found!", gameObject);
			Assert.That(cameraSplash != null, "Splash camera not found!", gameObject);

			StartCoroutine("PrepForInit");

			StringTable.RegisterChunkEvaluator("GetCurrentScore", GetCurrentScore);
			StringTable.RegisterChunkEvaluator("GetCurrentTime", GetCurrentTime);
			StringTable.RegisterChunkEvaluator("GetCurrentStreak", GetCurrentStreak);
		}

		public override void OnStartGame() {
			UiDirector.Instance.StartTransition("BlackoutPanels", true);
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

		// Button Handlers ////////////////////////////////////////////////////
		public void MainMenuStartTutorial() {
			cameraWorld.enabled = true;
			NextGameMode = eGameMode.TUTORIAL;
			UiDirector.Instance.UndoMostRecentTransition();
		}

		// Message Handlers ///////////////////////////////////////////////////
		public void ShowTitle(UiDirector.TransitionGroup group) {
			UiDirector.Instance.StartTransition("TitlePanels", true);
			cameraSplash.enabled = false;
		}

		public void ShowMainMenu(UiDirector.TransitionGroup group) {
			UiDirector.Instance.StartTransition("MainMenuOptions", true);
		}

		public void UndoPreviousTransition(UiDirector.TransitionGroup group) {
			UiDirector.Instance.UndoMostRecentTransition();
		}

		public void StartGameMode(UiDirector.TransitionGroup group) {
			switch (NextGameMode) {
				case eGameMode.TUTORIAL:
					// TODO:
					// Set "tutorial" flag.
					// Select the correct world configuration.
					UiDirector.Instance.StartTransition("GameHUD", true);
				break;
			}
		}

		// Chunk Evaluators ///////////////////////////////////////////////////
		private string GetCurrentScore(string chunk) {
			return "310";
		}

		private string GetCurrentTime(string chunk) {
			return "0:10";
		}

		private string GetCurrentStreak(string chunk) {
			return "x7";
		}
	}
}
