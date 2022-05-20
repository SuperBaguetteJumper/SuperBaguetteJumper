using Events;
using UnityEngine;

namespace Objects {
	public class SpringObject : PhysicalObject {
		[field: Header("Spring Object")]
		[field: SerializeField]
		public int Force { get; private set; } = 10;

		protected override bool OnCollect() {
			SpringObjectPickedUpEvent e = new SpringObjectPickedUpEvent(this);
			EventManager.Instance.Raise(e);
			return e.CanPickup;
		}
	}
}
