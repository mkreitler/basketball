using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.engine {
	public class GameDirector : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private List<PausableBehaviour> initList = null;

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

			StartCoroutine("PrepForInit");
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

		// Message Handlers ///////////////////////////////////////////////////
		public void ShowTitle(UiDirector.TransitionGroup group) {
			UiDirector.Instance.StartTransition("TitlePanels", true);
		}

		public void ShowMainMenu(UiDirector.TransitionGroup group) {
			UiDirector.Instance.StartTransition("MainMenuOptions", true);
		}

		public void UndoPreviousTransition(UiDirector.TransitionGroup group) {
			UiDirector.Instance.UndoMostRecentTransition();
		}

		public void StartGameMode(UiDirector.TransitionGroup group) {
			Debug.Log(">>> StartGameMode");
		}
	}
}
