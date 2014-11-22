using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	[HideInInspector]
	public GameManager gameManager;

	[Header ("Player Movement")]
	public float gravity = -6.5f;
	public float runSpeed = 1.3f;
	public float groundDamping = 20f;
	public float inAirDamping = 6f;
	public float jumpHeight = 0.5f;

	#region Hidden References

	[HideInInspector]
	public Player player;
	[HideInInspector]
	public CharacterController2D controller;

	#endregion

	#region Properties

	public PlayerState playerState {
		get { return player.playerState; }
		set { player.playerState = value; }
	}

	public AudioManager audioManager {
		get { return gameManager.audioManager; }
	}

	public InputManager inputManager {
		get{ return gameManager.inputManager; }
	}

	#endregion

	[HideInInspector]
	public Vector3 velocity;

	private float normalizedHorizontalSpeed = 0;
	private bool _isGrounded;
	private float hangDelay;
	[HideInInspector]
	public float hangSoundDelay;

	#region init

	void Awake ()
	{
		gameManager = GameManager.Instance;
		controller = GetComponent<CharacterController2D> ();

		Subscriptions ();

		_isGrounded = controller.isGrounded;
	}

	void Subscriptions ()
	{
		controller.onControllerCollidedEvent += onControllerCollider;
		controller.onTriggerEnterEvent += onTriggerEnterEvent;
		controller.onTriggerExitEvent += onTriggerExitEvent;
	}

	#endregion

	void Update ()
	{
		velocity = controller.velocity;

		if (controller.isGrounded) {
			velocity.y = -3f;
		}

		if (player.playerState == PlayerState.Hang && inputManager.jumpButtonDown && player.spriteAnimator.CurrentClip != player.spriteAnimator.GetClipByName("RobinHangClimb")) {
			player.spriteAnimator.Play ("RobinHangClimb");
			audioManager.climbSFX.Play ();
		}
		hangSoundDelay -= Time.deltaTime;



		ApplySteering ();

		if (!controller.isGrounded) {
			if (playerState == PlayerState.Walk || playerState == PlayerState.Idle) {
				playerState = PlayerState.Jump;
			}
		}
		HangTest ();
			
		ApplyJump ();
		ApplyMovement ();

	}

	void ApplySteering ()
	{
		if (!CanSteer ()) {
			normalizedHorizontalSpeed = 0;
			return;
		}

		if (inputManager.horizontalAxis == 1) {
			player.direction = 1;
			normalizedHorizontalSpeed = 1;
			if (transform.localScale.x < 0f)
				transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);

			if (controller.isGrounded) {
				player.spriteAnimator.Play ("RobinWalk");
				playerState = PlayerState.Walk;
			}
		} else if (inputManager.horizontalAxis == -1) {
			player.direction = -1;
			normalizedHorizontalSpeed = -1;
			if (transform.localScale.x > 0f)
				transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);

			if (controller.isGrounded) {
				player.spriteAnimator.Play ("RobinWalk");
				playerState = PlayerState.Walk;
			}
		} else {
			normalizedHorizontalSpeed = 0;

			if (controller.isGrounded) {
				player.spriteAnimator.Play ("RobinIdle");
				playerState = PlayerState.Idle;
			}
		}
	}

	void ApplyMovement ()
	{
		var smoothedMovementFactor = controller.isGrounded ? groundDamping : inAirDamping;
		velocity.x = Mathf.Lerp (velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

		if (GravityEnabled ()) {
			// apply gravity before moving
			velocity.y += gravity * Time.deltaTime;
		}

		controller.move (velocity * Time.deltaTime);

		if (!_isGrounded && controller.isGrounded && hangSoundDelay <= 0) {
			audioManager.landSFX.Play ();
		} else if (_isGrounded && !controller.isGrounded) {
			if (playerState == PlayerState.Walk || playerState == PlayerState.Idle) {
				controller.velocity.y = 0;
				playerState = PlayerState.Jump;
			}
		}

		_isGrounded = controller.isGrounded;
	}

	void ApplyJump ()
	{
		if (inputManager.jumpButtonDown) {
			if (CanJump ()) {
				velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
				player.spriteAnimator.Play ("RobinJumpUp");

				audioManager.jumpSFX.Play ();

				playerState = PlayerState.Jump;
			} else if (CanDoubleJump ()) {
				velocity.y = Mathf.Sqrt (1.8f * jumpHeight * -gravity);
				player.spriteAnimator.Play ("SamusFlipBlur");

				playerState = PlayerState.DoubleJump;
			}
		}
	}

	bool CanSteer ()
	{
		if (playerState == PlayerState.Hang) {
			return false;
		}

		return true;
	}

	bool CanJump ()
	{
		if (controller.isGrounded) {
			if (playerState == PlayerState.Idle || playerState == PlayerState.Walk) {
				return true;
			}
		}
		return false;
	}

	bool CanHang ()
	{
		return true;
	}

	bool CanDoubleJump ()
	{
		return false;

		if (!controller.isGrounded) {
			if (playerState == PlayerState.Jump || playerState == PlayerState.Hang) {
				return true;
			}
		}
		return false;
	}

	bool GravityEnabled ()
	{
		if (playerState == PlayerState.Hang) {
			return false;
		}
		return true;
	}

	void HangTest ()
	{
		hangDelay -= Time.deltaTime;

		if (!CanHang ()) {
			return;
		}

		Vector3 edge = transform.position + new Vector3 (controller.boxCollider.size.x * 0.5f * player.direction, 0, 0);

		RaycastHit2D hit = Physics2D.Raycast ((Vector2)edge + new Vector2 (0, 0.1f), new Vector2 (player.direction, 0), 0.05f, 1);
		RaycastHit2D hit3 = Physics2D.Raycast ((Vector2)edge + new Vector2 (0, 0f), new Vector2 (player.direction, 0), 0.05f, 1);
		RaycastHit2D hit2 = Physics2D.Raycast ((Vector2)edge + new Vector2 (0.05f * player.direction, 0.15f), new Vector2 (0, -1), 0.1f, 1);

		RaycastHit2D hitGround = Physics2D.Raycast ((Vector2)transform.position, new Vector2 (0, -1), controller.boxCollider.size.y * 0.5f + 0.1f, 1);

		if (hit.collider != null && hit.collider == hit2.collider && hit.collider == hit3.collider &&
			velocity.y < 0 && playerState != PlayerState.Hang && hangDelay <= 0 && hitGround.collider == null) {

			RaycastHit2D hit4 = Physics2D.Raycast ((Vector2)edge + new Vector2 (0, 0.15f), new Vector2 (player.direction, 0), 0.05f, 1);

			if (Vector2.Distance (new Vector2 (edge.x, 0), new Vector2 (hit.point.x, 0)) <= 0.025f && hit4.collider != hit.collider) {
				print (new Vector2 (hit.point.x, hit2.point.y));
				playerState = PlayerState.Hang;
				velocity.y = 0;
				controller.velocity.y = 0;
				player.spriteAnimator.Play ("RobinHangCling");
				audioManager.hangSFX.Play ();
				player.transform.position = new Vector3 (hit.point.x - (controller.boxCollider.size.x * 0.5f * player.direction), hit2.point.y - 0.15f, transform.position.z);
			}
		}

		if (playerState == PlayerState.Hang) {

			if (inputManager.verticalAxis < 0) {
				playerState = PlayerState.Jump;
				hangDelay = 0.2f;
			}
		}

	}

	void OnDrawGizmosSelected ()
	{

		Gizmos.color = Color.red;

		Vector3 edge = transform.position + new Vector3 (0.18f * 0.5f, 0, 0);

		Gizmos.DrawLine (edge + new Vector3 (0, 0.1f, 0), edge + new Vector3 (0.05f, 0.1f, 0));
		Gizmos.DrawLine (edge + new Vector3 (0.05f, 0.2f, 0), edge + new Vector3 (0.05f, 0.0f, 0));
	}

	#region Controller Event Listeners

	void onControllerCollider (RaycastHit2D hit)
	{
		// bail out on plain old ground hits cause they arent very interesting
		if (hit.normal.y == 1f)
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent (Collider2D col)
	{
		Debug.Log ("onTriggerEnterEvent: " + col.gameObject.name);
	}


	void onTriggerExitEvent (Collider2D col)
	{
		Debug.Log ("onTriggerExitEvent: " + col.gameObject.name);
	}

	#endregion
}
