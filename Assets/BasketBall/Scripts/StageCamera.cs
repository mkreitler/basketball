using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	public class StageCamera : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		
		// Interface //////////////////////////////////////////////////////////////
		// Implementation /////////////////////////////////////////////////////////
		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Start() {
			base.Start();

			Switchboard.AddListener("InitStage", OnInitStage);
		}

		// Coroutines /////////////////////////////////////////////////////////////
		// Message Handlers ///////////////////////////////////////////////////////
		private void OnInitStage(object objStageParams) {
			StageParameters p = objStageParams as StageParameters;

			Assert.That(p != null, "Invalid stage parameters!", gameObject);
			Assert.That(p.cameraProxy != null, "Invalid camera proxy!", gameObject);
			
			gameObject.transform.position = p.cameraProxy.transform.position;
			gameObject.transform.rotation = p.cameraProxy.transform.rotation;
		}
	}
}
