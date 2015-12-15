using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	
	// The distance in the x-z plane to the target
	public float distance = 20.0f;
	// the height we want the camera to be above the target
	public float height = 10.0f;
	// How much we smooth the movement
	public float heightDamping = 2.0f;

	TeamManager _tm;


	// Use this for initialization
	void Start () {
        _tm = GameObject.Find("TeamManager").GetComponent<TeamManager>();
	}
	
	// Update is called once per frame
	void Update () {
		GameObject target = _tm.mainPlayer;
		target.GetComponent<Player>().mainCamera = GetComponent<Camera>();

		// Early out if we don't have a target
		if (target == null)
			return;
		

		float wantedHeight = target.transform.position.y + height;

		float currentHeight = transform.position.y;
		

		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		

		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.transform.position;

		// Set the height of the camera
		transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z + distance);
		
		// Always look at the target
		transform.LookAt (target.transform);
	}
}
