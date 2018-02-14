﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.thinkagaingames.engine {
	public class UiDirector : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		[System.Serializable]
		public class TransitionEndEvent : UnityEvent<TransitionGroup, bool> {}

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

			public void Reset() {
				group = null;
				direction = eTransDirection.UNKNOWN;
				completionsRemaining = 0;
				wantsRemove = false;
			}

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

			Assert.That(!panels.Contains(newPanelName), "UiPanel::UiPanel name '" + newPanelName + "' not unique!");
			panels[newPanelName] = panel;
		}

		public static UiPanel GetPanel(string name) {
			return (name != null && name.Length > 0) ? panels[name.ToLower()] as UiPanel : null;
		}

		// Instance ---------------------------------------------------------------
		public void StartTransition(string groupName, bool isTransitionIn) {
			TransitionGroup group = GetTransitionGroup(groupName);

			Assert.That(group != null, "Unknown transition group: " + groupName, gameObject);

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
		public void PanelTransitionComplete(string groupName, string panelName) {
			Assert.That(groupName != null && groupName.Length > 0, "Invalid group name!", gameObject);
			Assert.That(panelName != null && panelName.Length > 0, "Invalid panel name!", gameObject);
			Assert.That(transitionHistory.Count > 0, "Invalid transtion history!", gameObject);

			int recordIndex = -1;
			TransitionRecord lastTransition = FindLastTransitionForGroup(groupName, out recordIndex);
			Assert.That(lastTransition != null, "Transition record not found!", gameObject);
			
			if (lastTransition.RegisterCompletion(panelName)) {
				// All panels have transitioned.
				if (lastTransition.wantsRemove) {
					transitionHistory.RemoveAt(recordIndex);
				}

				if (lastTransition.direction == eTransDirection.IN) {
					if (lastTransition.group.onTransitionInComplete != null) {
						lastTransition.group.onTransitionInComplete.Invoke(lastTransition.group, true);
					}
				}
				else {
					if (lastTransition.group.onTransitionOutComplete != null) {
						lastTransition.group.onTransitionOutComplete.Invoke(lastTransition.group, false);
					}
				}
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		private List<TransitionRecord> unusedRecords = new List<TransitionRecord>();

		private List<TransitionRecord> transitionHistory = new List<TransitionRecord>();

		private TransitionRecord FindLastTransitionForGroup(string groupName, out int recordIndex) {
			TransitionRecord record = null;
			recordIndex = -1;

			for (int i=0; i<transitionHistory.Count; ++i) {
				if (transitionHistory[i].group.name == groupName) {
					record = transitionHistory[i];
					recordIndex = i;
					break;
				}
			}

			return record;
		}

		private TransitionRecord GetUnusedTransitionRecord() {
			TransitionRecord record = null;

			if (unusedRecords.Count > 0) {
				record = unusedRecords[0];
				unusedRecords.RemoveAt(0);
				record.Reset();
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
					group.panels[i].TransitionIn(group.name);
				}
				else {
					group.panels[i].TransitionOut(group.name);
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
		}

		protected virtual void OnDestroy() {
			Instance = null;
		}

		// Coroutines /////////////////////////////////////////////////////////////

		// Message Handlers ///////////////////////////////////////////////////////
	}
}
