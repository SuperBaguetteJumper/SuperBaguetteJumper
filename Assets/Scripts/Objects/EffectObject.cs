using UnityEngine;

namespace Objects {
	public abstract class EffectObject : TimedObject {
		[field: Header("Effect Object")]
		[field: SerializeField] public float Strengh { get; private set; } = 0.4f;
	}
}
