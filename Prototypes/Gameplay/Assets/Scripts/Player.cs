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

	int _team = 1;


    // Use this for initialization
    protected virtual void Start()
    {
		// Minimap icon
		GameObject minimapIcon = GameObject.CreatePrimitive(PrimitiveType.Quad);
		minimapIcon.GetComponent<MeshCollider>().enabled = false;
		minimapIcon.transform.SetParent(transform);
		minimapIcon.transform.localPosition = new Vector3(0.0f, 1.5f, 0.0f);
		minimapIcon.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
		minimapIcon.transform.localScale = 9.0f * Vector3.one;
		minimapIcon.layer = 10;

		if (team == 1)
		{
			minimapIcon.GetComponent<Renderer>().material = Resources.Load("team1Minimap") as Material;
		}
		else
		{
			minimapIcon.GetComponent<Renderer>().material = Resources.Load("team2Minimap") as Material;
		}

    }


    // Update is called once per frame
    protected virtual void Update () {
		// user can control the main player
		GetComponent<ThirdPersonController>().enabled = (_mainCamera != null);

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
	
}
