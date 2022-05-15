using Events;
using UnityEngine;

namespace Objects {
	public class CoinObject : PhysicalObject {
		[field: Header("Coin Object")]
		[field: SerializeField]
		public int Value { get; private set; } = 1;

		protected override bool OnCollect() {
			CoinObjectPickedUpEvent e = new CoinObjectPickedUpEvent(this);
			EventManager.Instance.Raise(e);
			return e.CanPickup;
		}
	}
}
