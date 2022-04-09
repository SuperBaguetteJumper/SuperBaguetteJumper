using UnityEngine;

public class TestPlayer : MonoBehaviour {
	[SerializeField]
	private GameObject cameraContainer;

	private Vector3 spawnPos;
	private Quaternion spawnRot;

	public bool OnGround {
		get => Physics.Raycast(this.rigidbody.position + 0.249f * Vector3.down, Vector3.down, out _, 0.002f);
	}

	private new Rigidbody rigidbody;

	private void Awake() {
		this.rigidbody = this.GetComponent<Rigidbody>();
		this.spawnPos = this.rigidbody.position;
		this.spawnRot = this.rigidbody.rotation;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void FixedUpdate() {
		bool onGround = this.OnGround;
		float vInput = Input.GetAxis("Vertical");
		float hInput = Input.GetAxis("Horizontal");
		float mouseXInput = Input.GetAxisRaw("Mouse X");
		float mouseYInput = Input.GetAxisRaw("Mouse Y");
		Vector3 moveVect = (onGround ? 1 : 0.7f) * Time.fixedDeltaTime * 1.5f * Vector3.ProjectOnPlane(vInput * this.transform.forward + hInput * this.transform.right, Vector3.up).normalized;
		Quaternion qRot = Quaternion.AngleAxis(Time.fixedDeltaTime * 250 * mouseXInput, this.transform.up);
		Quaternion qRotUpright = Quaternion.FromToRotation(this.transform.up, Vector3.up);
		Quaternion qOrientSlightlyUpright = Quaternion.Slerp(this.transform.rotation, qRotUpright * this.transform.rotation, 4 * Time.fixedDeltaTime);
		this.rigidbody.MovePosition(this.transform.position + moveVect);
		this.rigidbody.MoveRotation(qRot * qOrientSlightlyUpright);
		this.rigidbody.angularVelocity = Vector3.zero;

		Transform cTrans = this.cameraContainer.transform;
		cTrans.RotateAround(cTrans.position, cTrans.right, -Time.fixedDeltaTime * 200 * mouseYInput);

		Debug.DrawRay(this.rigidbody.position + 0.249f * Vector3.down, Vector3.down * 0.002f, Color.blue);
		if (Input.GetButton("Jump") && onGround)
			this.rigidbody.AddForce(0, 5, 0, ForceMode.Impulse);

		if (this.rigidbody.position.y < -10) {
			this.rigidbody.MovePosition(this.spawnPos);
			this.rigidbody.MoveRotation(this.spawnRot);
		}
	}
}
