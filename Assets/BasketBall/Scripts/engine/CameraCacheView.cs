/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       13/02/2018 23:46
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.thinkagaingames.engine {
	public class CameraCacheView : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		// Implementation /////////////////////////////////////////////////////////
		private float size = 0f;
		private Camera Camera {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Start() {
			base.Start();

			Switchboard.AddListener("WidenAllViews", WidenView);
			Switchboard.AddListener("RestoreAllViews", RestoreView);
		}

		// Coroutines /////////////////////////////////////////////////////////////
		// Message Handlers ///////////////////////////////////////////////////////

		private void WidenView(object ignored) {
			Camera = gameObject.GetComponent<Camera>();
			Assert.That(Camera != null, "Camera component not found!", gameObject);

			if (!Camera.orthographic) {
				size = Camera.fieldOfView;
				Camera.fieldOfView = 179f;
			}
		}

		private void RestoreView(object ignored) {
			Camera = gameObject.GetComponent<Camera>();
			Assert.That(Camera != null, "Camera component not found!", gameObject);

			if (!Camera.orthographic) {
				Camera.fieldOfView = size;
			}
		}
	}
}
