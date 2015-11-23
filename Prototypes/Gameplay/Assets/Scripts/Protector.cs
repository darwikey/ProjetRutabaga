using UnityEngine;
using System.Collections;

public class Protector : Player
{

	public override Type playerType
	{
		get 
		{
			return Type.PROTECTOR;
		}
	}
}
