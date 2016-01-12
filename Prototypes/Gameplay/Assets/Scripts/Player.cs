using UnityEngine;
using System.Collections;

public abstract class Player : MonoBehaviour
{
	public enum Type
	{
		MARKSMAN,
		PROTECTOR,
		ENGINEER
	}

	// if not null, this player is the main player
	protected Camera _mainCamera;

	public int _team = 1;
    protected TeamManager _tm;

    protected ObstacleManager _obstacleManager;

    protected float _health = 0.0f;

	GameObject _minimapIcon;
	GameObject _fogMask;

    // grenades
    protected int _numGrenade = 99;
    protected float _launchGrenadeTimer = 0.0f;
    protected GameObject _grenadePrefab;

    protected static float GRENADE_LAUNCH_TIME = 2.0f;

    protected GameObject[] _zones;
    protected GameObject[] _foes;
    protected GameObject[] _teammates;

    //an object used for debug 
    protected GameObject _debugChild;

    protected ThirdPersonController _tpc;

    // When the player spawn
    public virtual void Start()
    {
		_health = 100.0f;

        _tm = GameObject.Find("TeamManager").GetComponent<TeamManager>();
        _obstacleManager = GameObject.Find("ObstacleManager").GetComponent<ObstacleManager>();
        _tpc = GetComponent<ThirdPersonController>();

        // Minimap icon
        if (_minimapIcon == null) {
			_minimapIcon = GameObject.CreatePrimitive (PrimitiveType.Quad);
			_minimapIcon.name = "Icon";
			_minimapIcon.GetComponent<MeshCollider> ().enabled = false;
			_minimapIcon.transform.SetParent (transform);
			_minimapIcon.transform.localPosition = new Vector3 (0.0f, 1.5f, 0.0f);
			_minimapIcon.transform.rotation = Quaternion.Euler (90.0f, 0.0f, 0.0f);
			_minimapIcon.transform.localScale = 9.0f * Vector3.one;
			_minimapIcon.layer = 10;
			
			if (team == 1) {
				_minimapIcon.GetComponent<Renderer> ().material = Resources.Load ("team1Minimap") as Material;
			} else {
				_minimapIcon.GetComponent<Renderer> ().material = Resources.Load ("team2Minimap") as Material;
			}
		}

		// Fog mask
		if (_team == 1 && _fogMask == null) 
		{
			_fogMask = GameObject.CreatePrimitive (PrimitiveType.Quad);
			_fogMask.name = "FogMask";
			_fogMask.GetComponent<MeshCollider> ().enabled = false;
			_fogMask.transform.SetParent (transform);
			_fogMask.transform.localPosition = new Vector3 (0.0f, 1.5f, 0.0f);
			_fogMask.transform.rotation = Quaternion.Euler (90.0f, 0.0f, 0.0f);
			_fogMask.transform.localScale = 16.0f * Vector3.one;
			_fogMask.layer = 9;
			_fogMask.GetComponent<Renderer>().material = Resources.Load ("FogMaskMat") as Material;
		}

        // grenade
        _grenadePrefab = Resources.Load("Grenade") as GameObject;

        if (!isMainPlayer())
            AI_Start();


    }


