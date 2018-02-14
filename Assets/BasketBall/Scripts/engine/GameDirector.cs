using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.thinkagaingames.engine {
	public class GameDirector : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		protected List<PausableBehaviour> initList = null;

		[SerializeField]
		protected Camera cameraWorld = null;

		[SerializeField]
		protected Camera cameraUI = null;

		[SerializeField]
		protected Camera cameraSplash = null;

		[SerializeField]
		protected float splashDuration = 1f;

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

			Assert.That(cameraWorld != null, "World camera not found!", gameObject);
			Assert.That(cameraUI != null, "UI camera not found!", gameObject);
			Assert.That(cameraSplash != null, "Splash camera not found!", gameObject);

			StartCoroutine("PrepForInit");
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

			StartCoroutine("TransitionFromSplashToGame");
		}

		// HACK: get the cameras to see all game entities. This uploads the
		// graphical resources to the GPU and (hopefully) prevents frame
		// hitching later in the game.
		IEnumerator TransitionFromSplashToGame() {
			Switchboard.Broadcast("ExpandCanvas", null);
			Switchboard.Broadcast("WidenAllViews", null);

			yield return new WaitForSeconds(splashDuration);

			Switchboard.Broadcast("RestoreAllViews", null);
			Switchboard.Broadcast("RestoreCanvas", null);

			yield return new WaitForSeconds(splashDuration);

			Switchboard.Broadcast("ShiftSplashCamera", null);

			for (int i=0; i<initList.Count; ++i) {
				initList[i].OnStartGame();
			}

			initList.Clear();
		}

		// Button Handlers ////////////////////////////////////////////////////

		// Message Handlers ///////////////////////////////////////////////////
		public void UndoPreviousTransition(UiDirector.TransitionGroup group, bool transitionedIn) {
			UiDirector.Instance.UndoMostRecentTransition();
		}

		// Chunk Evaluators ///////////////////////////////////////////////////
	}
}
