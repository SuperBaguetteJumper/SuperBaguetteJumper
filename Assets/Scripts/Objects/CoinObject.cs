using Events;
using UnityEngine;

namespace Objects {
	public class CoinObject : PhysicalObject {
		[field: Header("Coin Object")]
		[field: SerializeField]
		public int Value { get; private set; } = 1;

		protected override void OnCollect() {
			EventManager.Instance.Raise(new CoinObjectPickedUpEvent(this));
		}
	}
}
