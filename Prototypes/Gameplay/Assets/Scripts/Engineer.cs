using UnityEngine;
using System.Collections;

public class Engineer : Player
{

    public override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // the main player
        if (_mainCamera != null)
        {
            // the user pressed the right mouse button
            if (Input.GetMouseButton(1))
            {
                // look for the closest obstacle
                float obsDist;
                Obstacle obstacle = _obstacleManager.nearestObstacle(transform.position, out obsDist);
                if (obstacle != null && obsDist < 6.0f)
                {
                    obstacle.built();
                }
            }
        }
        // IA
        else
        {
        }
    }

    public override Type playerType
	{
		get
		{
			return Type.ENGINEER;
		}
	}
}
