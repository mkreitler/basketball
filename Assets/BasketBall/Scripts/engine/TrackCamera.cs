/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       15/02/2018 07:36
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.GAME_NAME {
	public class TrackCamera : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private Camera camToTrack = null;

		// Interface //////////////////////////////////////////////////////////////
		// Implementation /////////////////////////////////////////////////////////
		private void AlignToCamera() {
			Quaternion q = gameObject.transform.rotation;
			q.SetLookRotation(camToTrack.transform.position - gameObject.transform.position);
			gameObject.transform.rotation = q;
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();

			Assert.That(camToTrack != null, "Undefined camera!", gameObject);
		}

		protected void Update() {
			AlignToCamera();
		}

		// Coroutines /////////////////////////////////////////////////////////////

		// Message Handlers ///////////////////////////////////////////////////////
	}
}
