﻿using System.Collections;
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
		// Static -----------------------------------------------------------------

		// Instance ---------------------------------------------------------------
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

		public void StopPhysics() {
			MakeKinematic();
			RigidBody.velocity = Vector3.zero;
		}
		
		public void MakeKinematic() {
			RigidBody.isKinematic = true;
		}

		public void MakeDynamic() {
			Armed = true;
			Switchboard.Broadcast("BallArmed", null);
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
		protected bool Armed {get; set;}

		protected Rigidbody RigidBody {get; set;}

		private bool DoDrag {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();

			RigidBody = gameObject.GetComponent<Rigidbody>();
			Assert.That(RigidBody != null, "Rigidbody component not found!", gameObject);

			Switchboard.AddListener("BallInGoal", OnBallInGoal);
		}

		protected void OnEnable() {
			DoDrag = false;
		}

		protected virtual void Update() {
		}

		protected virtual void FixedUpdate() {
			if (DoDrag) {
				Vector3 vVel = RigidBody.velocity;
				Vector3 vDrag = RigidBody.velocity * netDragLateral * Time.fixedDeltaTime;

				vDrag.z = vVel.z * netDragVertical * Time.fixedDeltaTime;

				RigidBody.AddForce(-vDrag * Mass, ForceMode.Impulse);

				RigidBody.AddTorque(-RigidBody.angularVelocity, ForceMode.VelocityChange);
			}
		}

		protected void OnTriggerEnter(Collider collider) {
			string otherTag = collider != null && collider.gameObject != null ? collider.gameObject.tag : null;

			if (otherTag != null) {
				otherTag = otherTag.ToLower();

				if (Armed) {
					if (otherTag == "miss") {
						Armed = false;
						Switchboard.Broadcast("PlayerMissed", null);
					}
				}
			}
		}

		// Coroutines /////////////////////////////////////////////////////////////

		// Message Handlers ///////////////////////////////////////////////////////
		public void OnBallInGoal(object objBallGameObject) {
			GameObject goBall = objBallGameObject as GameObject;

			if (goBall == gameObject && Armed) {
				Armed = false;
				Switchboard.Broadcast("PlayerScored", null);
			}
		}
	}
}
