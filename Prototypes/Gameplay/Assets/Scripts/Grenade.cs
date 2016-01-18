using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

    public float _timeout = 3.0f;
    public float _blastRadius = 10.0f;
    public GameObject _explosionPrefab;

    float _timer = 0.0f;
    TeamManager _tm;
	ObstacleManager _om;

    // Use this for initialization
    void Start () {
        _tm = GameObject.Find("TeamManager").GetComponent<TeamManager>();
		_om = GameObject.Find("ObstacleManager").GetComponent<ObstacleManager>();
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

                    /*check if there is obstacle on the way to protect the target*/
                    bool isprotected = false;
                    RaycastHit rayHit;
                    Vector3 dir = (player.transform.position - transform.position).normalized;
                    if (Physics.Raycast(transform.position + dir * 0.1f, dir, out rayHit, 100))
                    {
                        Player hitPlayer = rayHit.collider.GetComponent<Player>();
                        isprotected = (hitPlayer == null);
                    }
                    if (!isprotected)
                    {
                        float damage = 100.0f * ((_blastRadius - d) / _blastRadius);
                        player.SetDamage(damage);
                        dir.y = 0.1f;
                        // direction of ejection 
                        player.GetComponent<Rigidbody>().velocity = 15.0f * dir;
                    }
                }
            }

			// damages on obstacles
			foreach(Obstacle obstacle in _om.obstacles)
			{
				if (Vector3.Distance(transform.position, obstacle.transform.position) < _blastRadius)
				{
					obstacle.SetDamage(3);
				}
			}

            // particles
            GameObject ps = Instantiate(_explosionPrefab, transform.position, Quaternion.identity) as GameObject;
            Destroy(ps, 1.2f);

            // explode
            Destroy(gameObject);
        }
	}
}
