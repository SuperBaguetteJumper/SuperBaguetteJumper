using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour {
	[SerializeField] private Transform cameraContainer;
	[SerializeField] private float moveSpeed = 1.5f;
	[SerializeField] private float airSpeedModifier = -0.3f;
	[SerializeField] private float crouchSpeedModifier = -0.6f;
	[SerializeField] private float sprintSpeedModifier = 0.6f;
	[SerializeField] private float rotationSpeed = 10;
	[SerializeField] private float jumpStrengh = 5;
	[SerializeField] private float fallModifier = 1.25f;
	[SerializeField] private float instaJumpModifier = 0.75f;
	[SerializeField] private float yRecoveryStrengh = 10;

	private float spawnCam;

	private Vector3 center {
		get => this.rigidbody.position + this.transform.TransformVector(this.collider.center);
	}

	private Vector3 offset {
		get => this.transform.up * (this.collider.height / 2 - this.collider.radius - 0.001f);
	}

	public bool OnGround {
		get {
			Vector3 c = this.center;
			Vector3 o = this.offset;
			return Physics.CapsuleCast(c - o, c + o, this.collider.radius, Vector3.down, out _, 0.002f);
		}
	}

	public Vector3 LookPosition => this.cameraContainer.position;
	public Vector3 LookDirection => this.cameraContainer.forward;
	public bool ViewLocked { get; set; }
	public bool CanMove { get; set; } = true;

	public float SpeedModifier { get; private set; }
	public float JumpModifier { get; private set; }

	public new Rigidbody rigidbody { get; private set; }
	public new CapsuleCollider collider { get; private set; }

	private static readonly float ROTATION_EPSILON = 0.001f;

	private void Die() {
		EventManager.Instance.Raise(new PlayerDiedEvent());
	}

	private void Awake() {
		this.rigidbody = this.GetComponent<Rigidbody>();
		this.collider = this.GetComponent<CapsuleCollider>();
		this.SpeedModifier = 1;
		this.JumpModifier = 1;
		this.spawnCam = this.cameraContainer.localEulerAngles.x;
		EventManager.Instance.AddListener<PlayerSpawnedEvent>(this.OnPlayerSpawned);
		EventManager.Instance.AddListener<PlayerTrappedEvent>(this.OnPlayerTrapped);
		EventManager.Instance.AddListener<EffectActivatedEvent>(this.OnEffectActivated);
		EventManager.Instance.AddListener<SpringObjectPickedUpEvent>(this.OnSpringObjectPickedUp);
	}

	private void Start() {
		EventManager.Instance.Raise(new PlayerLoadedEvent(this));
	}

	protected virtual void OnDestroy() {
		EventManager.Instance.RemoveListener<PlayerSpawnedEvent>(this.OnPlayerSpawned);
		EventManager.Instance.RemoveListener<PlayerTrappedEvent>(this.OnPlayerTrapped);
		EventManager.Instance.RemoveListener<EffectActivatedEvent>(this.OnEffectActivated);
		EventManager.Instance.RemoveListener<SpringObjectPickedUpEvent>(this.OnSpringObjectPickedUp);
	}

	private void FixedUpdate() {
		// Get player & controls status
		bool onGround = this.OnGround;
		float vInput = this.CanMove ? Input.GetAxis("Vertical") : 0;
		float hInput = this.CanMove ? Input.GetAxis("Horizontal") : 0;
		float mouseXInput = this.ViewLocked ? 0 : Input.GetAxisRaw("Mouse X");
		float mouseYInput = this.ViewLocked ? 0 : Input.GetAxisRaw("Mouse Y");

		// Compute move modifier
		float moveModifier = this.SpeedModifier;
		if (Input.GetButton("Crouch"))
			moveModifier += this.crouchSpeedModifier;
		if (Input.GetButton("Sprint"))
			moveModifier += this.sprintSpeedModifier;
		if (!onGround)
			moveModifier += this.airSpeedModifier;
		if (moveModifier < 0)
			moveModifier = 0;

		// Calculate move & rotation
		Vector3 moveVect = moveModifier * Time.fixedDeltaTime * this.moveSpeed * Vector3.ProjectOnPlane(vInput * this.transform.forward + hInput * this.transform.right, Vector3.up).normalized;
		float yRot = Time.fixedDeltaTime * this.rotationSpeed * 90 * mouseXInput;
		if (yRot < ROTATION_EPSILON && yRot > -ROTATION_EPSILON)
			yRot = 0;
		Quaternion qRot = Quaternion.AngleAxis(yRot, this.transform.up);
		Quaternion qRotUpright = Quaternion.FromToRotation(this.transform.up, Vector3.up);
		Quaternion qOrientSlightlyUpright = Quaternion.Slerp(this.transform.rotation, qRotUpright * this.transform.rotation, this.yRecoveryStrengh * Time.fixedDeltaTime);

		// Apply them
		this.rigidbody.MovePosition(this.transform.position + this.RestrictMove(moveVect));
		this.rigidbody.MoveRotation(qRot * qOrientSlightlyUpright);
		this.rigidbody.angularVelocity = Vector3.zero;

		// Rotate camera (up / down)
		Vector3 angle = this.cameraContainer.localEulerAngles;
		float xRot = Time.fixedDeltaTime * this.rotationSpeed * 90 * mouseYInput;
		if (xRot < ROTATION_EPSILON && xRot > -ROTATION_EPSILON)
			xRot = 0;
		this.cameraContainer.localEulerAngles = new Vector3(LimitCameraRot(angle.x - xRot), angle.y, angle.z);

		// Handle jump
		bool jumping = Input.GetButton("Jump");
		if (onGround && jumping)
			this.rigidbody.AddForce(this.transform.up * (this.jumpStrengh * this.JumpModifier), ForceMode.Impulse);
		if (this.rigidbody.velocity.y < 0 && !onGround)
			this.rigidbody.velocity += Physics.gravity * (this.fallModifier * Time.fixedDeltaTime);
		else if (this.rigidbody.velocity.y > 0 && !jumping)
			this.rigidbody.velocity += Physics.gravity * (this.instaJumpModifier * Time.fixedDeltaTime);

		// Kill player when too low
		if (this.rigidbody.position.y < -10)
			this.Die();
	}

	public void ForceMove(Vector3 move) {
		this.transform.position = this.transform.position + this.RestrictMove(move);
	}

	private Vector3 RestrictMove(Vector3 move) {
		Vector3 c = this.center;
		Vector3 o = this.offset;
		RaycastHit hit;
		float moveLength = move.magnitude;
		bool collision = Physics.CapsuleCast(c - o, c + o, this.collider.radius, move, out hit, moveLength);
		if (collision && !hit.collider.isTrigger) {
			Vector3 restrictedMove = Vector3.Project(move, Quaternion.AngleAxis(90, Vector3.up) * hit.normal);
			restrictedMove.y = (hit.normal.normalized * moveLength).y;
			return restrictedMove;
		}
		return move;
	}

	private void OnPlayerSpawned(PlayerSpawnedEvent e) {
		this.transform.position = e.RespawnPoint.position;
		this.transform.rotation = e.RespawnPoint.rotation * Quaternion.LookRotation(Vector3.right);
		this.rigidbody.velocity = Vector3.zero;
		Vector3 eulerAngles = this.cameraContainer.localEulerAngles;
		eulerAngles.x = this.spawnCam;
		this.cameraContainer.localEulerAngles = eulerAngles;
	}

	private void OnPlayerTrapped(PlayerTrappedEvent e) {
		this.Die();
	}

	private void OnEffectActivated(EffectActivatedEvent e) {
		switch (e.Effect) {
		case Effect.Slowness:
			this.StartCoroutine(this.SlownessEffect(e.Manager, e.Strengh));
			break;
		case Effect.Speedness:
			this.StartCoroutine(this.SpeednessEffect(e.Manager, e.Strengh));
			break;
		case Effect.JumpBoost:
			this.StartCoroutine(this.JumpBoostEffect(e.Manager, e.Strengh));
			break;
		case Effect.Drunkness:
			this.StartCoroutine(this.BlurEffect(e.Manager));
			this.StartCoroutine(this.ShakeEffect(e.Manager, e.Strengh));
			break;
		}
	}

	private IEnumerator SlownessEffect(EffectsManager manager, float strengh) {
		this.SpeedModifier -= strengh;
		yield return new WaitWhile(() => manager.HasSlownessEffect);
		this.SpeedModifier += strengh;
	}

	private IEnumerator SpeednessEffect(EffectsManager manager, float strengh) {
		this.SpeedModifier += strengh;
		yield return new WaitWhile(() => manager.HasSpeednessEffect);
		this.SpeedModifier -= strengh;
	}

	private IEnumerator JumpBoostEffect(EffectsManager manager, float strengh) {
		this.JumpModifier += strengh;
		yield return new WaitWhile(() => manager.HasJumpBoostEffect);
		this.JumpModifier -= strengh;
	}

	private IEnumerator BlurEffect(EffectsManager manager) {
		GameObject overlay = Instantiate(Resources.Load<GameObject>("BlurOverlay"));
		Image image = overlay.GetComponentInChildren<Image>();
		Color color = image.color;
		yield return this.BlurEffectTransition(image, color, true);
		color.a = 1;
		image.color = color;
		yield return new WaitWhile(() => manager.HasDrunknessEffect);
		yield return this.BlurEffectTransition(image, color, false);
		Destroy(overlay);
	}

	private IEnumerator BlurEffectTransition(Image image, Color color, bool isTransitionIn) {
		float start = Time.time;
		float length = 0.5f;
		float duration;
		while ((duration = Time.time - start) < length) {
			float progress = duration / length;
			color.a = isTransitionIn ? progress : 1 - progress;
			image.color = color;
			yield return null;
		}
	}

	private IEnumerator ShakeEffect(EffectsManager manager, float strengh) {
		while (manager.HasDrunknessEffect) {
			Vector3 delta = Random.insideUnitSphere * strengh;
            Vector3 eulerAngles = this.cameraContainer.localEulerAngles;
            eulerAngles.x = LimitCameraRot(eulerAngles.x + delta.y);
            this.cameraContainer.localEulerAngles = eulerAngles;
            this.transform.rotation = Quaternion.AngleAxis(delta.x + delta.z, this.transform.up) * this.transform.rotation;
            yield return null;
		}
	}

	private void OnSpringObjectPickedUp(SpringObjectPickedUpEvent e) {
		Vector3 velocity = this.rigidbody.velocity;
		velocity.y = 0;
		this.rigidbody.velocity = velocity;
		this.rigidbody.AddForce(0, e.Object.Force, 0, ForceMode.Impulse);
	}

	private static float LimitCameraRot(float rot) {
		if (rot < 0 || rot > 360) return rot % 360;
		if (rot > 90 && rot < 180) return 90;
		if (rot < 270 && rot >= 180) return 270;
		return rot;
	}
}
