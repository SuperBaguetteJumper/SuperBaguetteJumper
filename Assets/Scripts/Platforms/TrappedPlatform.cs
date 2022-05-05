using System.Collections;
using UnityEngine;

namespace Platforms {
	public class TrappedPlatform : MonoBehaviour {
		[SerializeField] private float initialDelay;
		[SerializeField] private float transitionInLength = 0.5f;
		[SerializeField] private float transitionOutLength = 0.5f;
		[SerializeField] private float safeLength = 3;
		[SerializeField] private float dangerLength = 5;
		[SerializeField] private AnimationCurve curve;

		private TrappedPlatformSpikes spikes;
		private int transitionFrames;

		private float SpikesScale {
			set {
				Vector3 scale = this.spikes.transform.localScale;
				scale.y = value;
				this.spikes.transform.localScale = scale;
			}
		}

		private void Awake() {
			this.spikes = this.GetComponentInChildren<TrappedPlatformSpikes>();
			this.SpikesScale = 0;
		}

		private IEnumerator Start() {
			yield return new WaitForSeconds(this.initialDelay);
			this.StartCoroutine(this.SpikesOut());
		}

		private IEnumerator SpikesOut() {
			yield return this.SpikesTransition(false);
			this.SpikesScale = 1;
			yield return new WaitForSeconds(this.dangerLength);
			this.StartCoroutine(this.SpikesIn());
		}

		private IEnumerator SpikesIn() {
			yield return this.SpikesTransition(true);
			this.SpikesScale = 0;
			yield return new WaitForSeconds(this.safeLength);
			this.StartCoroutine(this.SpikesOut());
		}

		private IEnumerator SpikesTransition(bool isTransitionIn) {
			float start = Time.time;
			float length = isTransitionIn ? this.transitionInLength : this.transitionOutLength;
			float duration;
			while ((duration = Time.time - start) < length) {
				float progress = duration / length;
				this.SpikesScale = this.curve.Evaluate(isTransitionIn ? 1 - progress : progress);
				yield return null;
			}
		}
	}
}
