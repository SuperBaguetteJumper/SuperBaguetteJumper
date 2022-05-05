using UnityEngine;

public class CameraViewManager : MonoBehaviour {
	[SerializeField] private Camera firstPersonView;
	[SerializeField] private Camera secondPersonView;
	[SerializeField] private Camera thirdPersonView;

	private void Awake() {
		this.firstPersonView.enabled = false;
		this.secondPersonView.enabled = false;
		this.thirdPersonView.enabled = true;
	}

	private void Update() {
		if (Input.GetButtonDown("Camera View"))
			this.Toggle();
	}

	public void Toggle() {
		bool thirdEnabled = this.thirdPersonView.enabled;
		this.thirdPersonView.enabled = this.secondPersonView.enabled;
		this.secondPersonView.enabled = this.firstPersonView.enabled;
		this.firstPersonView.enabled = thirdEnabled;
	}
}
