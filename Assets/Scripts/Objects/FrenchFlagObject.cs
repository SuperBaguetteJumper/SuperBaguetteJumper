using Events;

namespace Objects {
	public class FrenchFlagObject : PhysicalObject {
		protected override void OnCollect() {
			EventManager.Instance.Raise(new FrenchFlagObjectPickedUpEvent(this));
		}
	}
}
