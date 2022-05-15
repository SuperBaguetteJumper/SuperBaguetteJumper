using Events;
using UnityEngine;

namespace Platforms {
	public class TrappedPlatformSpikes : MonoBehaviour {
		private void OnTriggerEnter(Collider other) {
			if (other.gameObject.GetComponent<Player>() != null)
				EventManager.Instance.Raise(new PlayerTrappedEvent());
		}
	}
}
