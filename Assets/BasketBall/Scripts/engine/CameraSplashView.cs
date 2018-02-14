/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       13/02/2018 23:47
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.GAME_NAME {
	public class CameraSplashView : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private float finalDepth = 5f;

		// Interface //////////////////////////////////////////////////////////////
		// Implementation /////////////////////////////////////////////////////////
		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Start() {
			base.Start();

			Switchboard.AddListener("ShiftSplashCamera", ShiftSplashCamera);
		}

		// Coroutines /////////////////////////////////////////////////////////////
		// Message Handlers ///////////////////////////////////////////////////////
		private void ShiftSplashCamera(object ignored) {
			Camera camera = gameObject.GetComponent<Camera>();
			Assert.That(camera != null, "Camera component not found!", gameObject);

			camera.depth = finalDepth;
		}
		// Coroutines /////////////////////////////////////////////////////////////
		// Message Handlers ///////////////////////////////////////////////////////
	}
}
