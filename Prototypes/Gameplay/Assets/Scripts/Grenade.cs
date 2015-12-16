using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

    public float _timeout = 3.0f;
    public float _blastRadius = 10.0f;
    float _timer = 0.0f;
    TeamManager _tm;

    // Use this for initialization
    void Start () {
        _tm = GameObject.Find("TeamManager").GetComponent<TeamManager>();
    }
	
	// Update is called once per frame
	void Update () {
        _timer += Time.deltaTime;
        if (_timer > _timeout)
        {
            // explosion blast
            foreach (Player player in _tm.getPlayerList())
            {
                float d = Vector3.Distance(transform.position, player.transform.position);
                // in the blast radius
                if (d < _blastRadius)
                {
                    float damage = 100.0f * ((_blastRadius - d) / _blastRadius);
                    player.SetDamage(damage);
                    Vector3 dir = player.transform.position - transform.position;
                    dir.Normalize();
                    dir.y = 0.1f;
                    // direction of ejection 
                    player.GetComponent<Rigidbody>().velocity = 15.0f * dir;
                }
            }

            // explode
            Destroy(gameObject);
        }
	}
}
