using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using thinkagaingames.com.engine;

namespace thinkagaingames.com.basketball {
	public class FlickPlane : TouchPlane {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private BallBasic ball = null;

		// Interface //////////////////////////////////////////////////////////////
		public override void OnFlickStart(Vector2 vScreenPoint) {
			base.OnFlickStart(vScreenPoint);
			ball.MakeKinematic();
		}

		public override void OnFlickEnd(Vector2 vScreenPoint) {
			base.OnFlickEnd(vScreenPoint);
			ball.MakeDynamic();
		}
		
		// Implementation /////////////////////////////////////////////////////////
		protected override void OnRayHit(RaycastHit hitInfo) {
			ball.MoveTo(hitInfo.point);
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();

			Assert.That(ball != null, "Invalid ball object!", gameObject);
		}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
