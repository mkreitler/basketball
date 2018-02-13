using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	public class CourtManager : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private List<GameObject> ballPrefabs = null;

		[SerializeField]
		private List<GameObject> ballDocks = null;

		// Interface //////////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------

		// Instance ---------------------------------------------------------------
		public string CurrentBallType {get; set;}	// <-- Corresponds to the GameObject tag on the prefab.

		public GameObject GetNextBall() {
			CurrentBallIndex += 1;
			CurrentBallIndex %= balls.Count;

			return balls[CurrentBallIndex];
		}

		// Implementation /////////////////////////////////////////////////////////
		private int CurrentBallIndex {get; set;}

		private List<GameObject> balls = new List<GameObject>();

		private int BallTypeIndex {get; set;}

		public void ProvideNextBall(object objIgnored) {
			Switchboard.Broadcast("SetBall", GetNextBall());
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();
			
			Assert.That(ballPrefabs.Count > 0, "No ball prefabs loaded!", gameObject);
			Assert.That(ballDocks.Count > 0, "No ball docks defined!", gameObject);

			CurrentBallType = "ball_basic";
		}

		protected override void Start() {
			Switchboard.AddListener("RequestNextBall", ProvideNextBall);
			base.Start();
		}

		public override void OnStartGame() {
			base.OnStartGame();

			GameObject ballPrefab = null;			

			for (int i=0; i<ballPrefabs.Count; ++i) {
				if (ballPrefabs[i].tag.ToLower() == CurrentBallType.ToLower()) {
					ballPrefab = ballPrefabs[i];
					break;
				}
			}

			Assert.That(ballPrefab != null, "Ball prefab " + CurrentBallType + " does not exist!", gameObject);

			for (int i=0; i<ballDocks.Count; ++i) {
				balls.Add(GameObject.Instantiate(ballPrefab, ballDocks[i].gameObject.transform.position, Quaternion.identity, ballDocks[i].gameObject.transform));

				BallBasic newBall = balls[i].GetComponent<BallBasic>();
				Assert.That(newBall != null, "Instantiated invalid ball!", gameObject);
				newBall.MakeKinematic();
			}

			CurrentBallIndex = -1;

			ProvideNextBall(null);
		}

		// Coroutines /////////////////////////////////////////////////////////////
	}
}
