/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       20/02/2018 19:58
 *****************************************************************************/
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.thinkagaingames.engine {
	public class Switchboard : MonoBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		public delegate void messageHandler(object args);

		private class ListenerInfo {
			public bool inUse;
			public string message;
			public messageHandler handler;

			public ListenerInfo(string message, messageHandler handler) {
				this.message = message;
				this.handler = handler;
				this.inUse = true;
			}
		}

		// Editor Variables ///////////////////////////////////////////////////////
		// Interface //////////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------
		public static Switchboard Instance = null;

		public static void Broadcast(string message, object objArg = null) {
			Assert.That(Instance != null, "(Broadcast) No switchboard instance found!");

			Instance._Broadcast(message, objArg);
		}

		public static void AddListener(string message, messageHandler handler) {
			Assert.That(Instance != null, "(AddListener) No switchboard instance found!");

			Instance._AddListener(message, handler);
		}

		public static void RemoveListener(string message, messageHandler handler) {
			Assert.That(Instance != null, "(RemoveListener) No switchboard instance found!");

			Instance._RemoveListener(message, handler);			
		}

		// Instance ---------------------------------------------------------------

		private void _Broadcast(string message, object objArg) {
			List<messageHandler> handlers = messageBoard[message.ToLower()] as List<messageHandler>;

			if (handlers != null) {
				for (int i=0; i<handlers.Count; ++i) {
					handlers[i](objArg);
				}
			}
		}

		private void _AddListener(string message, messageHandler handler) {
			bool bAdded = false;

			Assert.That(message != null && message.Length > 0 && handler != null, "Invalid listener parameters!", gameObject);

			for (int i=0; i<addList.Count; ++i) {
				if (!addList[i].inUse) {
					addList[i].message = message;
					addList[i].handler = handler;
					addList[i].inUse = true;
					bAdded = true;
					break;
				}
			}

			if (!bAdded) {
				addList.Add(new ListenerInfo(message, handler));
			}

			if (!IsAddListDirty) {
				IsAddListDirty = true;

				if (!IsRemoveListDirty) {
					StartCoroutine("UpdateListeners");
				}
			}			
		}

		private void _RemoveListener(string message, messageHandler handler) {
			bool bAdded = false;

			Assert.That(message != null && message.Length > 0 && handler != null, "Invalid removal parameters!", gameObject);

			for (int i=0; i<removeList.Count; ++i) {
				if (!removeList[i].inUse) {
					removeList[i].message = message;
					removeList[i].handler = handler;
					removeList[i].inUse = true;
					bAdded = true;
					break;
				}
			}

			if (!bAdded) {
				removeList.Add(new ListenerInfo(message, handler));
			}

			if (!IsRemoveListDirty) {
				IsRemoveListDirty = true;

				if (!IsAddListDirty) {
					StartCoroutine("UpdateListeners");
				}
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private bool IsRemoveListDirty {get; set;}

		private bool IsAddListDirty {get; set;}

		private Hashtable messageBoard = new Hashtable();

		private List<ListenerInfo> removeList = new List<ListenerInfo>();

		private List<ListenerInfo> addList = new List<ListenerInfo>();

		private void RemoveHandlerFromTable(string message, messageHandler handler) {
			Assert.That(message != null && message.Length > 0 && handler != null, "RemoveHandlerFromTable -- Invalid parameters!", gameObject);

			message = message.ToLower();
			List<messageHandler>handlers = messageBoard[message] as List<messageHandler>;
			if (handlers != null) {
				handlers.Remove(handler);
			}
		}

		private void AddHandlerToTable(string message, messageHandler handler) {
			Assert.That(message != null && message.Length > 0 && handler != null, "AddHandlerToTable -- Invalid parameters!", gameObject);

			message = message.ToLower();
			List<messageHandler> handlers = messageBoard[message] as List<messageHandler>;

			if (handlers == null) {
				handlers = new List<messageHandler>();
				messageBoard[message] = handlers;
			}

			handlers.Add(handler);
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected void Awake() {
			Assert.That(Instance == null, "(Awake) Multiple Switchboards detected!", gameObject);

			Instance = this;
		}

		protected void OnDestroy() {
			Instance = null;
		}

		// Coroutines /////////////////////////////////////////////////////////////
		IEnumerator UpdateListeners() {
			yield return new WaitForEndOfFrame();

			if (IsAddListDirty) {
				for (int i=0; i<addList.Count; ++i) {
					if (addList[i].inUse) {
						AddHandlerToTable(addList[i].message, addList[i].handler);
						addList[i].inUse = false;
					}
				}
			}

			if (IsRemoveListDirty) {
				for (int i=0; i<removeList.Count; ++i) {
					if (removeList[i].inUse) {
						RemoveHandlerFromTable(removeList[i].message, removeList[i].handler);
						removeList[i].inUse = false;
					}
				}
			}

			IsAddListDirty = false;
			IsRemoveListDirty = false;
		}
	}
}
