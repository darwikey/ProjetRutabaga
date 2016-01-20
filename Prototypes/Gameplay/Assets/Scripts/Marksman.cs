using UnityEngine;
using System.Collections;

public class Marksman : Player
{

	LineRenderer _bulletLine; 
	float _shootTimer = 0.0f;
	float _bulletLineTimer = 0.0f;

    public const float _shootRate = 0.5f;
    public const float _bulletTime = 0.2f;

    public override void Start()
    {
        base.Start();
        if (_bulletLine == null)
        {
            _bulletLine = gameObject.AddComponent<LineRenderer>();
            _bulletLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _bulletLine.material = Resources.Load("bulletLineMat") as Material;
        }
    }

    protected override void Update()
	{	
		base.Update ();

        
       

        if (_shootTimer > _shootRate)
		{
			// the main player shoots
			if (isMainPlayer())
            {
				// the user pressed the left button mouse down
				if (Input.GetMouseButton (0))
                {
					// reset timer
					_shootTimer = 0.0f;

					mainPlayerShoot ();
				}
				// the user pressed the right button mouse 
				else if (Input.GetMouseButton(1))
				{
					// look for the closest obstacle
					float obsDist;
					Obstacle obstacle = _obstacleManager.nearestObstacle(transform.position, out obsDist);
					if (obstacle != null && obsDist < 6.0f)
					{
						obstacle.Built();
					}
				}
			}
		}

		// bullet line
		if (_bulletLine != null) 
		{
            if (_bulletLineTimer > _bulletTime) 
			{
				// disable the bullet line after a delay
				if (_bulletLine != null) 
				{
					_bulletLine.enabled = false;
				}
			}
			else
			{
				// add transparency to the bullet line
				Color color = _bulletLine.material.color;
				color.a = 1.0f - 5.0f * _bulletLineTimer;
				_bulletLine.material.color = color;
			}
		}

		// increment timers
		_shootTimer += Time.deltaTime;
		_bulletLineTimer += Time.deltaTime;
	}


	void mainPlayerShoot()
	{
        // Raycast for the mouse
        Vector3 cursorPos = getCursorWorldPosition();

		_bulletLine.enabled = true;
		_bulletLine.SetVertexCount (2);
		_bulletLine.SetPosition(0, transform.position);
		_bulletLine.SetWidth (0.1f, 0.1f);


		Vector3 direction = cursorPos - transform.position;
		direction.Normalize();
		direction.y = 0.0f;

		// Raycast for bullet
		RaycastHit bulletHit;
		if (Physics.Raycast (transform.position + direction*0.1f, direction, out bulletHit, 100)) 
		{
			// if the bullet hits a player of the other team
			Player hitPlayer = bulletHit.collider.GetComponent<Player>();
			if (hitPlayer != null && hitPlayer.team != _team)
			{
				hitPlayer.SetDamage(60.0f);
			}
			Obstacle hitObstacle = bulletHit.collider.GetComponent<Obstacle>();
			if (hitObstacle != null)
			{
				hitObstacle.SetDamage(1);
			}

			// end of the bullet line
			_bulletLine.SetPosition (1, bulletHit.point);
		}
		else
		{
			// end of the bullet line
			_bulletLine.SetPosition(1, transform.position + 100.0f * direction);
		}

		// reset bullet line timer
		_bulletLineTimer = 0.0f;
	}

    /* ================================================================= */
    /* AI */

    protected override void AI_Update()
    {
        base.AI_Update();
        AI_ManageShot();
    }

    protected void AI_ManageShot()
    {
        if (_shootTimer > _shootRate)
        {

            _bulletLine.SetVertexCount(2);
            _bulletLine.SetPosition(0, transform.position);
            _bulletLine.SetWidth(0.1f, 0.1f);

            Player target = null;
            Vector3 targetPoint = Vector3.zero;

            foreach (Player foe in _tm.getPlayerList(enemyteam()))
            {
                Vector3 targetpos = foe.transform.position;
                Vector3 direction = targetpos - transform.position;
                direction.Normalize();
                direction.y = 0.0f;

                // find a target
                RaycastHit bulletHit;
                if (Physics.Raycast(transform.position + direction*0.1f, direction, out bulletHit, 100))
                {
                    // if the bullet hits a player of the other team
                    Player hitPlayer = bulletHit.collider.GetComponent<Player>();
                    if (hitPlayer != null && hitPlayer.team != _team)
                    {

                        if (Vector3.Distance(hitPlayer.transform.position, transform.position) < _tm._visibilityDistance)
                        {
                            if (target == null || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(hitPlayer.transform.position, transform.position))
                            {

                                target = hitPlayer;
                                targetPoint = bulletHit.point;
                            }
                        }
                    }
                }
            }

            //if target found, shot
            if (target != null)
            {
                float distance = Vector3.Distance( targetPoint ,transform.position);


                // add a random component to avoid 100% accuracy
                Vector2 rand = Random.insideUnitCircle;
                Vector3 shotPoint = targetPoint + new Vector3(rand.x, 0, rand.y)*distance;

                Vector3 direction = shotPoint - transform.position;
                direction.Normalize();
                direction.y = 0;

                RaycastHit bulletHit;
                if (Physics.Raycast(transform.position + direction*0.1f, direction, out bulletHit, 100))
                {
                    // if the bullet hits a player of the other team
                    Player hitPlayer = bulletHit.collider.GetComponent<Player>();
                    if (hitPlayer != null && hitPlayer.team != _team)
                    {
                        hitPlayer.SetDamage(60.0f);
                    }
                    Obstacle hitObstacle = bulletHit.collider.GetComponent<Obstacle>();
                    if (hitObstacle != null)
                    {
                        hitObstacle.SetDamage(1);
                    }

                    // end of the bullet line
                    _bulletLine.SetPosition(1, bulletHit.point);
                }
                else
                {
                    // end of the bullet line
                    _bulletLine.SetPosition(1, transform.position + 100.0f * direction);
                }

                _bulletLineTimer = 0.0f;
                _bulletLine.enabled = true;
                _shootTimer = 0.0f;
            }
        }
    }


    /* AI */
    /* ================================================================= */




    public override Type playerType
	{
		get 
		{
			return Type.MARKSMAN;
		}
	}
}
