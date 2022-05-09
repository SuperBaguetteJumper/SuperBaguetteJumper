using Events;
using UnityEngine;

namespace Platforms {
	public class TrappedPlatformSpikes : MonoBehaviour {
		private void OnTriggerEnter(Collider other) {
			Player player = other.gameObject.GetComponent<Player>();
			if (player != null)
				EventManager.Instance.Raise(new PlayerTrappedEvent(player));
		}
	}
}
