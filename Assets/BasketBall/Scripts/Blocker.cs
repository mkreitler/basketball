/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       14/02/2018 09:44
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.GAME_NAME {
	public class Blocker : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private float period = 0.5f;

		// Interface //////////////////////////////////////////////////////////////
		// Implementation /////////////////////////////////////////////////////////
		private Quaternion qStart;
		private Quaternion qEnd;

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Start() {
			base.Start();

			qStart = Quaternion.identity * gameObject.transform.rotation;
			qEnd = Quaternion.AngleAxis(180f, Vector3.up) * gameObject.transform.rotation;
		}

		protected void OnEnable() {
			StartCoroutine("Block");
		}

		protected void OnDisable() {
			StopCoroutine("Block");
		}

		// Coroutines /////////////////////////////////////////////////////////////
		IEnumerator Block() {
			float timer = 0f;

			Assert.That(period > 0f, "Invalid rotation period!", gameObject);

			while (true) {
				timer += Time.fixedDeltaTime;
				float param = 1 - (1 + Mathf.Cos(2f * Mathf.PI * timer / (2f * period))) / 2;
				gameObject.transform.rotation = Quaternion.Slerp(qStart, qEnd, param);
				yield return new WaitForFixedUpdate();
			}
		}

		// Message Handlers ///////////////////////////////////////////////////////
	}
}
