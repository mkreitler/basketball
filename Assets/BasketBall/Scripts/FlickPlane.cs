using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using thinkagaingames.com.engine;

namespace thinkagaingames.com.basketball {
	public class FlickPlane : TouchPlane {
		// Types and Constants ////////////////////////////////////////////////////

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private BallBasic ball = null;

		[SerializeField]
		private GameObject target = null;

		[SerializeField]
		private float flightTime = 1f;

		[SerializeField]
		private float rotationalImpulse = 0f;

		// Interface //////////////////////////////////////////////////////////////
		public override void OnFlickStart(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			if (TrackingTouch) {
				base.OnFlickStart(vScreenPoint, vViewportPoint);
				ball.MakeKinematic();
				ResetVelocityTracker = true;
				LaunchTime = 0f;
			}
		}

		public override void OnFlickHold(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			ResetVelocityTracker = true;
		}

		public override void OnFlickEnd(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			if (TrackingTouch) {
				base.OnFlickEnd(vScreenPoint, vViewportPoint);
				ball.MakeDynamic();
				ComputeFlightPath();
				TrackingTouch = false;
			}
		}

		public void TrackTouchStart() {
			TrackingTouch = true;
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
		protected bool TrackingTouch {get; set;}

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

		private void ComputeFlightPath() {
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