    // Update is called once per frame
    protected virtual void Update () {

        _tpc.isControllable = isMainPlayer();

        // activate navmesh for bots
        //GetComponent<NavMeshAgent>().enabled = !isMainPlayer();



        // Grenade
        if (_launchGrenadeTimer > GRENADE_LAUNCH_TIME && _numGrenade > 0)
        {
            // main player launch a grenade
            if (mainCamera != null)
            {
                // middle mouse button or G key
                if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.G))
                {
                    launchGrenade();  
                }
            }
        }

        /* BOT AI */
        if(mainCamera == null)
        {
            AI_Update();
        }

        _launchGrenadeTimer += Time.deltaTime;
    }


    protected void launchGrenade()
    {
        _launchGrenadeTimer = 0.0f;
        _numGrenade--;
        
        Vector3 vel = getCursorWorldPosition() - transform.position;
        vel /= 20.0f;
        //  vel.Normalize();
        vel.y = 0.15f;
        GameObject grenade = Object.Instantiate(_grenadePrefab, transform.position + 0.5f * vel, Quaternion.identity) as GameObject;
        grenade.GetComponent<Rigidbody>().velocity = 20.0f *  vel;
    }


    // Raycast for the mouse
    public Vector3 getCursorWorldPosition()
	{
		RaycastHit mouseHit;
		if (!Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out mouseHit, 100))
			return Vector3.zero;

        return mouseHit.point; 
	}

	public void SetDamage(float damage)
	{
        bool isProtected = false;
        foreach(Player p in _tm.getPlayerList(_team))
        {
            if (p.playerType == Type.PROTECTOR)
            {
                if (Vector3.Distance(transform.position, p.transform.position) < 6.5f)
                {
                    isProtected = true;
                    break;
                }
            }
        }

        if (isProtected)
        {
            //Debug.Log("protected");
            damage *= 0.5f; 
        }
		_health -= damage;
	}


    /* ========================================================================================= */
    /* AI */


    protected virtual void AI_Start()
    {
        _zones = GameObject.FindGameObjectsWithTag("Zone");


        if (_debugChild == null)
        {
            _debugChild = transform.Find("DebugLinePath").gameObject;

        }
    }
    /* update AI choices */
    protected virtual void AI_Update()
    {
        //set destination
        GameObject target = AI_TargetZone();
        if (target != null)
        {
            GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
        }

        //set animation
        if (Vector3.Distance(transform.position, target.transform.position) > 0.5f)
        {
            AI_ManageGrenade();
        }

        if (_tm.DEBUG_MODE)
        {
            AI_DEBUG_drawPath();
        }
    }

    protected GameObject AI_TargetZone()
    {
        GameObject target = null;
        NavMeshPath path = new NavMeshPath();

        ZoneBehaviour z;

        foreach (GameObject zone in _zones)
        {
            z = zone.GetComponent<ZoneBehaviour>();
            if (z.ownerTeam != team || z.dangerLevel > 0)
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
    protected GameObject AI_TargetGrenadeZone()
    {
        GameObject target = null;
        ZoneBehaviour z;

        foreach (GameObject zone in _zones)
        {
            z = zone.GetComponent<ZoneBehaviour>();
            if (z.getTeamInArea() == enemyteam() && z.dangerLevel >= 2 && Vector3.Distance(z.transform.position, transform.position) < 50.0f)
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
    protected void AI_ManageGrenade()
    {
        //if we can't launch a grenade
        if (_launchGrenadeTimer < Player.GRENADE_LAUNCH_TIME || _numGrenade < 1)
            return;

        //TODO: check grenade trajectory


        GameObject target = AI_TargetGrenadeZone();
        if (target == null)
            return;

        _launchGrenadeTimer = 0.0f;
        _numGrenade--;


        float distance = Vector3.Distance(transform.position, target.transform.position);
        Vector2 rand = Random.insideUnitCircle*distance*0.25f;

        Vector3 grenadeTarget = target.transform.position + new Vector3(rand.x, 0, rand.y);


        Vector3 vel = grenadeTarget - transform.position;
        vel /= 20.0f;
        vel *= 0.75f;//anticipate roll
        vel.y = 0.15f;
        GameObject grenade = Object.Instantiate(_grenadePrefab, transform.position + 0.5f * vel, Quaternion.identity) as GameObject;
        grenade.GetComponent<Rigidbody>().velocity = 20.0f * vel;

    }


    /*trace navmesh path*/
    protected void AI_DEBUG_drawPath()
    {
        LineRenderer _debugPathLine = _debugChild.GetComponent<LineRenderer>(); 

        _debugPathLine.SetPosition(0, transform.position);

        NavMeshPath path = GetComponent<NavMeshAgent>().path;
        _debugPathLine.SetVertexCount(path.corners.Length);
        for (int i = 1; i < path.corners.Length; i++)
        {
            _debugPathLine.SetPosition(i, path.corners[i]);
        }

    }



    /* AI */
    /* =================================================================================================== */
    
    


   public bool isMainPlayer()
    {
        return (_mainCamera != null);
    }

    public abstract Type playerType 
	{
		get;
	}

	public Camera mainCamera 
	{
		get 
		{
			return _mainCamera;
		}
		set 
		{
			_mainCamera = value;
		}
	}

    public int team
	{
		get
		{
			return _team;
		}
		set
		{
			_team = value;
		}
	}

	public float health
	{
		get
		{
			return _health;
		}
	}


    public int enemyteam()
    {
        if (_team == 1)
            return 2;
        if (team == 2)
            return 1;
        else return 0;
    }
    
    public virtual bool canRun
    {
        get
        {
            return true;
        }
    }
}
