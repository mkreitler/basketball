using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.thinkagaingames.engine;

namespace com.thinkagaingames.basketball {
	public class BallBasic : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		public enum eTypes {
			BASIC
		}

		[System.Serializable]
		private class ContactSoundData {
			public string surface = null;
			public string sound = null;
			public float minRepeatInterval = 0f;

			[System.NonSerialized]
			public float lastPlayTime = 0f;
		}

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private float netDragLateral = 0.95f;

		[SerializeField]
		private float netDragVertical = 0.33f;

		[SerializeField]
		private List<ContactSoundData> contactSounds = null;

		// Interface //////////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------

		// Instance ---------------------------------------------------------------
		public float Mass {
			get {
				return RigidBody.mass;
			}
		}

		public Vector3 Position {
			get {
				return gameObject.transform.position;
			}
		}

		public void StopPhysics() {
			MakeKinematic();
			RigidBody.velocity = Vector3.zero;
		}
		
		public void MakeKinematic() {
			RigidBody.isKinematic = true;
		}

		public void MakeDynamic() {
			Armed = true;
			CollisionCount = 0;
			Switchboard.Broadcast("BallArmed", null);
			RigidBody.isKinematic = false;
		}

		public void EnteredGoal() {
			DoDrag = true;
		}

		public void ExitedGoal() {
			DoDrag = false;
		}

		public void MoveTo(Vector3 vPoint) {
			if (!RigidBody.isKinematic) {
				MakeKinematic();
			}

			gameObject.transform.position = vPoint;
		}

		public void Release() {
			MakeDynamic();
		}

		public void Launch(Vector3 vImpulse, Vector3 vAngularImpulse) {
			RigidBody.isKinematic = false;
			RigidBody.AddForce(vImpulse, ForceMode.Impulse);
			RigidBody.AddTorque(vAngularImpulse, ForceMode.Impulse);
		}

		// Implementation /////////////////////////////////////////////////////////
		protected bool Armed {get; set;}

		protected Rigidbody RigidBody {get; set;}

		private bool DoDrag {get; set;}

		private int CollisionCount {get; set;}

		private Renderer Renderer {get; set;}

		private Hashtable contactSoundsTable = new Hashtable();

		private void BuildContactSoundsTable() {
			for (int i=0; i<contactSounds.Count; ++i) {
				if (!contactSoundsTable.Contains(contactSounds[i].surface)) {
					contactSoundsTable[contactSounds[i].surface.ToLower()] = contactSounds[i];
				}
				else {
					Assert.That(false, "Duplicate contact sounds for surface " + contactSounds[i].surface, gameObject);
				}
			}
		}

		private void PlayContactSound(string surfaceTag) {
			surfaceTag = surfaceTag.ToLower();

			if (contactSoundsTable.Contains(surfaceTag) && Renderer.isVisible) {
				ContactSoundData contactData = contactSoundsTable[surfaceTag] as ContactSoundData;
				Assert.That(contactData != null, "Invalid contact sound data!", gameObject);

				if (Time.time - contactData.lastPlayTime > contactData.minRepeatInterval) {
					contactData.lastPlayTime = Time.time;
					SoundSystem.PlaySound(contactData.sound);
				}
			}
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();

			Renderer = gameObject.GetComponentInChildren<Renderer>(true);
			Assert.That(Renderer != null, "Renderer component not found!", gameObject);

			RigidBody = gameObject.GetComponent<Rigidbody>();
			Assert.That(RigidBody != null, "Rigidbody component not found!", gameObject);

			BuildContactSoundsTable();

			Switchboard.AddListener("BallInGoal", OnBallInGoal);
			Switchboard.AddListener("InitStage", OnInitStage);
			Switchboard.AddListener("StageEnded", OnStageEnded);
		}

		protected void OnEnable() {
			DoDrag = false;
		}

		protected virtual void Update() {
		}

		protected virtual void FixedUpdate() {
			if (DoDrag) {
				Vector3 vVel = RigidBody.velocity;
				Vector3 vDrag = RigidBody.velocity * netDragLateral * Time.fixedDeltaTime;

				vDrag.z = vVel.z * netDragVertical * Time.fixedDeltaTime;

				RigidBody.AddForce(-vDrag * Mass, ForceMode.Impulse);

				RigidBody.AddTorque(-RigidBody.angularVelocity, ForceMode.VelocityChange);
			}
		}

		protected void OnTriggerEnter(Collider collider) {
			string otherTag = collider != null && collider.gameObject != null ? collider.gameObject.tag : null;

			if (otherTag != null) {
				otherTag = otherTag.ToLower();

				if (Armed) {
					if (otherTag == "miss") {
						Armed = false;
						Switchboard.Broadcast("PlayerMissed", null);
					}
				}
			}
		}

		protected void OnCollisionEnter(Collision collision) {
			if (collision.gameObject != null) {
				CollisionCount += 1;
				PlayContactSound(collision.gameObject.tag);
			}
		}

		// Coroutines /////////////////////////////////////////////////////////////

		// Message Handlers ///////////////////////////////////////////////////////
		public void OnBallInGoal(object objBallGameObject) {
			GameObject goBall = objBallGameObject as GameObject;

			if (goBall == gameObject && Armed) {
				Armed = false;
				SoundSystem.PlaySound("swish");
				Switchboard.Broadcast("PlayerScored", gameObject.transform.position);
				MakeKinematic();
				gameObject.transform.localPosition = Vector3.zero;
			}
		}

		public void OnInitStage(object ignored) {
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.SetActive(true);
		}

		public void OnStageEnded(object ignored) {
			MakeDynamic();
			Armed = false;
			gameObject.SetActive(false);
		}
	}
}
