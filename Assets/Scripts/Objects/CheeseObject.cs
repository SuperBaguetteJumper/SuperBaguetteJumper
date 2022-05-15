using Events;

namespace Objects {
	public class CheeseObject : EffectObject {
		protected override bool OnCollect() {
			CheeseObjectPickedUpEvent e = new CheeseObjectPickedUpEvent(this);
			EventManager.Instance.Raise(e);
			return e.CanPickup;
		}
	}
}
