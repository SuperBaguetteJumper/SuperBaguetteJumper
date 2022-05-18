using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platforms {
	public class MovingPlatform : MonoBehaviour {
		[SerializeField] private bool useCurrentPosAsFirstCoord = true;
		[SerializeField] [DraggableVector3] private List<Vector3> coordinates;
		[SerializeField] private float speed = 0.5f;
		[SerializeField] private float initialDelay;
		[SerializeField] private float waitDelay;
		[SerializeField] private AnimationCurve curve;

		private void Awake() {
			if (this.useCurrentPosAsFirstCoord)
				this.coordinates.Insert(0, this.transform.position);
			if (this.coordinates.Count < 2) {
				Debug.LogWarning($"Not enough coordinates: {this.name} destroyed.");
				Destroy(this.gameObject);
				return;
			}
			if (!this.useCurrentPosAsFirstCoord)
				this.transform.position = this.coordinates[0];
		}

		private IEnumerator Start() {
			yield return new WaitForSeconds(this.initialDelay);
			while (this.enabled) {
				for (int i = 0; i < this.coordinates.Count; i++) {
					Vector3 from = this.coordinates[i];
					Vector3 to = this.coordinates[i + 1 == this.coordinates.Count ? 0 : i + 1];
					yield return this.Translation(from, to);
					yield return new WaitForSeconds(this.waitDelay);
				}
			}
		}

		private IEnumerator Translation(Vector3 from, Vector3 to) {
			float start = Time.time;
			float length = Vector3.Distance(from, to) / this.speed;
			float duration;
			while ((duration = Time.time - start) < length) {
				float progress = duration / length;
				this.transform.position = Vector3.Lerp(from, to, this.curve.Evaluate(progress));
				yield return null;
			}
			this.transform.position = to;
		}

		private void OnDrawGizmos() {
			if (this.coordinates.Count == 0)
				return;
			Gizmos.color = new Color(1, 1, 1, 0.5f);
			Vector3 start = this.useCurrentPosAsFirstCoord ? this.transform.position : this.coordinates[0];
			Vector3 from = start;
			for (int i = this.useCurrentPosAsFirstCoord ? 0 : 1; i < this.coordinates.Count; i++) {
				Vector3 to = this.coordinates[i];
				Gizmos.DrawLine(from, to);
				from = to;
			}
			Gizmos.DrawLine(from, start);
		}

		private void OnDrawGizmosSelected() {
			BoxCollider boxCollider = this.GetComponent<BoxCollider>();
			Gizmos.color = new Color(0, 0.5f, 1, 0.25f);
			for (int i = 0; i < this.coordinates.Count; i++)
				Gizmos.DrawCube(this.coordinates[i], boxCollider.size);
		}
	}
}
