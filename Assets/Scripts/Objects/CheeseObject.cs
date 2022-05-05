using Events;

namespace Objects {
	public class CheeseObject : PhysicalObject {
		protected override void OnCollect() {
			EventManager.Instance.Raise(new CheeseObjectPickedUpEvent(this));
		}
	}
}
