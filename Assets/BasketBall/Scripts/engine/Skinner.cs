/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       20/02/2018 19:58
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.thinkagaingames.engine {
	public class Skinner : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		[System.Serializable]
		public enum eSKINNER_TYPE {
			IMAGE,
			SPRITE
		}

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private string triggerMessage = null;

		[SerializeField]
		private eSKINNER_TYPE type = eSKINNER_TYPE.IMAGE;

		// Interface //////////////////////////////////////////////////////////////
		public bool WantsVisible {
			get {
				return wantsVisible;
			}
		}

		// Implementation /////////////////////////////////////////////////////////
		protected bool wantsVisible = false;

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Start() {
			base.Start();

			Assert.That(triggerMessage != null && triggerMessage.Length > 0, "Undefined trigger message!", gameObject);
			Switchboard.AddListener(triggerMessage, OnTriggered);
		}

		// Coroutines /////////////////////////////////////////////////////////////
		// Message Handlers ///////////////////////////////////////////////////////
		public void OnTriggered(object objResource) {
			wantsVisible = true;

			switch(type) {
				case eSKINNER_TYPE.IMAGE: {
						Image image = gameObject.GetComponent<Image>();
						Assert.That(image != null, "Invalid skin target (image)!", gameObject);

						Sprite sprite = objResource as Sprite;

						if (sprite == null) {
							Renderer renderer = image.GetComponent<Renderer>();
							if (renderer != null) {
								renderer.enabled = false;
								image.sprite = null;
								wantsVisible = false;
							}
						}
						else {
							image.sprite = sprite;
						}
					break;
				}

				case eSKINNER_TYPE.SPRITE: {
						SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
						Assert.That(spriteRenderer != null, "No spriteRenderer found!", gameObject);

						Sprite sprite = objResource as Sprite;

						if (sprite == null) {
							spriteRenderer.enabled = false;
							spriteRenderer.sprite = null;
							wantsVisible = false;
						}
						else {
							spriteRenderer.sprite = sprite;
							Vector2 spriteSize = spriteRenderer.size;
							spriteSize.x = 1;
							spriteSize.y = 1;
							spriteRenderer.size = spriteSize;
						}
					break;
				}

				default:
					Assert.That(false, "Unknown SKINNER_TYPE!", gameObject);
				break;
			}
		}
	}
}
