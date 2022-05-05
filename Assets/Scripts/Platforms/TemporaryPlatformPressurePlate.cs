using UnityEngine;

namespace Platforms {
	public class TemporaryPlatformPressurePlate : MonoBehaviour {
		public bool IsActivated { get; private set; }

		private void OnTriggerEnter(Collider other) {
			this.IsActivated = true;
		}

		public void Desactivate() {
			this.IsActivated = false;
		}
	}
}
