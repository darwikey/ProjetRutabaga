using UnityEngine;
using System.Collections;

public class IAController : MonoBehaviour {

    NavMeshAgent _agent;
    Player _player;
    GameObject[] _zones;

	// Use this for initialization
	void Start () {
        _agent = GetComponent<NavMeshAgent>();
        _player = GetComponent<Player>();
        _zones = GameObject.FindGameObjectsWithTag("Zone");


    }

    // Update is called once per frame
    void Update () {
        if (_player == null)
            Destroy(this);
        //set destination
        GameObject target = targetZone();
        if(target != null)
        _agent.SetDestination(target.transform.position);

        manageGrenade();
        /* TODO
            launch grenade
            specific behaviour
        */
	}


    // decide the zone where the bot should go
    GameObject targetZone()
    {
        GameObject target = null;
        NavMeshPath path = new NavMeshPath();

        ZoneBehaviour z;

        foreach (GameObject zone in _zones)
        {
            z = zone.GetComponent<ZoneBehaviour>();
            if (z.ownerTeam != _player.team|| z.dangerLevel > 0)
            {
                if (target == null)
                    target = zone;
                else
                {
                    if (Vector3.Distance(transform.position, zone.transform.position) < Vector3.Distance(transform.position, target.transform.position))
                        target = zone;
                }
            }
        }
        return target;
    }

    // decide in wich zone we trow a grenade
    GameObject  targetGrenadeZone()
    {
        GameObject target = null;
        ZoneBehaviour z;

        foreach (GameObject zone in _zones)
        {
            z = zone.GetComponent<ZoneBehaviour>();
            if (z.getTeamInArea() == _player.enemyteam() && z.dangerLevel >= 2 && Vector3.Distance(z.transform.position, transform.position) < 50.0f)
            {
                if (target == null)
                    target = zone;
                else
                {
                    if (Vector3.Distance(transform.position, zone.transform.position) < Vector3.Distance(transform.position, target.transform.position))
                        target = zone;
                }
            }
        }
        return target;

    }


    // manage the grenade launching
    void manageGrenade()
    {
        //if we can't launch a grenade
        if (_player._launchGrenadeTimer < Player.GRENADE_LAUNCH_TIME || _player._numGrenade < 1)
            return;

        GameObject target = targetGrenadeZone();
        if (target == null)
            return;

        _player._launchGrenadeTimer = 0.0f;
        _player._numGrenade--;

        Vector3 vel = target.transform.position - transform.position;
        vel /= 20.0f;
        vel *= 0.75f;//anticipate roll
        vel.y = 0.15f;
        GameObject grenade = Object.Instantiate(_player._grenadePrefab, transform.position + 0.5f * vel, Quaternion.identity) as GameObject;
        grenade.GetComponent<Rigidbody>().velocity = 20.0f * vel;

    }
}
