using Events;

namespace Objects {
	public class SnailObject : EffectObject {
		protected override bool OnCollect() {
			SnailObjectPickedUpEvent e = new SnailObjectPickedUpEvent(this);
			EventManager.Instance.Raise(e);
			return e.CanPickup;
		}
	}
}
