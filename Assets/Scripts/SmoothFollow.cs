using UnityEngine;
using System.Collections;



public class SmoothFollow : MonoBehaviour
{
	public Vector3 topLeft;
	public Vector3 topRight;
	public Vector3 bottomLeft;
	public Vector3 bottomRight;

	public Transform target;
	public float smoothDampTime = 0.2f;
	[HideInInspector]
	public new Transform transform;
	public Vector3 cameraOffset;
	public bool useFixedUpdate = false;
	
	private CharacterController2D _playerController;
	private Vector3 _smoothDampVelocity;
	
	
	void Awake()
	{
		transform = gameObject.transform;
		_playerController = target.GetComponent<CharacterController2D>();

	}
	
	
	void LateUpdate()
	{
		//print (camera.pixelHeight);

		//camera.orthographicSize = (Screen.height / 400);

		if( !useFixedUpdate ){
			updateCameraPosition();
			ClampCamera();
		}


	}


	void FixedUpdate()
	{
		if( useFixedUpdate ){
			updateCameraPosition();
			ClampCamera();
		}
	}

	void OnDrawGizmosSelected () {
		Gizmos.color = Color.green;
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(bottomRight, topRight);
		Gizmos.DrawLine(topLeft, bottomLeft);
		Gizmos.DrawLine(bottomLeft, bottomRight);
	}
	
	void ClampCamera()
	{		
		Vector3 pos = new Vector3(Mathf.Clamp(transform.position.x, bottomLeft.x + camera.orthographicSize*camera.aspect, bottomRight.x - camera.orthographicSize*camera.aspect),
		                          Mathf.Clamp (transform.position.y, bottomRight.y + camera.orthographicSize, topRight.y - camera.orthographicSize),
		                          transform.position.z);
			
		Vector3 newPos = new Vector3 (Mathf.Round (pos.x * 100) / 100, Mathf.Round (pos.y * 100) / 100, -10);

		transform.position = newPos;

	}


	void updateCameraPosition()
	{
		if( _playerController == null )
		{
			transform.position = Vector3.SmoothDamp( transform.position, target.position - cameraOffset, ref _smoothDampVelocity, smoothDampTime );
			return;
		}
		
		if( _playerController.velocity.x > 0 )
		{
			transform.position = Vector3.SmoothDamp( transform.position, target.position - cameraOffset, ref _smoothDampVelocity, smoothDampTime );
		}
		else
		{
			var leftOffset = cameraOffset;
			leftOffset.x *= -1;
			transform.position = Vector3.SmoothDamp( transform.position, target.position - leftOffset, ref _smoothDampVelocity, smoothDampTime );
		}
	}
	
}
