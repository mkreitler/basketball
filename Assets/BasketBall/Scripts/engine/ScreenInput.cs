using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.thinkagaingames.engine {
	[System.Serializable]
	public class ScreenInputBegin : UnityEvent<Vector2, Vector2> {}

	[System.Serializable]
	public class ScreenInputMove : UnityEvent<Vector2, Vector2> {}

	[System.Serializable]
	public class ScreenInputHold : UnityEvent<Vector2, Vector2> {}

	[System.Serializable]
	public class ScreenInputEnd : UnityEvent<Vector2, Vector2> {}

	[System.Serializable]
	public enum eInputMode {
		UNKNOWN,
		MOUSE,
		TOUCH
	}

	public class ScreenInput : PausableBehaviour {
		public static eInputMode Mode {get; set;}

		// Types and Constants ////////////////////////////////////////////////////
		private const float MOVE_TOLERANCE = 10f;	// pixels

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private ScreenInputBegin onScreenInputBegin = null;

		[SerializeField]
		private ScreenInputMove onScreenInputMove = null;

		[SerializeField]
		private ScreenInputHold onScreenInputHold = null;

		[SerializeField]
		private ScreenInputEnd onScreenInputEnd = null;

		[SerializeField]
		private float moveTolerance = MOVE_TOLERANCE;

		[SerializeField]
		private Camera uiCamera = null;

		// Interface //////////////////////////////////////////////////////////////
		public Vector2 ContactPoint {get; set;}

		public bool IsUserContact {get; set;}
		
		// Implementation /////////////////////////////////////////////////////////
		public Vector2 ContactPointStart {get; set;}
		public Vector2 ContactPointCurrent {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		protected virtual void Awake() {
			Assert.That(uiCamera != null, "UI Camera not found!", gameObject);
			Mode = eInputMode.UNKNOWN;
		}

		protected virtual void Update() {
			if (Input.touchCount > 0) {
				ContactPoint = Input.touches[0].position;
				Vector3 vScreenPoint = uiCamera.ScreenToViewportPoint(ContactPoint);
				Mode = eInputMode.TOUCH;

				switch(Input.touches[0].phase) {
					case TouchPhase.Began: {
						if (onScreenInputBegin != null) {
							onScreenInputBegin.Invoke(ContactPoint, vScreenPoint);
						}
					}
					break;

					case TouchPhase.Moved: {
						if (onScreenInputMove != null) {
							onScreenInputMove.Invoke(ContactPoint, vScreenPoint);
						}
					}
					break;

					case TouchPhase.Stationary: {
						if (onScreenInputHold != null) {
							onScreenInputHold.Invoke(ContactPoint, vScreenPoint);
						}
					}
					break;

					case TouchPhase.Ended: case TouchPhase.Canceled: {
						if (onScreenInputEnd != null) {
							onScreenInputEnd.Invoke(ContactPoint, vScreenPoint);
						}
					}
					break;
				}
			}
			else if (Input.GetMouseButton(0)) {
				Mode = eInputMode.MOUSE;

				if (!IsUserContact) {
					IsUserContact = true;
					ContactPoint = Input.mousePosition;
					Vector3 vScreenPoint = uiCamera.ScreenToViewportPoint(ContactPoint);

					if (onScreenInputBegin != null) {
						onScreenInputBegin.Invoke(ContactPoint, vScreenPoint);
					}
				}
				else {
					float dx = Input.mousePosition.x - ContactPoint.x;
					float dy = Input.mousePosition.y - ContactPoint.y;

					if (Mathf.Abs(dx) + Mathf.Abs(dy) > moveTolerance) {
						ContactPoint = Input.mousePosition;
						Vector3 vScreenPoint = uiCamera.ScreenToViewportPoint(ContactPoint);

						if (onScreenInputMove != null) {
							onScreenInputMove.Invoke(ContactPoint, vScreenPoint);
						}
					}
					else {
						if (onScreenInputHold != null) {
							Vector3 vScreenPoint = uiCamera.ScreenToViewportPoint(ContactPoint);
							onScreenInputHold.Invoke(ContactPoint, vScreenPoint);
						}
					}
				}

				if (!IsUserContact) {
					ContactPointStart = Input.mousePosition;
					IsUserContact = true;
				}
			}
			else if (IsUserContact) {
				IsUserContact = false;
				if (onScreenInputEnd != null) {
					Vector3 vScreenPoint = uiCamera.ScreenToViewportPoint(ContactPoint);
					onScreenInputEnd.Invoke(ContactPoint, vScreenPoint);
				}
			}
		}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
