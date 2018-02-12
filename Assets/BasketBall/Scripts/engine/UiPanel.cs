/**
 *	UiPanel manages a group of UI elements and is the atomic instance manipulated
 *  by the UiDirector.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.thinkagaingames.engine {
	public class UiPanel : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		private const float DEFAULT_TRANSITION_TIME = 0.5f;

		private delegate void updateState();

		[System.Serializable]
		public enum eTransitionType {
			FADE,
			SLIDE,
		}

		[System.Serializable]
		public enum eStartState {
			OFF_BOTTOM,
			OFF_LEFT,
			OFF_RIGHT,
			OFF_TOP,
			ON_SCREEN
		}

		[System.Serializable]
		private class FadeInfo {
			public MaskableGraphic element = null;
			public float targetAlpha = 1f;
		}

		[System.Serializable]
		private class TransitionInfo {
			public eTransitionType type = eTransitionType.SLIDE;
			public AnimationCurve curve = null;
			public RectTransform onScreenTransform = null;
		}

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private List<TransitionInfo>transitionInfo = null;

		[SerializeField]
		private List<FadeInfo>fadeInfo = null;

		[SerializeField]
		private float transitionTime = DEFAULT_TRANSITION_TIME;

		// Interface //////////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------

		// Instance ---------------------------------------------------------------

		// Implementation /////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------
		public void TransitionIn() {
			gameObject.SetActive(true);

			if (NewTransition) {
				TransitionParam = 0f;
			}

			FixedUpdateState = UpdateTransitionIn;
		}

		public void TransitionOut() {
			if (NewTransition) {
				TransitionParam = 1f;
			}

			FixedUpdateState = UpdateTransitionOut;
		}

		// Instance ---------------------------------------------------------------
		private updateState FixedUpdateState {get; set;}

		private float TransitionParam {get; set;}

		private bool NewTransition {get; set;}

		private Vector3 OffScreenLocalPosition {get; set;}

		private RectTransform Transform2D {get; set;}

		private void UpdateTransitionIn() {
			TransitionParam += Time.fixedDeltaTime / transitionTime;
			TransitionParam = Mathf.Min(TransitionParam, 1f);

			for (int i=0; i<transitionInfo.Count; ++i) {
				ResolveProgress(transitionInfo[i], TransitionParam);
			}

			if (TransitionParam == 1f) {
				NewTransition = true;
				FixedUpdateState = null;
				Switchboard.Broadcast("TransitionComplete", name);
			}
		}

		private void UpdateTransitionOut() {
			TransitionParam -= Time.fixedDeltaTime / transitionTime;
			TransitionParam = Mathf.Max(0f, TransitionParam);

			for (int i=0; i<transitionInfo.Count; ++i) {
				ResolveProgress(transitionInfo[i], TransitionParam);
			}

			if (TransitionParam == 0f) {
				NewTransition = true;
				FixedUpdateState = null;
				Switchboard.Broadcast("TransitionComplete", name);
				gameObject.SetActive(false);
			}
		}

		private void ResolveProgress(TransitionInfo info, float param) {
			Assert.That(param >= 0f && param <= 1f, "Transition paramter out of range!", gameObject);
			Assert.That(info.curve != null, "Undefined transition curve!", gameObject);

			param = info.curve.Evaluate(param);

			switch(info.type) {
				case eTransitionType.FADE:
					ResolveFade(param);
				break;

				case eTransitionType.SLIDE:
					ResolveSlide(param, info.onScreenTransform);
				break;
			}
		}

		private void ResolveFade(float param) {
			for (int i=0; i<fadeInfo.Count; ++i) {
				Color color = fadeInfo[i].element.color;
				color.a = fadeInfo[i].targetAlpha * param;
				fadeInfo[i].element.color = color;
			}
		}

		private void ResolveSlide(float param, RectTransform onScreenTransform) {
			Transform2D.localPosition = Vector3.Lerp(OffScreenLocalPosition, onScreenTransform.localPosition, param);
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();
			
			UiDirector.RegisterPanel(name, this);
			NewTransition = true;
			Transform2D = gameObject.GetComponent<RectTransform>();

			Assert.That(Transform2D != null, "RectTransform component not found!", gameObject);
		}

		protected override void Start() {
			base.Start();

			for (int i=0; i<fadeInfo.Count; ++i) {
				Assert.That(fadeInfo[i].element != null, "Fade element not defined!", gameObject);
			}

			// Copy our target transform.
			OffScreenLocalPosition = Transform2D.localPosition;
		}

		protected void FixedUpdate() {
			if (FixedUpdateState != null) {
				FixedUpdateState();
			}
		}
		
		// Coroutines /////////////////////////////////////////////////////////////
	}
}
