using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public PlayerState playerState = PlayerState.Idle;

	#region Hidden
	[HideInInspector]
	public GameManager gameManager;
	[HideInInspector]
	public PlayerMovement playerMovement;
	[HideInInspector]
	public PlayerAnimation playerAnimation;

	#endregion

	#region Properties

	public tk2dSpriteAnimator spriteAnimator {
		get {
			return playerAnimation.spriteAnimator;
		}
	}

	#endregion

	[HideInInspector]
	public int direction = 1;

	void Awake ()
	{
		gameManager = GameManager.Instance;
		playerMovement = GetComponent<PlayerMovement> ();
		playerAnimation = GetComponent<PlayerAnimation> ();
		playerMovement.player = this;
		playerAnimation.player = this;
	}
}

public enum PlayerState
{
	Idle,
	Walk,
	Jump,
	DoubleJump,
	Hang
}