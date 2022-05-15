using Events;
using UnityEngine;

namespace Objects {
	public class FrenchFlagObject : PhysicalObject {
		protected override bool OnCollect() {
			FrenchFlagObjectPickedUpEvent e = new FrenchFlagObjectPickedUpEvent(this);																																	Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
			EventManager.Instance.Raise(e);
			return e.CanPickup;
		}
	}
}
