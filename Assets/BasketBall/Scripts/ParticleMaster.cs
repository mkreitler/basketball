using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	public class ParticleMaster : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		public void Play() {
			if (particleSystems != null) {
				for (int i=0; i<particleSystems.Length; ++i) {
					particleSystems[i].Play();
				}
			}
		}

		public void Stop() {
			if (particleSystems != null) {
				for (int i=0; i<particleSystems.Length; ++i) {
					particleSystems[i].Stop();
				}
			}
		}

		public void Pause() {
			if (particleSystems != null) {
				for (int i=0; i<particleSystems.Length; ++i) {
					particleSystems[i].Pause();
				}
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		protected ParticleSystem[] particleSystems = null;

		// Interfaces /////////////////////////////////////////////////////////////
		protected void Awake() {
			particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>(true);
		}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
