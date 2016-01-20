using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class ObstacleManager : MonoBehaviour {

    Obstacle[] _obstacles = null;
	bool _isInit = false;
	
    public Obstacle nearestObstacle(Vector3 point, out float minDistance)
    {
		if (!_isInit) {
			_isInit = true;
			_obstacles = FindObjectsOfType<Obstacle> ();
		}

        Obstacle obstacle = null;
        minDistance = float.PositiveInfinity;

        foreach (Obstacle o in _obstacles)
        {
            float d = Vector3.Distance(o.transform.position, point);
            if (d < minDistance)
            {
                minDistance = d;
                obstacle = o;
            }
        }

        return obstacle;
    }

    public Obstacle[] obstacles
    {
        get
        {
            return _obstacles;
        }
    }
}
