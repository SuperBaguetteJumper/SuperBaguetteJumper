using Common;
using UnityEngine;

public class CameraController : SimpleGameStateObserver {
	[SerializeField] private Transform target;
	private Vector3 initPosition;
	private Transform cameraTransform;

	protected override void Awake() {
		base.Awake();
		this.cameraTransform = this.transform;
		this.initPosition = this.cameraTransform.position;
	}

	private void Update() {
		if (!GameManager.Instance.IsPlaying)
			return;

		// TODO
	}

	private void ResetCamera() {
		this.cameraTransform.position = this.initPosition;
	}

	protected override void GameMenu(GameMenuEvent e) {
		this.ResetCamera();
	}
}
