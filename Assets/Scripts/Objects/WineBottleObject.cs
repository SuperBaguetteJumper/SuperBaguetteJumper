using Events;

namespace Objects {
	public class WineBottleObject : EffectObject {
		protected override bool OnCollect() {
			WineBottleObjectPickedUpEvent e = new WineBottleObjectPickedUpEvent(this);
			EventManager.Instance.Raise(e);
			return e.CanPickup;
		}
	}
}
