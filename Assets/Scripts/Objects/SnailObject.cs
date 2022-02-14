using UnityEngine;

namespace Objects {
	public class SnailObject : PhysicalObject {
		protected override void OnCollect() {
			Debug.Log("Effect - Slowness");
		}
	}
}
