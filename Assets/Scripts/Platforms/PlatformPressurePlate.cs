using UnityEngine;

namespace Platforms {
	public class PlatformPressurePlate : MonoBehaviour {
		public bool IsActivated { get; private set; }
		public Player Player { get; private set; }

		private void OnTriggerEnter(Collider other) {
			if (!this.IsActivated && other.gameObject.GetComponent<Player>() != null)
				this.IsActivated = true;
		}

		private void OnTriggerStay(Collider other) {
			this.Player = other.gameObject.GetComponent<Player>();
		}

		private void OnTriggerExit(Collider other) {
			if (this.Player != null && other.gameObject.GetComponent<Player>() != null)
				this.Player = null;
		}

		public void Deactivate() {
			this.IsActivated = false;
		}
	}
}
