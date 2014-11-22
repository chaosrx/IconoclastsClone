using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Vector3 topLeft;
	public Vector3 topRight;
	public Vector3 bottomLeft;
	public Vector3 bottomRight;

	public Transform playerPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		print (camera.aspect);
	}

	void OnDrawGizmosSelected () {
		// Display the explosion radius when selected
		Gizmos.color = Color.green;
		//Gizmos.DrawSphere (transform.position, explosionRadius);
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(bottomRight, topRight);
		Gizmos.DrawLine(topLeft, bottomLeft);
		Gizmos.DrawLine(bottomLeft, bottomRight);
	}

	void LateUpdate()
	{
		transform.position = new Vector3(playerPosition.position.x, playerPosition.position.y, transform.position.z);

		float c1 = bottomLeft.x + camera.orthographicSize*camera.aspect;
		float c2 = bottomRight.x - camera.orthographicSize*camera.aspect;
		float c3 = bottomLeft.x + camera.orthographicSize*camera.aspect;
		float c4 = bottomRight.x - camera.orthographicSize*camera.aspect;

		Vector3 pos = new Vector3(Mathf.Clamp(transform.position.x, bottomLeft.x + camera.orthographicSize*camera.aspect, bottomRight.x - camera.orthographicSize*camera.aspect),
		                          Mathf.Clamp (transform.position.y, bottomRight.y + camera.orthographicSize, topRight.y - camera.orthographicSize),
		                          transform.position.z);
		transform.position = pos;
	}
}
