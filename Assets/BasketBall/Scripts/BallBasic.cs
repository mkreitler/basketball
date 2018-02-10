using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using thinkagaingames.com.engine;

namespace thinkagaingames.com.basketball {
	public class BallBasic : MonoBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		public void MakeKinematic() {
			RigidBody.isKinematic = true;
		}

		public void MakeDynamic() {
			RigidBody.isKinematic = false;
		}

		public void MoveTo(Vector3 vPoint) {
			if (!RigidBody.isKinematic) {
				MakeKinematic();
			}

			gameObject.transform.position = vPoint;
		}

		public void Release() {
			MakeDynamic();
		}

		public void Launch(Vector3 vImpulse) {
			RigidBody.isKinematic = false;
			RigidBody.AddForce(vImpulse, ForceMode.Impulse);
		}

		// Implementation /////////////////////////////////////////////////////////
		private Rigidbody RigidBody {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		protected void Awake() {
			RigidBody = gameObject.GetComponent<Rigidbody>();
			Assert.That(RigidBody != null, "Rigidbody component not found!", gameObject);
		}
		// Coroutines /////////////////////////////////////////////////////////////
	}
}
