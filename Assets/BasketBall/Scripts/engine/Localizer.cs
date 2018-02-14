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
		public void Refresh() {
			if (Localized) {
				LocalText.text = StringTable.GetString(Key);
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private bool Localized {get; set;}

		private Text LocalText {get; set;}

		private string Key {get; set;}

		// Interfaces /////////////////////////////////////////////////////////////
		protected void OnEnable() {
			Refresh();
		}

		public override void OnStartGame() {
			if (!Localized) {
				LocalText = gameObject.GetComponent<Text>();
				Assert.That(LocalText != null, "Text component not found!", gameObject);

				base.OnStartGame();

				Key = LocalText.text;
				Localized = true;
			}

			Refresh();
		}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
