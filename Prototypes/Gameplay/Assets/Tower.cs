using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

	public GameObject _player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ((transform.position - _player.transform.position).sqrMagnitude < 150.0f) {

		}
	}
}
