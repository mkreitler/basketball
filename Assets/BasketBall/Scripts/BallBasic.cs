using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	public class BallBasic : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		public enum eTypes {
			BASIC
		}

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private float netDragLateral = 0.95f;

		[SerializeField]
		private float netDragVertical = 0.33f;

		// Interface //////////////////////////////////////////////////////////////
		public float Mass {
			get {
				return RigidBody.mass;
			}
		}

		public Vector3 Position {
			get {
				return gameObject.transform.position;
			}
		}
		
		public void MakeKinematic() {
			RigidBody.isKinematic = true;
		}

		public void MakeDynamic() {
			RigidBody.isKinematic = false;
		}

		public void EnteredGoal() {
			DoDrag = true;
		}

		public void ExitedGoal() {
			DoDrag = false;
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

		public void Launch(Vector3 vImpulse, Vector3 vAngularImpulse) {
			RigidBody.isKinematic = false;
			RigidBody.AddForce(vImpulse, ForceMode.Impulse);
			RigidBody.AddTorque(vAngularImpulse, ForceMode.Impulse);
		}

		// Implementation /////////////////////////////////////////////////////////
		private Rigidbody RigidBody {get; set;}

		private bool DoDrag {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		protected void Awake() {
			RigidBody = gameObject.GetComponent<Rigidbody>();
			Assert.That(RigidBody != null, "Rigidbody component not found!", gameObject);
		}

		protected void OnEnable() {
			DoDrag = false;
		}

		protected virtual void Update() {
		}

		protected void FixedUpdate() {
			if (DoDrag) {
				Vector3 vVel = RigidBody.velocity;
				Vector3 vDrag = RigidBody.velocity * netDragLateral * Time.fixedDeltaTime;

				vDrag.z = vVel.z * netDragVertical * Time.fixedDeltaTime;

				RigidBody.AddForce(-vDrag * Mass, ForceMode.Impulse);

				RigidBody.AddTorque(-RigidBody.angularVelocity, ForceMode.VelocityChange);
			}
		}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
