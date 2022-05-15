using System.Collections;
using Events;
using Objects;
using UnityEngine;
using UnityEngine.UI;

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
		// TODO move in game manager
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		EventManager.Instance.AddListener<PlayerSpawnedEvent>(this.OnPlayerSpawned);
		EventManager.Instance.AddListener<PlayerTrappedEvent>(this.OnPlayerTrapped);
		EventManager.Instance.AddListener<SnailObjectPickedUpEvent>(this.OnSnailObjectPickedUp);
		EventManager.Instance.AddListener<CheeseObjectPickedUpEvent>(this.OnCheeseObjectPickedUp);
		EventManager.Instance.AddListener<FrogLegObjectPickedUpEvent>(this.OnFrogLegObjectPickedUp);
		EventManager.Instance.AddListener<WineBottleObjectPickedUpEvent>(this.OnWineBottleObjectPickedUp);
	}

	protected virtual void OnDestroy() {
		EventManager.Instance.RemoveListener<PlayerSpawnedEvent>(this.OnPlayerSpawned);
		EventManager.Instance.RemoveListener<PlayerTrappedEvent>(this.OnPlayerTrapped);
		EventManager.Instance.RemoveListener<SnailObjectPickedUpEvent>(this.OnSnailObjectPickedUp);
		EventManager.Instance.RemoveListener<CheeseObjectPickedUpEvent>(this.OnCheeseObjectPickedUp);
		EventManager.Instance.RemoveListener<FrogLegObjectPickedUpEvent>(this.OnFrogLegObjectPickedUp);
		EventManager.Instance.RemoveListener<WineBottleObjectPickedUpEvent>(this.OnWineBottleObjectPickedUp);
	}

	private void FixedUpdate() {
		// Get player & controls status
		bool onGround = this.OnGround;
		float vInput = Input.GetAxis("Vertical");
		float hInput = Input.GetAxis("Horizontal");
		float mouseXInput = Input.GetAxisRaw("Mouse X");
		float mouseYInput = Input.GetAxisRaw("Mouse Y");

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
		float rot = angle.x - xRot;
		if (rot < 0 || rot > 360) rot %= 360;
		if (rot > 90 && rot < 180) rot = 90;
		if (rot < 270 && rot >= 180) rot = 270;
		this.cameraContainer.localEulerAngles = new Vector3(rot, angle.y, angle.z);

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

	private void OnSnailObjectPickedUp(SnailObjectPickedUpEvent e) {
		this.StartCoroutine(this.SnailEffect(e.Object));
	}

	private IEnumerator SnailEffect(SnailObject snail) {
		this.SpeedModifier -= snail.Strengh;
		yield return new WaitForSeconds(snail.Duration);
		this.SpeedModifier += snail.Strengh;
	}

	private void OnCheeseObjectPickedUp(CheeseObjectPickedUpEvent e) {
		this.StartCoroutine(this.CheeseEffect(e.Object));
	}

	private IEnumerator CheeseEffect(CheeseObject cheese) {
		this.SpeedModifier += cheese.Strengh;
		yield return new WaitForSeconds(cheese.Duration);
		this.SpeedModifier -= cheese.Strengh;
	}

	private void OnFrogLegObjectPickedUp(FrogLegObjectPickedUpEvent e) {
		this.StartCoroutine(this.FrogLegEffect(e.Object));
	}

	private IEnumerator FrogLegEffect(FrogLegObject frogLeg) {
		this.JumpModifier += frogLeg.Strengh;
		yield return new WaitForSeconds(frogLeg.Duration);
		this.JumpModifier -= frogLeg.Strengh;
	}

	private void OnWineBottleObjectPickedUp(WineBottleObjectPickedUpEvent e) {
		this.StartCoroutine(this.WineBottleEffect(e.Object));
		this.StartCoroutine(this.ShakeEffect(e.Object.Duration, e.Object.Strengh));
	}

	private IEnumerator WineBottleEffect(WineBottleObject wineBottle) {
		GameObject overlay = Instantiate(Resources.Load<GameObject>("BlurOverlay"));
		Image image = overlay.GetComponentInChildren<Image>();
		Color color = image.color;
		yield return this.BlurEffectTransition(image, color, true);
		color.a = 1;
		image.color = color;
		yield return new WaitForSeconds(wineBottle.Duration - 1);
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

	private IEnumerator ShakeEffect(float length, float strengh) {
		float start = Time.time;
		while (Time.time - start < length) {
			Vector3 delta = Random.insideUnitSphere * strengh;
            Vector3 eulerAngles = this.cameraContainer.localEulerAngles;
            eulerAngles.x += delta.y;
            this.cameraContainer.localEulerAngles = eulerAngles;
            this.transform.rotation = Quaternion.AngleAxis(delta.x + delta.z, this.transform.up) * this.transform.rotation;
            yield return null;
		}
	}
}
