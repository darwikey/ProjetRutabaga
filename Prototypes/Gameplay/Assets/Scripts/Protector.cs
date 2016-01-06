using UnityEngine;
using System.Collections;

public class Protector : Player
{
	GameObject _forceField;

	public override void Start()
	{
		base.Start ();
		_health *= 1.5f;

		_forceField = Instantiate (Resources.Load ("ForceField") as GameObject, transform.position, Quaternion.identity) as GameObject;
		_forceField.transform.SetParent (transform);
	}


	public override Type playerType
	{
		get 
		{
			return Type.PROTECTOR;
		}
	}

    public override bool canRun
    {
        get
        {
            return false;
        }
    }
}
