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

	protected Camera _mainCamera;

    // Use this for initialization
    protected virtual void Start()
    {
    }


    // Update is called once per frame
    protected virtual void Update () {

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

}
