using UnityEngine;

namespace Objects {
	public abstract class TimedObject : PhysicalObject {
		[field: Header("Timed Object")]
		[field: SerializeField] public float Duration { get; private set; } = 5;
	}
}
