using Events;

namespace Objects {
	public class OldCarObject : PhysicalObject {
		protected override bool OnCollect() {
			OldCarObjectPickedUpEvent e = new OldCarObjectPickedUpEvent(this);
			EventManager.Instance.Raise(e);
			return e.CanPickup;
		}
	}
}
