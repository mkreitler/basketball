using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {

	public class FlickPlane : TouchPlane {
		// Types and Constants ////////////////////////////////////////////////////
		private const float EPSILON = 0.01f;

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private float flightTime = 1f;

		[SerializeField]
		private float lowShotThreshold = 0.3f;
		
		[SerializeField]
		private float highShotThreshold = 0.6f;

		[SerializeField]
		private AnimationCurve lowShotCurve = null;

		[SerializeField]
		private AnimationCurve highShotCurve = null;

		[SerializeField]
		private float driftThreshold = 2.5f;

		[SerializeField]
		private float lateralDriftScalar = 2f;

		[SerializeField]
		private GameObject target = null;

		[SerializeField]
		private GameObject targetLow = null;

		[SerializeField]
		private GameObject targetHigh = null;
		
		[SerializeField]
		private float rotationalImpulse = 0f;

		[SerializeField]
		private RectTransform touchZoneTransform = null;

		[SerializeField]
		private float viewportDistZ = 5f;

		// Interface //////////////////////////////////////////////////////////////
		public override void OnFlickStart(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			if (TrackingTouch && gameObject.activeSelf) {
				base.OnFlickStart(vScreenPoint, vViewportPoint);
				ball.MakeKinematic();
				ResetVelocityTracker = true;
				LaunchTime = 0f;
				Switchboard.Broadcast("FlickStart", null);
			}
		}

		public override void OnFlickHold(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			ResetVelocityTracker = true;
		}

		public override void OnFlickEnd(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			if (TrackingTouch && gameObject.activeSelf) {
				base.OnFlickEnd(vScreenPoint, vViewportPoint);
				ball.MakeDynamic();
				ComputeFlightPath();
				// ComputeFlightPathV2();
				TrackingTouch = false;
				Switchboard.Broadcast("FlickEnd", null);
				Switchboard.Broadcast("RequestNextBall");
			}
		}

		public void Hide() {
			Image image = gameObject.GetComponent<Image>();
			Assert.That(image != null, "Image component not found!", gameObject);

			Color color = image.color;
			color.a = 0f;
			image.color = color;
		}

		public void TrackTouchStart() {
			TrackingTouch = true;
		}

		// Implementation /////////////////////////////////////////////////////////
		protected BallBasic ball = null;
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

		protected Vector3 FirstHitPoint {get; set;}
		protected Vector3 LastHitPoint {get; set;}

		protected override void OnRayHit(RaycastHit hitInfo) {
			ball.MoveTo(hitInfo.point);

			if (ResetVelocityTracker) {
				ResetVelocityTracker = false;
				vLastDirection = Vector3.zero;
				vDisplacementAccumulator = Vector3.zero;
				vCurlAccumulator = Vector3.zero;
				LaunchTime = 0f;
				FirstHitPoint = hitInfo.point;
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
				LastHitPoint = hitInfo.point;
			}

			vLastRayHit = hitInfo.point;
			vLastPosition = ball.Position;
		}

		// Ray afterRay;
		// Ray beforeRay;
		// Ray velRay;

		private void ComputeFlightPathV2() {
			Vector3 screenPosStart = worldCamera.WorldToScreenPoint(FirstHitPoint);
			Vector3 screenPosEnd = worldCamera.WorldToScreenPoint(LastHitPoint);

			float swipeDistance = (screenPosEnd - screenPosStart).magnitude / Screen.dpi;

			Debug.Log(">>> Swipe Dist: " + swipeDistance);
		}

		private void ComputeFlightPath() {
			if (target != null && flightTime > 0f) {
				float aspectCompensator = (float)Screen.height / (float)Screen.width;
				float normalizedDy = vDisplacementAccumulator.y * Mathf.Sqrt(aspectCompensator) / worldExtentsVertical;

				// Assume a perfect shot.
				Vector3 vDelta = target.transform.position - ball.Position;
				Vector3 vVel = Vector3.zero;
				Vector3 vCamRight = worldCamera.transform.rotation * Vector3.right;
				Vector3 vRotImpulse = vCamRight * rotationalImpulse * Random.Range(-1f, 1f);
				Vector3 vLocalForward = worldCamera.transform.rotation * Vector3.forward;

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

					// Compute lateral accuracy.
					Vector3 touchZoneOriginViewport = uiCamera.WorldToViewportPoint(touchZoneTransform.position);
					touchZoneOriginViewport.z = viewportDistZ;
					
					Vector3 touchZoneWorldPoint = worldCamera.ViewportToWorldPoint(touchZoneOriginViewport);
					Vector3 vOriginToTarget = target.transform.position - touchZoneWorldPoint;

					// Project the ideal swipe path onto the screen.
					vOriginToTarget = vOriginToTarget - Vector3.Dot(vOriginToTarget, vLocalForward) * vLocalForward;

					// Project the player's total path onto the screen.
					Vector3 vPlayerPath2D = vDisplacementAccumulator - Vector3.Dot(vDisplacementAccumulator, vLocalForward) * vLocalForward;

					vOriginToTarget.Normalize();
					vPlayerPath2D.Normalize();

					float deviationDot = Vector3.Dot(vOriginToTarget, vPlayerPath2D);
					float angularDeviation = Mathf.Acos(Mathf.Clamp(deviationDot, -1f, 1f)) * 180f / Mathf.PI;
					float deviationDirection = Vector3.Dot(Vector3.Cross(vOriginToTarget, vPlayerPath2D), vLocalForward) > 0f ? -1 : 1; // Negative means drifting left. Positive means driving right. Magnitude of 1 means swipe was perpendicular to the desired path.

					// beforeRay.origin = ball.transform.position;
					// beforeRay.direction = vDelta.normalized;

					if (angularDeviation > driftThreshold * 0.5f) {
						Quaternion qRot = Quaternion.AngleAxis(deviationDirection * angularDeviation * lateralDriftScalar, Vector3.up);
						vDelta = qRot * vDelta;
					}

					// afterRay.origin = ball.transform.position;
					// afterRay.direction = vDelta.normalized;

					vVel = vDelta / flightTime;

					vVel.y = vDelta.y / flightTime - 0.5f * Physics.gravity.y * flightTime;
					
					// velRay.origin = ball.Position;
					// velRay.direction = vVel.normalized;

					ball.Launch(vVel * ball.Mass, vRotImpulse);
				}
				else {
					ball.Launch(Vector3.zero, vRotImpulse);
				}
			}
		}

		private void SetBall(object ballObj) {
			GameObject goBall = ballObj as GameObject;
			ball = goBall != null ? goBall.GetComponent<BallBasic>() : null;

			Assert.That(ball != null, "Failed to set ball!", gameObject);
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();

			Assert.That(lowShotCurve != null, "Low shot curve not found!", gameObject);
			Assert.That(highShotCurve != null, "High short curve not found!", gameObject);
		}

		protected override void Update() {
			// Debug.DrawRay(beforeRay.origin, beforeRay.direction, Color.green);
			// Debug.DrawRay(afterRay.origin, afterRay.direction, Color.blue);
			// Debug.DrawRay(velRay.origin, velRay.direction, Color.yellow);

			base.Update();
		}

		protected override void Start() {
			base.Start();

			Switchboard.AddListener("SetBall", SetBall);
			Switchboard.AddListener("InitStage", OnInitStage);
			Switchboard.AddListener("BeginRound", OnBeginRound);
			Switchboard.AddListener("EndRound", OnEndRound);
			Switchboard.AddListener("EnableTouchInput", OnEnableTouchInput);
			Switchboard.AddListener("DisableTouchInput", OnDisableTouchInput);
		}

		public override void OnStartGame() {
			Assert.That(ball != null, "Invalid ball object!", gameObject);
			Assert.That(touchZoneTransform != null, "Touch zone transform not defined!", gameObject);

			gameObject.SetActive(false);
		}

		// Coroutines /////////////////////////////////////////////////////////////

		// Message Handlers ///////////////////////////////////////////////////////
		private void OnBeginRound(object boolIsTutorial) {
			bool isTutorial = (bool)boolIsTutorial;

			if (!isTutorial) {
				gameObject.SetActive(true);
			}
		}

		private void OnEndRound(object ignored) {
			// Anything to do, here?
		}

		private void OnEnableTouchInput(object objStr) {
			gameObject.SetActive(true);
		}

		private void OnDisableTouchInput(object ignored) {
			gameObject.SetActive(false);
		}

		private void OnInitStage(object objParams) {
			StageParameters p = objParams as StageParameters;
			Assert.That(p != null, "Invalid stage parameters!", gameObject);

			flightTime = p.flightTime;
			lowShotThreshold = p.lowShotThreshold;
			highShotThreshold = p.highShotThreshold;
			lowShotCurve = p.lowShotCurve;
			highShotCurve = p.highShotCurve;
			driftThreshold = p.driftThreshold;
			lateralDriftScalar = p.lateralDriftScalar;
			rotationalImpulse = p.rotationalImpulse;
			viewportDistZ = p.viewportDistZ;
		}
	}
}
