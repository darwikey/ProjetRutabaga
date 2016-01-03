using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

    public Material _hiddenMaterial;
    public Material _builtMaterial;
    int _life = 0;


	// Initialization
	void Start () {
        UpdateObstacle();
	}
	
	public void Built()
    {
        _life = 10;
        UpdateObstacle();
    }

    public void SetDamage(int damage)
    {
        _life -= damage;
        UpdateObstacle();
    }

    void UpdateObstacle()
    {
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
}
