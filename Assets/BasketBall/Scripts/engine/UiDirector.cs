using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.thinkagaingames.engine {
	public class UiDirector : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		[System.Serializable]
		public class TransitionEndEvent : UnityEvent<TransitionGroup> {}

		[System.Serializable]
		public class TransitionGroup {
			public string name = null;
			public List<UiPanel> panels = null;
			public TransitionEndEvent onTransitionInComplete = null;
			public TransitionEndEvent onTransitionOutComplete = null;
		}

		private enum eTransDirection {
			UNKNOWN,
			IN,
			OUT
		}

		private class TransitionRecord {
			public TransitionGroup group = null;
			public eTransDirection direction = eTransDirection.UNKNOWN;
			public int completionsRemaining = 0;
			public bool wantsRemove = false;

			public bool RegisterCompletion(string name) {
				name = name.ToLower();

				for (int i=0; i<group.panels.Count; ++i) {
					if (group.panels[i].name.ToLower() == name) {
						completionsRemaining -= 1;
						break;
					}
				}
				return completionsRemaining == 0;
			}
		}

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private Camera uiCamera = null;

		[SerializeField]
		private List<TransitionGroup> transitionGroups = null;

		// Interface //////////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------		
		public static UiDirector Instance = null;

		public static Camera Camera {
			get {
				Assert.That(Instance != null, "(UIDirector) No UiDirector in scene!");
				return Instance != null ? Instance.uiCamera : null;
			}
		}

		private static Hashtable panels = new Hashtable();

		public static void RegisterPanel(string newPanelName, UiPanel panel) {
			newPanelName = newPanelName.ToLower();

			Assert.That(!panels.Contains(newPanelName), "UiPanel::UiPanel name not unique!");
			panels[newPanelName] = panel;
		}

		public static UiPanel GetPanel(string name) {
			return (name != null && name.Length > 0) ? panels[name.ToLower()] as UiPanel : null;
		}

		// Instance ---------------------------------------------------------------
		public void StartTransition(string groupName, bool isTransitionIn) {
			TransitionGroup group = GetTransitionGroup(groupName);

			Assert.That(group != null, "Unknown transition group!", gameObject);

			TransitionRecord record = GetUnusedTransitionRecord();
			record.group = group;
			record.direction = isTransitionIn ? eTransDirection.IN : eTransDirection.OUT;
			record.wantsRemove = false;
			transitionHistory.Insert(0, record);			

			StartPanelTransitions(record);
		}

		public void UndoMostRecentTransition() {
			if (transitionHistory.Count > 0) {
				TransitionRecord record = transitionHistory[0];
				Assert.That(record != null, "Invalid transition history!", gameObject);
				record.direction = record.direction == eTransDirection.IN ? eTransDirection.OUT : eTransDirection.IN;
				record.wantsRemove = true;

				StartPanelTransitions(record);
			}
		}

		public void ResetHistory() {
			while (transitionHistory.Count > 0) {
				unusedRecords.Add(transitionHistory[0]);
				transitionHistory.RemoveAt(0);
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private List<TransitionRecord> unusedRecords = new List<TransitionRecord>();

		private List<TransitionRecord> transitionHistory = new List<TransitionRecord>();

		private TransitionRecord GetUnusedTransitionRecord() {
			TransitionRecord record = null;

			if (unusedRecords.Count > 0) {
				record = unusedRecords[0];
				unusedRecords.RemoveAt(0);
			}
			else {
				record = new TransitionRecord();
			}

			return record;
		}

		private TransitionGroup GetTransitionGroup(string name) {
			TransitionGroup group = null;

			name = name.ToLower();

			for (int i=0; i<transitionGroups.Count; ++i) {
				Assert.That(transitionGroups[i].name != null, "Invalid transition group name!", gameObject);
				if (transitionGroups[i].name.ToLower() == name) {
					group = transitionGroups[i];
					break;
				}
			}

			return group;
		}

		private void PopTransitionHistory() {
			if (transitionHistory.Count > 0) {
				TransitionRecord topRecord = transitionHistory[0];
				transitionHistory.RemoveAt(0);
				unusedRecords.Add(topRecord);
			}
		}

		private void StartPanelTransitions(TransitionRecord record) {
			Assert.That(record != null, "Invalid transition record!", gameObject);

			TransitionGroup group = record.group;
			Assert.That(group != null, "Invalid transition group!", gameObject);

			Assert.That(record.completionsRemaining == 0, "Non-zero transition completion count!", gameObject);

			for (int i=0; i<group.panels.Count; ++i) {
				if (record.direction == eTransDirection.IN) {
					group.panels[i].TransitionIn();
				}
				else {
					group.panels[i].TransitionOut();
				}

				record.completionsRemaining += 1;
			}
		}
		
		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();
			
			Assert.That(Instance == null, "Multiple UiDirectors found!", gameObject);
			Instance = this;

			Assert.That(uiCamera != null, "UI Camera not set!", gameObject);
		}

		protected override void Start() {
			base.Start();

			Switchboard.AddListener("TransitionComplete", OnTransitionComplete);
		}

		protected virtual void OnDestroy() {
			Instance = null;
		}

		// Coroutines /////////////////////////////////////////////////////////////

		// Message Handlers ///////////////////////////////////////////////////////
		public bool OnTransitionComplete(object objPanelName) {
			string panelName = objPanelName as string;

			Assert.That(panelName != null && panelName.Length > 0, "Invalid panel name!", gameObject);
			Assert.That(transitionHistory.Count > 0, "Invalid transtion history!", gameObject);

			TransitionRecord lastTransition = transitionHistory[0];
			
			if (lastTransition.RegisterCompletion(panelName)) {
				// All panels have transitioned.
				if (lastTransition.wantsRemove) {
					transitionHistory.RemoveAt(0);
				}

				if (lastTransition.direction == eTransDirection.IN) {
					if (lastTransition.group.onTransitionInComplete != null) {
						lastTransition.group.onTransitionInComplete.Invoke(lastTransition.group);
					}
				}
				else {
					if (lastTransition.group.onTransitionOutComplete != null) {
						lastTransition.group.onTransitionOutComplete.Invoke(lastTransition.group);
					}
				}
			}

			return true;
		}
	}
}
