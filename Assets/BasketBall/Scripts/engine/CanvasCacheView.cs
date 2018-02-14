/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       14/02/2018 00:44
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.GAME_NAME {
	public class CanvasCacheView : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		private const float MAX_RES = 100000f;

		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		// Implementation /////////////////////////////////////////////////////////
		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Start() {
			base.Start();

			Switchboard.AddListener("ExpandCanvas", ExpandCanvas);
			Switchboard.AddListener("RestoreCanvas", RestoreCanvas);
		}

		// Coroutines /////////////////////////////////////////////////////////////
		private UnityEngine.UI.CanvasScaler.ScreenMatchMode screenMatchMode;
		private float matchWidthOrHeight = 0;
		private Vector2 referenceResolution = Vector2.zero;

		// Message Handlers ///////////////////////////////////////////////////////

		private void ExpandCanvas(object ignored) {
			CanvasScaler scalar = gameObject.GetComponent<CanvasScaler>();
			Assert.That(scalar != null, "CanvasScalar component not found!", gameObject);

			screenMatchMode = scalar.screenMatchMode;
			matchWidthOrHeight = scalar.matchWidthOrHeight;
			referenceResolution = scalar.referenceResolution;

			scalar.screenMatchMode = 0;
			Vector2 refRes = scalar.referenceResolution;
			refRes.x = MAX_RES;
			refRes.y = MAX_RES;
			scalar.referenceResolution = refRes;
		}

		private void RestoreCanvas(object ignored) {
			CanvasScaler scalar = gameObject.GetComponent<CanvasScaler>();
			Assert.That(scalar != null, "CanvasScalar component not found!", gameObject);

			scalar.screenMatchMode = screenMatchMode;
			scalar.matchWidthOrHeight = matchWidthOrHeight;
			scalar.referenceResolution = referenceResolution;
		}
	}
}
