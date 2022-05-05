using Events;
using UnityEngine;

namespace Objects {
	public class CheeseObject : PhysicalObject {
		[field: SerializeField] public float Strengh { get; private set; } = 0.4f;
		[field: SerializeField] public float Duration { get; private set; } = 5;

		protected override void OnCollect() {
			EventManager.Instance.Raise(new CheeseObjectPickedUpEvent(this));
		}
	}
}
