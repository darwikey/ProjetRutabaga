﻿using UnityEngine;
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

	protected float _health = 0.0f;

	GameObject _minimapIcon;

    // grenades
    int _numGrenade = 99;
    float _launchGrenadeTimer = 0.0f;
    GameObject _grenadePrefab;


    // When the player spawn
    public virtual void Start()
    {
		_health = 100.0f;

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

        // grenade
        _grenadePrefab = Resources.Load("Grenade") as GameObject;
    }


    // Update is called once per frame
    protected virtual void Update () {
		// user can control the main player
		GetComponent<ThirdPersonController>().enabled = (_mainCamera != null);

        // Grenade
        if (_launchGrenadeTimer > 2.0f && _numGrenade > 0)
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
            else // AI todo
            {
                if (false)
                {
                    launchGrenade();
                }
            }
        }

        _launchGrenadeTimer += Time.deltaTime;
    }


    protected void launchGrenade()
    {
        _launchGrenadeTimer = 0.0f;
        _numGrenade--;
        
        Vector3 vel = getCursorWorldPosition() - transform.position;
        vel.Normalize();
        vel.y = 0.15f;
        GameObject grenade = Object.Instantiate(_grenadePrefab, transform.position + 0.5f * vel, Quaternion.identity) as GameObject;
        grenade.GetComponent<Rigidbody>().velocity = 20.0f * vel;
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
		_health -= damage;
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
}
