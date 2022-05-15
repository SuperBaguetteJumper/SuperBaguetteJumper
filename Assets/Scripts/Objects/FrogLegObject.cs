using Events;

namespace Objects {
	public class FrogLegObject : EffectObject {
		protected override bool OnCollect() {
			FrogLegObjectPickedUpEvent e = new FrogLegObjectPickedUpEvent(this);
			EventManager.Instance.Raise(e);
			return e.CanPickup;
		}
	}
}
