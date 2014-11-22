using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour 
{
	[HideInInspector]
	public Player player;
	[HideInInspector]
	public GameManager gameManager;

	public tk2dSpriteAnimator spriteAnimator;

	private float delayScrewAttackSound;
	private float delayFootStepsSound;
	private float delayFootStepsSoundAfterJump;

	private int foot;

	void Awake(){
		spriteAnimator.AnimationCompleted += HandleAnimationCompleted;
		gameManager = GameManager.Instance;
	}

	void Update()
	{
		JumpAnimation ();

		SoundEffects ();
	}

	void JumpAnimation()
	{
		if (player.playerState == PlayerState.Jump) {
			if (player.playerMovement.velocity.y > 0) {
				if (player.spriteAnimator.CurrentClip != player.spriteAnimator.GetClipByName ("RobinJumpUp")) {
					player.spriteAnimator.Play ("RobinJumpUp");
				}
			} else {
				if (player.spriteAnimator.CurrentClip != player.spriteAnimator.GetClipByName ("RobinJumpDown")) {
					player.spriteAnimator.Play ("RobinJumpDown");
				}
			}
		}
	}

	void SoundEffects()
	{
		if (player.spriteAnimator.CurrentClip == player.spriteAnimator.GetClipByName("RobinWalk"))
		{
			if(!gameManager.audioManager.footStepsSFX.isPlaying)
			{
				if (delayFootStepsSound <= 0 && delayFootStepsSoundAfterJump <= 0)
				{
					if (foot == 0) {
						gameManager.audioManager.footStepsSFX.Play ();
						foot = 1;
					} else {
						gameManager.audioManager.footSteps2SFX.Play ();
						foot = 0;
					}
					delayFootStepsSound = 0.26f;
				}
				delayFootStepsSound -= Time.deltaTime;
			}
		}

		if (player.playerState != PlayerState.Walk && player.playerState != PlayerState.Idle) {
			delayFootStepsSoundAfterJump = 0.23f;
		} else {
			delayFootStepsSoundAfterJump -= Time.deltaTime;
		}
	}

	void HandleAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
	{
		if (clip == animator.GetClipByName ("RobinHangCling")) {
			if (player.playerState == PlayerState.Hang) {
				player.spriteAnimator.Play ("RobinHangDangle");
			}
		}

		if (clip == animator.GetClipByName ("RobinHangClimb")) {
			if (player.playerState == PlayerState.Hang) {
				transform.position += new Vector3 (player.direction * 0.235f, 0.43f, 0);
				player.spriteAnimator.Play ("RobinHangStandUp");
			}
		}

		if (clip == animator.GetClipByName ("RobinHangStandUp")) {
			if (player.playerState == PlayerState.Hang) {
				player.playerMovement.hangSoundDelay = 0.2f;
				player.playerState = PlayerState.Idle;
			}
		}
	}
}
