using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace thinkagaingames.com.engine {
	public class TouchPlane : MonoBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		private const float MAX_RAYCAST_DIST	= 20000f;

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		protected Camera worldCamera = null;

		// Interface //////////////////////////////////////////////////////////////

		public void Resize(Camera resizedCamera) {
			Assert.That(resizedCamera != null, "Invalid camera in Resize() call!", gameObject);

			Vector3 vCamForward = resizedCamera.transform.rotation * Vector3.forward;
			Vector3 vCamToPlane = gameObject.transform.position - resizedCamera.gameObject.transform.position;
			float camToPlaneDist = Vector3.Dot(vCamToPlane, vCamForward);

			Vector3 vViewportPoint = Vector3.zero;
			vViewportPoint.z = camToPlaneDist;

			vViewportPoint.x = 0f;
			vViewportPoint.y = 1f;
			vViewportPoint.x = 1f;
			Vector3 vUpperRight = resizedCamera.ViewportToWorldPoint(vViewportPoint);

			vViewportPoint.y = 0f;
			Vector3 vLowerRight = resizedCamera.ViewportToWorldPoint(vViewportPoint);

			vViewportPoint.x = 0f;
			Vector3 vLowerLeft = resizedCamera.ViewportToWorldPoint(vViewportPoint);

			Vector3 vNewScale = gameObject.transform.localScale;
			vNewScale.x = Mathf.Abs(vLowerRight.x - vLowerLeft.x);
			vNewScale.y = Mathf.Abs(vUpperRight.y - vLowerRight.y);

			worldExtentsVertical = vNewScale.y;
			worldExtentsHorizontal = vNewScale.x;

			gameObject.transform.localScale = vNewScale;
		}

		public virtual void OnFlickStart(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			TrackingFlick = true;
			this.vScreenPoint = vScreenPoint;
		}

		public virtual void OnFlickMove(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			if (TrackingFlick) {
				this.vScreenPoint = vScreenPoint;
			}
		}

		public virtual void OnFlickHold(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			if (TrackingFlick) {
				this.vScreenPoint = vScreenPoint;
			}
		}

		public virtual void OnFlickEnd(Vector2 vScreenPoint, Vector2 vViewportPoint) {
			if (TrackingFlick) {
				TrackingFlick = false;
				this.vScreenPoint = vScreenPoint;
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private Ray ray = new Ray();

		private bool TrackingFlick {get; set;}

		private Vector2 vScreenPoint = Vector2.zero;

		protected float worldExtentsVertical {get; set;}

		protected float worldExtentsHorizontal {get; set;}

		protected virtual void OnRayHit(RaycastHit hitInfo) {
			// Override this in child classes to do something interesting.
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected virtual void Awake() {
			Assert.That(worldCamera != null, "Undefined world camera!", gameObject);
		}

		protected virtual void Update() {
			if (TrackingFlick) {
				int layerMask = 1 << LayerMask.NameToLayer("touchplane");

				Vector3 vEnd = Vector3.zero;
				vEnd.x = vScreenPoint.x;
				vEnd.y = vScreenPoint.y;
				vEnd.z = MAX_RAYCAST_DIST;
				vEnd = worldCamera.ScreenToWorldPoint(vEnd);

				ray.origin = worldCamera.transform.position;
				ray.direction = (vEnd - ray.origin).normalized;

				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, MAX_RAYCAST_DIST, layerMask)) {
					OnRayHit(hitInfo);
				}
			}

			Debug.DrawRay(ray.origin, ray.direction, Color.red);
		}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
