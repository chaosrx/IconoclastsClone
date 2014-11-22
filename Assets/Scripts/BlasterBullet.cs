using UnityEngine;
using System.Collections;

public class BlasterBullet : MonoBehaviour {

	[HideInInspector]
	public int direction;

	public tk2dSpriteAnimator spriteAnimator;
	public BlasterState blasterState;

	public float blasterSpeed = 10;

	public LayerMask layerMask;

	private RaycastHit rHit;

	public void LaunchBullet(int direction)
	{
		if (direction == 1)
		{
			spriteAnimator.Sprite.scale = Vector3.one;
		}else
		{
			spriteAnimator.Sprite.scale = new Vector3(-1,1,1);
		}

		this.direction = direction;

		blasterState = BlasterState.Launched;
	}

	void Update()
	{
		RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, new Vector2(direction,0), blasterSpeed*Time.deltaTime, layerMask.value);
		
		if (hit.collider != null)
		{
			SpawnManager.SpawnPrefab(Spawn.BlasterHitFX, (Vector3)hit.point);
			SpawnManager.DespawnPrefab(this.transform, PoolName.Default);
		}

		if (blasterState == BlasterState.Launched){
			transform.Translate(new Vector3(direction * blasterSpeed * Time.deltaTime, 0, 0));
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		print (col.gameObject.layer + " " + layerMask.value);

		if (col.gameObject.layer == 0)
		{
			//SpawnManager.SpawnPrefab(Spawn.BlasterHitFX, (Vector3)transform.position);
			//SpawnManager.DespawnPrefab(this.transform, PoolName.Default);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
	}
}

public enum BlasterState
{
	Idle,
	Launched,
	Hit
}
