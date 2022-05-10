using UnityEngine;

namespace Objects {
	[ExecuteInEditMode]
	public abstract class PhysicalObject : MonoBehaviour {
		[Header("Physical Object")]
		[SerializeField] private bool IsCollectible;
		[SerializeField] private GameObject model;
		[SerializeField] private float scale = 0.4f;
		[SerializeField] private float offset = 0.3f;

		private GameObject modelInstance;
		private bool hasNoActiveInstance = true;

		private void Start() {
			if (this.model == null)
				return;
			this.ClearModel();
			this.modelInstance = Instantiate(this.model, this.transform);
			this.modelInstance.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
			this.modelInstance.transform.localPosition = new Vector3(0, this.offset, 0);
			this.modelInstance.transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
			this.hasNoActiveInstance = false;
		}

		private void Update() {
			if (!Application.isPlaying || this.hasNoActiveInstance)
				return;
			this.modelInstance.transform.localPosition = new Vector3(0, this.offset + Mathf.Sin(Time.time * 2) / 10, 0);
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
			this.modelInstance = null;
			this.hasNoActiveInstance = true;
		}

		protected abstract void OnCollect();
	}
}
