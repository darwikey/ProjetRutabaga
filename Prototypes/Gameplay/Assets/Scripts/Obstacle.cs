using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

    public Material _hiddenMaterial;
    public Material _builtMaterial;
    int _life = 0;


	// Use this for initialization
	void Start () {
	    if (_life <= 0)
        {
            GetComponent<MeshRenderer>().material = _hiddenMaterial;
            GetComponent<MeshCollider>().enabled = false;
        }
        else
        {
            GetComponent<MeshRenderer>().material = _builtMaterial;
            GetComponent<MeshCollider>().enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
