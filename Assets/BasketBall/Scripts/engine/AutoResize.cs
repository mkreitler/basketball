using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace thinkagaingames.com.engine {
	[System.Serializable]
	public class ResizeEvent : UnityEvent<Camera> {
	}

	public class AutoResize : MonoBehaviour {
		// Types and Constants ////////////////////////////////////////////////////

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private float minWidth = 640;

		[SerializeField]
		private float minHeight = 960f;

		[SerializeField]
		private float maxWidth = 1125f;

		[SerializeField]
		private float maxHeight = 2436f;

		[SerializeField]
		private float minFov = 40f;

		[SerializeField]
		private float maxFov = 60f;

		[SerializeField]
		private ResizeEvent resizeEvents;

		// Interface //////////////////////////////////////////////////////////////
		public void OnEnable() {
			Camera = gameObject.GetComponent<Camera>();
			Assert.That(Camera != null, "No camera component found!", gameObject);

			ComputeFOV();
		}

		public void Update() {
			if (Screen.width != currentWidth || Screen.height != currentHeight) {
				ComputeFOV();
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private Camera Camera {get; set;}
		private float currentWidth = 0f;
		private float currentHeight = 0f;

		private void ComputeFOV() {
			currentWidth = Screen.width;
			currentHeight = Screen.height;

			Assert.That(minHeight > 0, "Invalid minHeight!", gameObject);
			Assert.That(maxHeight > 0, "Invalid maxHeight!", gameObject);
			Assert.That(currentHeight > 0, "Invalid currentHeight!", gameObject);

			float currentAspect = currentWidth / currentHeight;
			float minAspect = minWidth / minHeight;
			float maxAspect = maxWidth / maxHeight;

			float fovParam = (currentAspect - minAspect) / (maxAspect - minAspect);
			fovParam = Mathf.Clamp(fovParam, 0f, 1f);

			Camera.fieldOfView = minFov + (maxFov - minFov) * fovParam;

			resizeEvents.Invoke(Camera);
		}

		// Interfaces /////////////////////////////////////////////////////////////
		// Coroutines /////////////////////////////////////////////////////////////
	}
}
