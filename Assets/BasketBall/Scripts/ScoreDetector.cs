using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace thinkagaingames.com.basketball {
	public class ScoreDetector : MonoBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		public void OnTriggerEnter(Collider collider) {
			if (collider.gameObject != null && collider.gameObject.tag == "ball" && !WaitingForExit) {
				WaitingForExit = true;
				collider.gameObject.SendMessage("EnteredGoal", SendMessageOptions.DontRequireReceiver);

				Rigidbody rbIncoming = collider.gameObject.GetComponent<Rigidbody>();

				if (rbIncoming && Vector3.Dot(rbIncoming.velocity, Vector3.up) < 0f) {
					EntryHeight = collider.gameObject.transform.position.z;
					EnteredFromAbove = true;
				}
			}
		}

		public void OnTriggerExit(Collider collider) {
			if (collider.gameObject != null && collider.gameObject.tag == "ball") {
				WaitingForExit = false;
				collider.gameObject.SendMessage("ExitedGoal", SendMessageOptions.DontRequireReceiver);

				Rigidbody rbIncoming = collider.gameObject.GetComponent<Rigidbody>();
				ExitHeight = collider.gameObject.transform.position.z;

				if (rbIncoming && Vector3.Dot(rbIncoming.velocity, Vector3.up) < 0f && ExitHeight < EntryHeight) {
					Score();
				}

				EnteredFromAbove = false;
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private float EntryHeight {get; set;}
		private float ExitHeight {get; set;}

		private bool EnteredFromAbove {get; set;}
		private bool WaitingForExit {get; set;}

		private void Score() {
			Debug.Log(">>> SCORE !!! <<<");
		}

		// Interfaces /////////////////////////////////////////////////////////////
		// Coroutines /////////////////////////////////////////////////////////////
	}
}
