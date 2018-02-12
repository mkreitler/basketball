using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.engine {
	public class Localizer : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		// Implementation /////////////////////////////////////////////////////////
		private bool Localized {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		protected void OnEnable() {
			if (!Localized) {
				Text localText = gameObject.GetComponent<Text>();

				base.OnStartGame();

				if (localText != null) {
					localText.text = StringTable.GetString(localText.text);
				}

				Localized = true;
			}
		}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
