using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	
	// The distance in the x-z plane to the target
	float distance = 10.0f;
	// the height we want the camera to be above the target
	float height = 5.0f;
	// How much we 
	float heightDamping = 2.0f;
	float rotationDamping = 3.0f;

	public TeamManager _tm;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject target = _tm.mainPlayer;

		// Early out if we don't have a target
		if (target == null)
			return;
		
		// Calculate the current rotation angles
		float wantedRotationAngle = target.transform.eulerAngles.y;
		float wantedHeight = target.transform.position.y + height;
		
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.transform.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		
		// Set the height of the camera
		transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
		
		// Always look at the target
		transform.LookAt (target.transform);
	}
}
