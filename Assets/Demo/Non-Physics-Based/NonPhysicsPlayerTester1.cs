using UnityEngine;
using System.Collections;


public class NonPhysicsPlayerTester1 : MonoBehaviour
{
	public PlayerState playerState = PlayerState.Idle;

	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	public Player player;
	private RaycastHit2D _lastControllerColliderHit;
	public Vector3 _velocity;

	private int jumpCount;

	//_____________________
	public AudioManager audioManager;

	private bool tempGrounded;
	private float delayScrewAttackSound;
	
	public Transform bulletSpawnPoint;
	private float blasterCooldown;

	public LayerMask layerMask;

	void Awake()
	{
		Application.targetFrameRate = 60;
		_controller = GetComponent<CharacterController2D>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;

		Subscriptions();

		tempGrounded = _controller.isGrounded;
	}


	void Subscriptions()
	{
		player.spriteAnimator.AnimationEventTriggered += HandleAnimationEventTriggered;
	}

	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent( Collider2D col )
	{
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		blasterCooldown -= Time.deltaTime;

		if (Input.GetButtonDown("Fire1") && blasterCooldown <= 0)
		{
			blasterCooldown = 0.15f;
			audioManager.blasterSFX.Play();
			SpawnManager.SpawnPrefab(Spawn.BlasterSparkFX, bulletSpawnPoint.position);
			BlasterBullet bullet = SpawnManager.SpawnPrefab(Spawn.Blaster, bulletSpawnPoint.position).GetComponent<BlasterBullet>();
			bullet.LaunchBullet(player.direction);
		}

		SoundEffects();

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

		if( _controller.isGrounded ){
			_velocity.y = 0;
			jumpCount = 0;
		}

		if( Input.GetAxisRaw("Horizontal") == 1)
		{
			player.direction = 1;
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded ){
				player.spriteAnimator.Play("SamusWalk");
				playerState = PlayerState.Walk;
			}
		}
		else if( Input.GetAxisRaw("Horizontal") == -1)
		{
			player.direction = -1;
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded ){
				player.spriteAnimator.Play("SamusWalk");
				playerState = PlayerState.Walk;
			}
		}
		else
		{
			normalizedHorizontalSpeed = 0;

			if( _controller.isGrounded ){
				player.spriteAnimator.Play("SamusIdle");
				playerState = PlayerState.Idle;
			}
		}

		// we can only jump whilst grounded
		if( CanJump() && Input.GetButtonDown("Jump") )
		{
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
			player.spriteAnimator.Play("SamusJumpUp");
			jumpCount = 1;

			audioManager.jumpSFX.Play();

			playerState = PlayerState.Jump;
		}

		if (CanFlip() && Input.GetButtonDown("Jump"))
		{
			_velocity.y = Mathf.Sqrt( 1.8f * jumpHeight * -gravity );
			player.spriteAnimator.Play("SamusFlipBlur");
			jumpCount = 2;

			playerState = PlayerState.DoubleJump;
		}

		if (!_controller.isGrounded && jumpCount < 2 && (playerState == PlayerState.Jump || playerState == PlayerState.Idle)){
			if (_velocity.y > 0)
			{
				if (player.spriteAnimator.CurrentClip != player.spriteAnimator.GetClipByName("SamusJumpUp"))
				{
					player.spriteAnimator.Play("SamusJumpUp");
				}
			}else
			{
				if (player.spriteAnimator.CurrentClip != player.spriteAnimator.GetClipByName("SamusJumpDown"))
				{
					player.spriteAnimator.Play("SamusJumpDown");
				}
			}
		}

		// apply horizontal speed smoothing it
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		if (playerState != PlayerState.Hang){
			// apply gravity before moving
			_velocity.y += gravity * Time.deltaTime;
		}

		_controller.move( _velocity * Time.deltaTime );

		if (!tempGrounded && _controller.isGrounded)
		{
			audioManager.landSFX.Play();
		}else if (tempGrounded && !_controller.isGrounded)
		{
			if (playerState == PlayerState.Walk)
			{
				playerState = PlayerState.Jump;
			}
		}

		tempGrounded = _controller.isGrounded;

		HangTest();
	}

	void HangTest()
	{
		Vector3 edge = transform.position + new Vector3(_controller.boxCollider.size.x*0.5f*player.direction, 0, 0);

		print (player.direction);

		RaycastHit2D hit = Physics2D.Raycast((Vector2)edge+new Vector2(0, 0.1f), new Vector2(player.direction,0), 0.05f, layerMask.value);
		RaycastHit2D hit2 = Physics2D.Raycast((Vector2)edge+new Vector2(0.05f*player.direction, 0.2f), new Vector2(0,-1), 0.1f, layerMask.value);

		if (hit.collider != null && hit.collider == hit2.collider && _velocity.y < 0 && playerState != PlayerState.Hang)
		{
			if (Vector2.Distance(new Vector2(edge.x, 0), new Vector2(hit.point.x,0)) == 0){
				print (new Vector2(hit.point.x, hit2.point.y));
				playerState = PlayerState.Hang;
				_velocity.y = 0;
				_controller.velocity.y = 0;
				player.spriteAnimator.Play("SamusHang");
				audioManager.hangSFX.Play();
				player.transform.position = new Vector3(transform.position.x, hit2.point.y-0.15f, transform.position.z);
			}
		}

	}

	void OnDrawGizmosSelected()
	{

		Gizmos.color = Color.red;

		//if (_controller.boxCollider != null){
			//Vector3 edge = transform.position + new Vector3(_controller.boxCollider.size.x*0.5f, 0, 0);

			//Gizmos.DrawLine(edge+new Vector3(0, 0.1f, 0), edge+new Vector3(0.05f,0.1f,0));
			//Gizmos.DrawLine(edge+new Vector3(0.05f, 0.2f, 0), edge+new Vector3(0.05f,0.1f,0));
		//}
	}


	//____________
	void SoundEffects()
	{
		if (player.spriteAnimator.CurrentClip == player.spriteAnimator.GetClipByName("SamusFlipBlur"))
		{
			if(!audioManager.screwAttackSFX.isPlaying)
			{
				if (delayScrewAttackSound <= 0)
				{
					audioManager.screwAttackSFX.Play();
					delayScrewAttackSound = 0.035f;
				}
				delayScrewAttackSound -= Time.deltaTime;
			}
		}
	}

	void HandleAnimationEventTriggered(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip, int frame)
	{
		if (clip == anim.GetClipByName("SamusWalk"))
		{
			audioManager.footStepsSFX.Play();
		}
	}

	bool CanJump()
	{
		if (_controller.isGrounded)
		{
			if(playerState == PlayerState.Idle || playerState == PlayerState.Walk)
			{
				return true;
			}
		}

		return false;
	}

	bool CanFlip()
	{
		if (!_controller.isGrounded && jumpCount < 2 && playerState == PlayerState.Jump || playerState == PlayerState.Hang)
		{
			return true;
		}

		return false;
	}
}
