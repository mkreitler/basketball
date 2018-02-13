using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	public class StopPlane : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		// Implementation /////////////////////////////////////////////////////////
		// Interfaces /////////////////////////////////////////////////////////////
		public void OnTriggerEnter(Collider collider) {
			GameObject goOther = collider.gameObject;

			if (goOther != null) {
				BallBasic ball = goOther.GetComponent<BallBasic>();
				if (ball != null) {
					ball.StopPhysics();
				}
			}
		}
		// Coroutines /////////////////////////////////////////////////////////////
	}
}
