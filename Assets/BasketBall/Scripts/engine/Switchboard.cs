using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace thinkagaingames.com.engine {
	public class Switchboard : MonoBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		public delegate bool messageHandler(object args);

		private class RemoveInfo {
			public bool inUse;
			public string message;
			public messageHandler handler;

			public RemoveInfo(string message, messageHandler handler) {
				this.message = message;
				this.handler = handler;
				this.inUse = false;
			}
		}

		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------
		public static Switchboard SWITCHBOARD = null;

		// Instance ---------------------------------------------------------------
		public void Awake() {
			Assert.That(SWITCHBOARD == null, "(Awake) Multiple Switchboards detected!", gameObject);

			SWITCHBOARD = this;
		}

		public void OnDestroy() {
			Assert.That(SWITCHBOARD != null, "(Destroy) ultiple Switchboards detected!", gameObject);
			
			SWITCHBOARD = null;
		}

		public void AddListener(string message, messageHandler handler) {
			Assert.That(message != null && message.Length > 0 && handler != null, "Invalid listener parameters!", gameObject);

			message = message.ToLower();
			List<messageHandler> handlers = messageBoard[message] as List<messageHandler>;

			if (handlers == null) {
				handlers = new List<messageHandler>();
			}

			handlers.Add(handler);
			messageBoard[message] = handlers;
		}

		public void RemoveListener(string message, messageHandler handler) {
			bool bAdded = false;

			Assert.That(message != null && message.Length > 0 && handler != null, "Switchboard::RemoveInfo -- Invalid removal parameters!");

			IsRemoveListDirty = true;

			for (int i=0; i<removeList.Count; ++i) {
				if (!removeList[i].inUse) {
					removeList[i].message = message;
					removeList[i].handler = handler;
					removeList[i].inUse = true;
					bAdded = true;
				}
			}

			if (!bAdded) {
				removeList.Add(new RemoveInfo(message, handler));
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private bool IsRemoveListDirty {get; set;}

		private Hashtable messageBoard = new Hashtable();

		private List<RemoveInfo> removeList = new List<RemoveInfo>();

		private void RemoveHandlerFromTable(string message, messageHandler handler) {
			Assert.That(message != null && message.Length > 0 && handler != null, "Switchboard::RemoveHandlerFromTable -- Invalid removal parameters!");

			message = message.ToLower();
			List<messageHandler>handlers = messageBoard[message] as List<messageHandler>;
			if (handlers != null) {
				handlers.Remove(handler);
			}
		}

		// Interfaces /////////////////////////////////////////////////////////////
		// Coroutines /////////////////////////////////////////////////////////////
		IEnumerator RemoveListeners() {
			while (true) {
				if (IsRemoveListDirty) {
					for (int i=0; i<removeList.Count; ++i) {
						if (removeList[i].inUse) {
							RemoveHandlerFromTable(removeList[i].message, removeList[i].handler);
							removeList[i].inUse = false;
						}
					}
				}

				yield return new WaitForEndOfFrame();
			}
		}
	}
}
