using System.Collections;
using Common;
using Events;
using UnityEngine;

namespace Elements {
	public class Flag : MonoBehaviour {
		[SerializeField] private FlagType type = FlagType.None;
		[SerializeField] private bool raised;
		[SerializeField] private float raiseLength;
		[SerializeField] private AnimationCurve curve;
		[SerializeField] private GameObject animatedObject;
		[SerializeField] private string sound;

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
			Player player = other.gameObject.GetComponent<Player>();
			if (player == null)
				return;
			switch (this.type) {
			case FlagType.Checkpoint:
				EventManager.Instance.Raise(new CheckpointReachedEvent(this));
				break;
			case FlagType.End:
				EventManager.Instance.Raise(new EndReachedEvent(this));
				break;
			}
			if (!this.raised) {
				this.raised = true;
				this.StartCoroutine(this.Raise());
				if (this.sound != null && this.sound.Length != 0)
					SfxManager.Instance.PlaySfx2D(this.sound);
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
	}

	public enum FlagType {
		None, Start, Checkpoint, End
	}
}
