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

	protected float _health = 0.0f;

	GameObject _minimapIcon;


    // When the player spawn
    public virtual void Start()
    {
		_health = 100.0f;

		// Minimap icon
		if (_minimapIcon == null) {
			_minimapIcon = GameObject.CreatePrimitive (PrimitiveType.Quad);
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

    }


    // Update is called once per frame
    protected virtual void Update () {
		// user can control the main player
		GetComponent<ThirdPersonController>().enabled = (_mainCamera != null);

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
