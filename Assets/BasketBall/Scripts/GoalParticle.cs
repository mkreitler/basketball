/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       15/02/2018 06:52
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	public class GoalParticle : ParticleMaster {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		// Implementation /////////////////////////////////////////////////////////
		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Start() {
			base.Start();

			Switchboard.AddListener("PlayerScored", OnPlayerScored);
		}

		// Coroutines /////////////////////////////////////////////////////////////
		// Message Handlers ///////////////////////////////////////////////////////
		public void OnPlayerScored(object objBallPos) {
			Vector3 ballPosition = (Vector3)objBallPos;

			gameObject.transform.position = ballPosition;
			Play();
		}
	}
}
