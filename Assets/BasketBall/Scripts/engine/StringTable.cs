using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.engine {
	public class StringTable : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		private const string FLAG_DYNAMIC = "!";
		private const char SEPARATOR = '~';
		private const string FLAG_FUNCTION = "#";
		private const string KEY_NOT_FOUND = "Key Not Found: ";
		private const string RETURN = "<br>";

		[System.Serializable]
		private class StringInfo {
			public string key = null;
			public string value = null;
		}

		public delegate string chunkEvaluator(string chunk);

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private List<StringInfo>stringList = null;

		// Interface //////////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------
		private static StringTable Instance = null;

		public static string GetString(string fromKey) {
			return Instance != null ? Instance._GetString(fromKey) : KEY_NOT_FOUND + fromKey;
		}

		public static void RegisterChunkEvaluator(string name, chunkEvaluator function) {
			Assert.That(Instance != null, "No StringTable in scene!");
			Instance._RegisterChunkEvaluator(name, function);
		}

		// Instance ---------------------------------------------------------------
		// Implementation /////////////////////////////////////////////////////////
		private Hashtable stringTable = new Hashtable();

		private Hashtable functionTable = new Hashtable();

		private void BuildStringTable() {
			for (int i=0; i<stringList.Count; ++i) {
				Assert.That(!stringTable.Contains(stringList[i].key), "StringTable already contains entry for " + stringList[i].key + "!", gameObject);
				stringTable.Add(stringList[i].key, stringList[i].value);
			}
		}

		private string EvaluateChunk(string chunk) {
			string value = "";

			if (chunk.StartsWith(FLAG_FUNCTION)) {
				string subChunk = chunk.Substring(1);
				chunkEvaluator function = functionTable[subChunk] as chunkEvaluator;
				Assert.That(function != null, "Evaluator " + subChunk + " not found!", gameObject);
				value = function(subChunk);
			}
			else {
				value = chunk;
			}

			return value;
		}

		private string _GetString(string key) {
			string value = key != null && key != null ? stringTable[key] as string : null;

			Assert.That(value != null && value.Length > 0, "Invalid string for key: " + key, gameObject);

			value = value.Replace(RETURN, "\n");

			if (value != null && value.StartsWith(FLAG_DYNAMIC)) {
				string[] chunks = value.Substring(1).Split(SEPARATOR);

				value = "";
				for (int i=0; i<chunks.Length; ++i) {
					value += EvaluateChunk(chunks[i]);
				}
			}

			return value;
		}

		private void _RegisterChunkEvaluator(string name, chunkEvaluator function) {
			if (name != null && name.Length > 0 && function != null) {
				functionTable[name] = function;
			}
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();

			Assert.That(Instance == null, "Multiple StringTables in project!", gameObject);
			Instance = this;

			BuildStringTable();
		}

		protected void OnDestroy() {
			Instance = null;
		}

		// Coroutines /////////////////////////////////////////////////////////////

		// Special String Functions ///////////////////////////////////////////////
	}
}
