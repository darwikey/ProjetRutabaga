using UnityEngine;
using System.Collections;

public class Marksman : Player
{

	LineRenderer _bulletLine; 
	float _shootTimer = 0.0f;
	float _bulletLineTimer = 0.0f;

	protected override void Update()
	{	
		base.Update ();
		
		if (_shootTimer > 0.5f)
		{

			// the main player shoots
			if (_mainCamera != null) {
				// the user pressed the mouse down
				if (Input.GetMouseButton (0)) {
					// reset timer
					_shootTimer = 0.0f;

					mainPlayerShoot ();
				}
			}
			// IA shoots
			else {
			
				// TODO reset timer


			}
		}

		// bullet line
		if (_bulletLine != null) 
		{
			if (_bulletLineTimer > 0.2f) 
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
		RaycastHit mouseHit;
		if (!Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out mouseHit, 100))
			return;

		if (_bulletLine == null) 
		{
			_bulletLine = gameObject.AddComponent<LineRenderer>();
			_bulletLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			_bulletLine.material = Resources.Load("bulletLineMat") as Material;
		}

		_bulletLine.enabled = true;
		_bulletLine.SetVertexCount (2);
		_bulletLine.SetPosition(0, transform.position);
		_bulletLine.SetWidth (0.1f, 0.1f);


		Vector3 direction = mouseHit.point - transform.position;
		direction.y = 0.0f;

		// Raycast for bullet
		RaycastHit bulletHit;
		if (Physics.Raycast (transform.position + direction, direction, out bulletHit, 100)) 
		{
			//if ()

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


	public override Type playerType
	{
		get 
		{
			return Type.MARKSMAN;
		}
	}
}
