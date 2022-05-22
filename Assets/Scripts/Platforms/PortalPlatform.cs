using Events;
using UnityEngine;
using UnityEngine.UI;

namespace Platforms {
	public class PortalPlatform : PressurePlatePlatform {
		[SerializeField] private string levelName;
		[SerializeField] private Text nameText;

		private void Awake() {
			if (this.levelName == null || this.levelName.Length == 0) {
				Debug.LogWarning($"Empty level name in portal {this.name}! Destroying");
				Destroy(this.gameObject);
			}
		}

		private void Start() {
			this.nameText.text = this.levelName;
		}

		private void Update() {
			if (this.PressurePlate.IsActivated) {
				this.PressurePlate.Deactivate();
				EventManager.Instance.Raise(new LevelLaunchEvent(this.levelName));
			}
		}
	}
}
