/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       10/03/2018 19:15
 *****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	public class A2Ustart : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		AndroidJavaClass _class;
		AndroidJavaObject Instance { get { return _class.GetStatic<UnityEngine.AndroidJavaObject>("a2uInstance"); } }

		public void ForceQuit() {
			Application.Quit();
		}

		public override void OnStartGame() {
			Switchboard.AddListener("SetScore", OnSetScore);

#if UNITY_ANDROID && !UNITY_EDITOR
			if (_class != null) {
				_class.CallStatic("UnityDidCompleteSetup");
			}
#endif
		}

		protected void OnSetScore(object objScore) {
#if UNITY_ANDROID && !UNITY_EDITOR
			object[] args = {objScore};

			AndroidJavaObject a2u = Instance;
			Assert.That(a2u != null, "No Android plug-in found!", gameObject);
			a2u.Call("setLastScore", args);
#endif
		}

		protected override void Start()
		{
			base.Start();

#if UNITY_ANDROID && !UNITY_EDITOR
			// Start plugin.
			_class = new UnityEngine.AndroidJavaClass("com.thinkagaingames.integratedbasketball.AndroidToUnity");
			_class.CallStatic("createInstance");
#endif
		}

		// Implementation /////////////////////////////////////////////////////////
		// Interfaces /////////////////////////////////////////////////////////////
		// Coroutines /////////////////////////////////////////////////////////////
		// Message Handlers ///////////////////////////////////////////////////////
	}
}
