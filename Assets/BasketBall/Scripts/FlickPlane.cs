using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using thinkagaingames.com.engine;

namespace thinkagaingames.com.basketball {
	public class FlickPlane : TouchPlane {
		// Types and Constants ////////////////////////////////////////////////////
		private const float EPSILON = 0.01f;

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private BallBasic ball = null;

		[SerializeField]
		private GameObject target = null;

		[SerializeField]
		private GameObject targetLow = null;

		[SerializeField]
		private GameObject targetHigh = null;

		[SerializeField]
		private float flightTime = 1f;
		
		[SerializeField]
		private float rotationalImpulse = 0f;

		[SerializeField]
		private float lowShotThreshold = 0.3f;
		
		[SerializeField]
		private float highShotThreshold = 0.6f;

		[SerializeField]
		private AnimationCurve lowShotCurve = null;

		[SerializeField]
		private AnimationCurve highShotCurve = null;

		[SerializeField]
		private float driftLeftThreshold = 0.015f;

		[SerializeField]
		private float driftRightThreshold = 0.4f;

		[SerializeField]
		private float lateralDriftScalar = 2f;

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
		protected Vector3 vLastRayHit {get; set;}

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

			vLastRayHit = hitInfo.point;
			vLastPosition = ball.Position;
		}

		private void ComputeFlightPath() {
			if (target != null && flightTime > 0f) {
				float aspectCompensator = (float)Screen.height / (float)Screen.width;
				float normalizedDy = vDisplacementAccumulator.y * aspectCompensator / worldExtentsVertical;
				float normalizedDx = vDisplacementAccumulator.x / worldExtentsHorizontal;

				// Assume a perfect shot.
				Vector3 vDelta = target.transform.position - ball.Position;
				Vector3 vVel = Vector3.zero;
				Vector3 vCamRight = worldCamera.transform.rotation * Vector3.right;
				Vector3 vRotImpulse = vCamRight * rotationalImpulse * Random.Range(-1f, 1f);

				// Debug.Log("NormDX: " + normalizedDx + "   NormDY: " + normalizedDy);

				if (Mathf.Abs(normalizedDy) > EPSILON) {
					if (normalizedDy < lowShotThreshold) {
						float param = Mathf.Max(normalizedDy, 0f) / lowShotThreshold;
						param = lowShotCurve.Evaluate(param);
						Vector3 vTargetPos = Vector3.Lerp(targetLow.transform.position, target.transform.position, param);
						vDelta = vTargetPos - ball.Position;
					}
					else if (normalizedDy > highShotThreshold) {
						float param = Mathf.Min(1f, (normalizedDy - highShotThreshold) / highShotThreshold);
						param = highShotCurve.Evaluate(param);
						Vector3 vTargetPos = Vector3.Lerp(target.transform.position, targetHigh.transform.position, param);
						vDelta = vTargetPos - ball.Position;
					}

					if (normalizedDx > driftLeftThreshold) {
						vDelta.x -= lateralDriftScalar * vCamRight.x * (normalizedDx - driftLeftThreshold) * (target.transform.position - ball.Position).magnitude;
					}
					else if (normalizedDx < -driftRightThreshold) {
						vDelta.x -= lateralDriftScalar * vCamRight.x * (normalizedDx + driftRightThreshold) * (target.transform.position - ball.Position).magnitude;
					}

					vVel = vDelta / flightTime;

					vVel.y = vDelta.y / flightTime - 0.5f * Physics.gravity.y * flightTime;

					ball.Launch(vVel * ball.Mass, vRotImpulse);
				}
				else {
					ball.Launch(Vector3.zero, vRotImpulse);
				}
			}
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();

			Assert.That(ball != null, "Invalid ball object!", gameObject);
			Assert.That(lowShotCurve != null, "Low shot curve not found!", gameObject);
			Assert.That(highShotCurve != null, "High short curve not found!", gameObject);
		}

		protected override void Update() {
			base.Update();
		}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
