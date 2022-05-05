using Events;

namespace Objects {
	public class FrogLegObject : PhysicalObject {
		protected override void OnCollect() {
			EventManager.Instance.Raise(new FrogLegObjectPickedUpEvent(this));
		}
	}
}
