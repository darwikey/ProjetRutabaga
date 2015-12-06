using UnityEngine;
using System.Collections;

public class Protector : Player
{
	public override void Start()
	{
		base.Start ();
		_health *= 1.5f;
	}


	public override Type playerType
	{
		get 
		{
			return Type.PROTECTOR;
		}
	}
}
