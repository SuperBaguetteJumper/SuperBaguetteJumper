using System.Collections;
using UnityEngine;

namespace Elements.Flags {
	public abstract class Flag : MonoBehaviour {
		[SerializeField] private bool raised;
		[SerializeField] private float raiseLength;
		[SerializeField] private AnimationCurve curve;
		[SerializeField] private GameObject animatedObject;

		private float FlagRaise {
			set {
				Vector3 position = this.animatedObject.transform.localPosition;
				position.y = value;
				this.animatedObject.transform.localPosition = position;
			}
		}

		private void Awake() {
			this.FlagRaise = this.raised ? 1 : 0;
		}

		private void OnTriggerEnter(Collider other) {
			this.OnReach();
			if (!this.raised) {
				this.raised = true;
				this.StartCoroutine(this.Raise());
			}
		}

		private IEnumerator Raise() {
			float start = Time.time;
			float duration;
			while ((duration = Time.time - start) < this.raiseLength) {
				float progress = duration / this.raiseLength;
				this.FlagRaise = this.curve.Evaluate(progress);
				yield return null;
			}
		}

		protected abstract void OnReach();
	}
}
