using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

    public Material _hiddenMaterial;
    public Material _builtMaterial;
    int _life = 0;


	// Initialization
	void Start () {
        updateObstacle();
	}
	
	public void built()
    {
        _life = 10;
        updateObstacle();
    }

    public void setDamage(int damage)
    {
        _life -= damage;
        updateObstacle();
    }

    void updateObstacle()
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
