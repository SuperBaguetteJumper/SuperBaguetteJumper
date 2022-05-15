using UnityEngine;

namespace Platforms {
	public class TemporaryPlatformPressurePlate : MonoBehaviour {
		public bool IsActivated { get; private set; }

		private void OnTriggerEnter(Collider other) {
			if (other.gameObject.GetComponent<Player>() != null)
				this.IsActivated = true;
		}

		public void Desactivate() {
			this.IsActivated = false;
		}
	}
}
