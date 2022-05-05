using Events;

namespace Elements.Flags {
	public class CheckpointFlag : Flag {
		protected override void OnReach() {
			EventManager.Instance.Raise(new CheckpointReachedEvent());
		}
	}
}
