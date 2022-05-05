using Events;
using UnityEngine;

namespace Platforms {
	public class TrappedPlatformSpikes : MonoBehaviour {
		private void OnTriggerEnter(Collider other) {
			EventManager.Instance.Raise(new PlayerTrappedEvent());
		}
	}
}
