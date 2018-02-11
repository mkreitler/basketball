/**
 * This class provides hooks for pausing and ordered initialization.
 * It's meant to work in conjunction with the GameDirector class, which
 * maintains an ordered list of game entities to initialize before
 * the game starts.
 *
 * In order to take advantage of this initialization system, you must:
 * 1) Add pausables you want to initialize to the GameDirector's "initList" via the editor or a call to GameDirector.AddPausable().
 * 2) Ensure that the pausable's base Start() gets called (otherwise the Director will never call OnGameStart()).
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.thinkagaingames.engine {
	public class PausableBehaviour : MonoBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		public bool ReadyToStart {get; set;}

		// Implementation /////////////////////////////////////////////////////////
		// Interfaces /////////////////////////////////////////////////////////////
		protected virtual void Start() {
			ReadyToStart = true;
		}

		// Don't think we will need this...
		// protected virtual void AddToInitList() {
		// 	Assert.That(GameDirector.AddToInitList(this) == false, "Instance already in initList!", gameObject);
		// }

		public virtual void OnPauseGame() {}

		public virtual void OnResumeGame() {}

		public virtual void OnStartGame() {}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
