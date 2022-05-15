using Events;

namespace Objects {
	public class BaguetteObject : PhysicalObject {
		protected override bool OnCollect() {
			BaguetteObjectPickedUpEvent e = new BaguetteObjectPickedUpEvent(this);
			EventManager.Instance.Raise(e);
			return e.CanPickup;
		}
	}
}
