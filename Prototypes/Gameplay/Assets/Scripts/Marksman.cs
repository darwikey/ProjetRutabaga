using UnityEngine;
using System.Collections;

public class Marksman : Player
{

	LineRenderer _bulletLine; 
	float _shootTimer;

	protected override void Update()
	{	
		base.Update ();
		
		// the main player shoots
		if (_mainCamera != null)
		{
			// the user pressed the mouse down
			if (Input.GetMouseButtonDown (0))
			{
				mainPlayerShoot ();
			}
		}
		// IA shoots
		else 
		{

		}

		_shootTimer += Time.deltaTime;
	}


	void mainPlayerShoot()
	{
		
		// We need to actually hit an object
		RaycastHit hit;
		if (!Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
			return;


		if (_bulletLine == null) 
		{
			_bulletLine = gameObject.AddComponent<LineRenderer>();
			_bulletLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			_bulletLine.material = Resources.Load("bulletLineMat") as Material;
		}

		_bulletLine.SetVertexCount (2);
		_bulletLine.SetPosition(0, transform.position);
		_bulletLine.SetPosition(1, hit.point);
		_bulletLine.SetWidth (0.1f, 0.1f);
	}


	public override Type playerType
	{
		get 
		{
			return Type.MARKSMAN;
		}
	}
}
