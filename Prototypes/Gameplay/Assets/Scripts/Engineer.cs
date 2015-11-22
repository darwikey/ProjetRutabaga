using UnityEngine;
using System.Collections;

public class Engineer : Player
{

	public override Type playerType
	{
		get 
		{
			return Type.ENGINEER;
		}
	}
}
