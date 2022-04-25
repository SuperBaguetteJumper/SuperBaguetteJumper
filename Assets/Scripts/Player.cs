using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField]
	private GameObject cameraContainer;

	private Vector3 spawnPos;
	private Quaternion spawnRot;

	public bool OnGround {
		get => Physics.SphereCast(this.rigidbody.position + 0.001f * Vector3.up, 0.25f, Vector3.down, out _, 0.002f);
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
		Quaternion qRot = Quaternion.AngleAxis(Time.fixedDeltaTime * 900 * mouseXInput, this.transform.up);
		Quaternion qRotUpright = Quaternion.FromToRotation(this.transform.up, Vector3.up);
		Quaternion qOrientSlightlyUpright = Quaternion.Slerp(this.transform.rotation, qRotUpright * this.transform.rotation, 4 * Time.fixedDeltaTime);
		this.rigidbody.MovePosition(this.transform.position + moveVect);
		this.rigidbody.MoveRotation(qRot * qOrientSlightlyUpright);
		this.rigidbody.angularVelocity = Vector3.zero;

		Transform cTrans = this.cameraContainer.transform;
		float rot = cTrans.localEulerAngles.x - Time.fixedDeltaTime * 450 * mouseYInput;
		if (rot < 0) rot += 360;
		if (rot > 360) rot -= 360;
		if (rot > 90 && rot < 180) rot = 90;
		if (rot < 270 && rot >= 180) rot = 270;
		cTrans.localEulerAngles = new Vector3(rot, cTrans.localEulerAngles.y, cTrans.localEulerAngles.z);

		if (onGround && Input.GetButton("Jump"))
			this.rigidbody.AddForce(this.transform.up * 5, ForceMode.Impulse);

		if (this.rigidbody.velocity.y < 0 && !onGround)
			this.rigidbody.velocity += Vector3.up * (Physics.gravity.y * 1.25f * Time.fixedDeltaTime);
		else if (this.rigidbody.velocity.y > 0 && !Input.GetButton("Jump"))
			this.rigidbody.velocity += Vector3.up * (Physics.gravity.y * 0.75f * Time.fixedDeltaTime);

		if (this.rigidbody.position.y < -10) {
			this.rigidbody.MovePosition(this.spawnPos);
			this.rigidbody.MoveRotation(this.spawnRot);
			this.rigidbody.velocity = Vector3.zero;
		}
	}
}
