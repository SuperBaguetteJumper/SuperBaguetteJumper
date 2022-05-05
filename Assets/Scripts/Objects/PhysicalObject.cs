using UnityEngine;

namespace Objects {
	[ExecuteInEditMode]
	public abstract class PhysicalObject : MonoBehaviour {
		[Header("PhysicalObject")]
		[SerializeField] private bool IsCollectible;
		[SerializeField] private GameObject model;

		private GameObject modelInstance;

		private static readonly float MODEL_SCALE = 0.4f;

		private void Start() {
			this.ClearModel();
			this.modelInstance = Instantiate(this.model, this.transform);
			this.modelInstance.transform.localScale = new Vector3(MODEL_SCALE, MODEL_SCALE, MODEL_SCALE);
			this.modelInstance.transform.localPosition = new Vector3(0, 0.3f, 0);
			this.modelInstance.transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
		}

		private void Update() {
			if (!Application.isPlaying)
				return;
			this.modelInstance.transform.localPosition = new Vector3(0, 0.3f + Mathf.Sin(Time.time * 2) / 10, 0);
			this.modelInstance.transform.localRotation = Quaternion.AngleAxis(Time.time * 30, Vector3.up);
		}

		private void OnTriggerEnter(Collider other) {
			if (!Application.isPlaying)
				return;
			this.OnCollect();
			if (this.IsCollectible)
				Destroy(this.gameObject);
		}

		private void OnDestroy() {
			this.ClearModel();
		}

		private void ClearModel() {
			foreach (Transform child in this.transform)
				DestroyImmediate(child.gameObject);
		}

		protected abstract void OnCollect();
	}
}
