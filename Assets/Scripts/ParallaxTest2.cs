using UnityEngine;
using System.Collections;

public class ParallaxTest2 : MonoBehaviour
{

	public Vector3 topLeft;
	public Vector3 topRight;
	public Vector3 bottomLeft;
	public Vector3 bottomRight;

	public Vector3 topLeft2;
	public Vector3 topRight2;
	public Vector3 bottomLeft2;
	public Vector3 bottomRight2;

	public Transform target;

	public bool x;
	public bool y;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		Vector3 pos = transform.position;

		if (x) {
			pos.x = Mathf.Lerp (bottomLeft.x, bottomRight.x, Mathf.InverseLerp (bottomLeft2.x, bottomRight2.x, target.position.x));
		}
		if (y) {
			pos.y = Mathf.Lerp (bottomLeft.y, topRight.y, Mathf.InverseLerp (bottomLeft2.y, topRight2.y, target.position.y));
		}


		transform.position = pos;

	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine (topLeft, topRight);
		Gizmos.DrawLine (bottomRight, topRight);
		Gizmos.DrawLine (topLeft, bottomLeft);
		Gizmos.DrawLine (bottomLeft, bottomRight);

		Gizmos.color = Color.red;
		Gizmos.DrawLine (topLeft2, topRight2);
		Gizmos.DrawLine (bottomRight2, topRight2);
		Gizmos.DrawLine (topLeft2, bottomLeft2);
		Gizmos.DrawLine (bottomLeft2, bottomRight2);
	}
}
