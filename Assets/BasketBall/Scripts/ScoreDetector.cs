using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace thinkagaingames.com.basketball {
	public class ScoreDetector : MonoBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		public void OnTriggerEnter(Collider collider) {
			if (collider.gameObject != null && collider.gameObject.tag == "ball") {
				if (collider.gameObject.transform.position.y > gameObject.transform.position.y) {
					EnteredFromAbove = true;
				}
			}
		}

		public void OnTriggerExit(Collider collider) {
			if (collider.gameObject != null && collider.gameObject.tag == "ball") {
				if (EnteredFromAbove && collider.gameObject.transform.position.y < gameObject.transform.position.y) {
					Score();
				}

				EnteredFromAbove = false;
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private bool EnteredFromAbove {get; set;}

		private void Score() {
			Debug.Log(">>> SCORE !!! <<<");
		}

		// Interfaces /////////////////////////////////////////////////////////////
		// Coroutines /////////////////////////////////////////////////////////////
	}
}
