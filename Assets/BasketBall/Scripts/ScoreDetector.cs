using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	public class ScoreDetector : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private ParticleMaster scoreParticle = null;

		// Interface //////////////////////////////////////////////////////////////
		private bool TriggerEntered {get; set;}

		public void OnTriggerEnter(Collider collider) {
			if (!TriggerEntered) {
				TriggerEntered = true;

				if (collider.gameObject != null && collider.gameObject.tag.ToLower().Contains("ball_") && !WaitingForExit) {
					WaitingForExit = true;
					collider.gameObject.SendMessage("EnteredGoal", SendMessageOptions.DontRequireReceiver);

					Rigidbody rbIncoming = collider.gameObject.GetComponent<Rigidbody>();

					if (rbIncoming && Vector3.Dot(rbIncoming.velocity, Vector3.up) < 0f) {
						EntryHeight = collider.gameObject.transform.position.y;
						EnteredFromAbove = true;
					}
				}
			}
		}

		public void OnTriggerExit(Collider collider) {
			if (TriggerEntered) {
				TriggerEntered = false;

				if (collider.gameObject != null && collider.gameObject.tag.ToLower().Contains("ball_")) {
					WaitingForExit = false;
					collider.gameObject.SendMessage("ExitedGoal", SendMessageOptions.DontRequireReceiver);

					Rigidbody rbIncoming = collider.gameObject.GetComponent<Rigidbody>();
					ExitHeight = collider.gameObject.transform.position.y;

					if (rbIncoming && Vector3.Dot(rbIncoming.velocity, Vector3.up) < 0f && ExitHeight < EntryHeight) {
						Switchboard.Broadcast("BallInGoal", collider.gameObject);
					}

					EnteredFromAbove = false;
				}
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private float EntryHeight {get; set;}
		private float ExitHeight {get; set;}

		private bool EnteredFromAbove {get; set;}
		private bool WaitingForExit {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();
			
			Assert.That(scoreParticle != null, "Score particle system not found!", gameObject);

			Switchboard.AddListener("PlayerScored", OnPlayerScored);
			Switchboard.AddListener("BallArmed", OnBallArmed);
		}

		// Coroutines /////////////////////////////////////////////////////////////

		// Message Handlers ///////////////////////////////////////////////////////
		public void OnPlayerScored(object ignored) {
			// scoreParticle.Play();
		}

		public void OnBallArmed(object ignored) {
			TriggerEntered = false;
		}
	}
}
