// DUMMY COMMENT!
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	public class ScoreDetector : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private bool isTop = true;

		// Interface //////////////////////////////////////////////////////////////

		public void OnTriggerEnter(Collider collider) {
			if (collider.gameObject != null && collider.gameObject.tag.ToLower().Contains("ball_")) {
				if (isTop) {
					Switchboard.Broadcast("EnteredGoal", collider.gameObject);
				}
				else {
					Switchboard.Broadcast("EnteredScoringZone", collider.gameObject);
				}
			}
		}

		public void OnTriggerExit(Collider collider) {
			if (collider.gameObject != null && collider.gameObject.tag.ToLower().Contains("ball_")) {
				if (!isTop) {
					Switchboard.Broadcast("ExitedGoal", collider.gameObject);
				}
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private float EntryHeight {get; set;}
		private float ExitHeight {get; set;}

		private bool EnteredFromAbove {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();
		}

		// Coroutines /////////////////////////////////////////////////////////////

		// Message Handlers ///////////////////////////////////////////////////////
	}
}
