using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using thinkagaingames.com.engine;

namespace thinkagaingames.com.basketball {
	public class FlickPlane : TouchPlane {
		// Types and Constants ////////////////////////////////////////////////////
		[System.Serializable]
		protected class ShotProfile {
		public float lateralImpulse = 25f;
			public float verticalImpulse = 20f;
			public float forwardImpulse = 2f;
			public float rotationalImpulse = -30f;
		}

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private BallBasic ball = null;

		[SerializeField]
		private List<ShotProfile> shots = null;

		[SerializeField]
		private int shotIndex = -1;

		[SerializeField]
		private GameObject target = null;

		[SerializeField]
		private float flightTime = 1f;

		[SerializeField]
		private float rotationalImpulse = 0f;

		// Interface //////////////////////////////////////////////////////////////
		public override void OnFlickStart(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			base.OnFlickStart(vScreenPoint, vViewportPoint);
			ball.MakeKinematic();
			ResetVelocityTracker = true;
			LaunchTime = 0f;
		}

		public override void OnFlickHold(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			ResetVelocityTracker = true;
		}

		public override void OnFlickEnd(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			base.OnFlickEnd(vScreenPoint, vViewportPoint);
			ball.MakeDynamic();
			AttemptTwo();
		}

		// Implementation /////////////////////////////////////////////////////////
		protected bool ResetVelocityTracker {get; set;}
		protected Vector3 vLastBallPos {get; set;}
		protected Vector3 vDisplacementAccumulator {get; set;}
		protected Vector3 vCurlAccumulator {get; set;}
		protected Vector3 vLastPosition {get; set;}
		protected Vector3 vLastDirection {get; set;}
		protected float LaunchTime {get; set;}
		protected float CurlTimeCorrection {get; set;}
		protected ShotProfile CurrentProfile {
			get {
				return shotIndex >= 0 && shotIndex < shots.Count ? shots[shotIndex] : null;
			}
		}

		protected override void OnRayHit(RaycastHit hitInfo) {
			ball.MoveTo(hitInfo.point);

			if (ResetVelocityTracker) {
				ResetVelocityTracker = false;
				vLastDirection = Vector3.zero;
				vDisplacementAccumulator = Vector3.zero;
				vCurlAccumulator = Vector3.zero;
				LaunchTime = 0f;
			}
			else {
				Vector3 vDisplacement = ball.Position - vLastPosition;
				vDisplacementAccumulator += vDisplacement;

				vDisplacement = vDisplacement.normalized;

				if (vLastDirection != Vector3.zero) {
					vCurlAccumulator = Vector3.Cross(vDisplacement, vLastDirection);
				}
				else {
					CurlTimeCorrection = Time.deltaTime;
				}

				LaunchTime += Time.deltaTime;

				vLastDirection = vDisplacement;
			}

			vLastPosition = ball.Position;
		}

		private void AttemptOne() {
			if (CurrentProfile != null) {
				ResetVelocityTracker = false;

				Vector3 vCamForward = worldCamera.gameObject.transform.rotation * Vector3.forward;
				Vector3 vCamRight = worldCamera.gameObject.transform.rotation * Vector3.right;
				Vector3 vCamUp = worldCamera.gameObject.transform.rotation * Vector3.up;

				float accumDispX = Vector3.Dot(vCamRight, vDisplacementAccumulator);
				float accumDispY = Vector3.Dot(vCamUp, vDisplacementAccumulator);

				Vector3 vLaunchImpulse = vCamForward * CurrentProfile.forwardImpulse +
										vCamRight * CurrentProfile.lateralImpulse +
										vCamUp * CurrentProfile.verticalImpulse;

				Vector3 vLaunchRotImpulse = vCamRight * CurrentProfile.rotationalImpulse;

				ball.Launch(vLaunchImpulse, vLaunchRotImpulse);

				// TODO: compute spin on ball by finding average curl = Vector3.Dot(vCurlAccumulator, vCameraForward) / (LaunchTime - CurlCorrectionTime);
			}
		}

		private void AttemptTwo() {
			if (target != null && flightTime > 0f) {
				Vector3 vDelta = target.transform.position - ball.Position;
				Vector3 vVel = vDelta / flightTime;

				vVel.y = vDelta.y / flightTime - 0.5f * Physics.gravity.y * flightTime;

				Vector3 vCamRight = worldCamera.transform.rotation * Vector3.right;
				Vector3 vRotImpulse = vCamRight * rotationalImpulse;

				ball.Launch(vVel * ball.Mass, vRotImpulse);
			}
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();

			Assert.That(ball != null, "Invalid ball object!", gameObject);
		}

		protected override void Update() {
			base.Update();
		}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
