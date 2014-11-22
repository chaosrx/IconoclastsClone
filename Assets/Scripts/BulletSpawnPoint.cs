using UnityEngine;
using System.Collections;

public class BulletSpawnPoint : MonoBehaviour {

	public Player player;

	void Start () {
	
	}

	void Update () {
	//	RepositionSpawnPoint();
	}

	void RepositionSpawnPoint()
	{
		if (player.spriteAnimator.CurrentClip == player.spriteAnimator.GetClipByName("SamusIdle"))
		{
			transform.localPosition = new Vector3(0.14f, 0.015f, 0);
		}
		if (player.spriteAnimator.CurrentClip == player.spriteAnimator.GetClipByName("SamusJumpUp") || 
		    player.spriteAnimator.CurrentClip == player.spriteAnimator.GetClipByName("SmausJumpDown"))
		{
			transform.localPosition = new Vector3(0.14f, 0.02f, 0);
		}
	}
}
