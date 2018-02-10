using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace thinkagaingames.com.GAME_NAME {
	[System.Serializable]
	public class ScreenInputBegin : UnityEvent<Vector2> {}

	[System.Serializable]
	public class ScreenInputMove : UnityEvent<Vector2> {}

	[System.Serializable]
	public class ScreenInputHold : UnityEvent<Vector2> {}

	[System.Serializable]
	public class ScreenInputEnd : UnityEvent<Vector2> {}

	public class ScreenInput : MonoBehaviour {
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

		// Interface //////////////////////////////////////////////////////////////
		public Vector2 ContactPoint {get; set;}

		public bool IsUserContact {get; set;}

		public void Update() {
			if (Input.touchCount > 0) {
				ContactPoint = Input.touches[0].position;

				switch(Input.touches[0].phase) {
					case TouchPhase.Began: {
						if (onScreenInputBegin != null) {
							onScreenInputBegin.Invoke(ContactPoint);
						}
					}
					break;

					case TouchPhase.Moved: {
						if (onScreenInputMove != null) {
							onScreenInputMove.Invoke(ContactPoint);
						}
					}
					break;

					case TouchPhase.Stationary: {
						if (onScreenInputHold != null) {
							onScreenInputHold.Invoke(ContactPoint);
						}
					}
					break;

					case TouchPhase.Ended: case TouchPhase.Canceled: {
						if (onScreenInputEnd != null) {
							onScreenInputEnd.Invoke(ContactPoint);
						}
					}
					break;
				}
			}
			else if (Input.GetMouseButton(0)) {
				if (!IsUserContact) {
					IsUserContact = true;
					ContactPoint = Input.mousePosition;

					if (onScreenInputBegin != null) {
						onScreenInputBegin.Invoke(ContactPoint);
					}
				}
				else {
					float dx = Input.mousePosition.x - ContactPoint.x;
					float dy = Input.mousePosition.y - ContactPoint.y;

					if (Mathf.Abs(dx) + Mathf.Abs(dy) > moveTolerance) {
						ContactPoint = Input.mousePosition;

						if (onScreenInputMove != null) {
							onScreenInputMove.Invoke(ContactPoint);
						}
					}
					else {
						if (onScreenInputHold != null) {
							onScreenInputHold.Invoke(ContactPoint);
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
					onScreenInputEnd.Invoke(ContactPoint);
				}
			}
		}
		
		// Implementation /////////////////////////////////////////////////////////
		public Vector2 ContactPointStart {get; set;}
		public Vector2 ContactPointCurrent {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		// Coroutines /////////////////////////////////////////////////////////////
	}
}
