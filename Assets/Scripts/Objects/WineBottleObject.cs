using Events;

namespace Objects {
	public class WineBottleObject : PhysicalObject {
		protected override void OnCollect() {
			EventManager.Instance.Raise(new WineBottleObjectPickedUpEvent(this));
		}
	}
}
