using Events;

namespace Objects {
	public class OldCarObject : PhysicalObject {
		protected override void OnCollect() {
			EventManager.Instance.Raise(new OldCarObjectPickedUpEvent(this));
		}
	}
}
