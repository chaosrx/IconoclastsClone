using UnityEngine;
using System.Collections;

public class ParallaxTest : MonoBehaviour {

	public float value = -1; 

	public Vector3 offset;
	public float valueY = -1; 

	public Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = new Vector3 (Camera.main.transform.position.x * value, Camera.main.transform.position.y * valueY, transform.position.z)+offset;

		//pos = new Vector3( Mathf.Round (pos.x * 100) / 100, Mathf.Round (pos.y * 100) / 100, pos.z);

		transform.position = pos;
	}
}
