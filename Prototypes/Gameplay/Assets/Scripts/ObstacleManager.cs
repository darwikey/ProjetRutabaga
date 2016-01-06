using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class ObstacleManager : MonoBehaviour {

    List<Obstacle> _obstacles = new List<Obstacle>();

	
    public Obstacle nearestObstacle(Vector3 point, out float minDistance)
    {
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

    public List<Obstacle> obstacles
    {
        get
        {
            return _obstacles;
        }
    }
}
