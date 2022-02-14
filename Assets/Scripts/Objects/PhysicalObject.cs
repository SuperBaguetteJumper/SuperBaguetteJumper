using UnityEngine;

namespace Objects {
	public abstract class PhysicalObject : MonoBehaviour {
		[Header("PhysicalObject")]
		[SerializeField] private bool IsCollectible;
		[SerializeField] private GameObject model;

		private GameObject modelInstance;

		private static readonly float MODEL_SCALE = 0.4f;

		private void Start() {
			this.modelInstance = Instantiate(this.model, this.transform);
			this.modelInstance.transform.localScale = new Vector3(MODEL_SCALE, MODEL_SCALE, MODEL_SCALE);
		}

		private void Update() {
			this.modelInstance.transform.localPosition = new Vector3(0, 0.3f + Mathf.Sin(Time.time * 2) / 10, 0);
			this.modelInstance.transform.localRotation = Quaternion.AngleAxis(Time.time * 30, Vector3.up);
		}

		private void OnTriggerEnter(Collider other) {
			this.OnCollect();
			if (this.IsCollectible)
				Destroy(this.gameObject);
		}

		protected abstract void OnCollect();
	}
}
