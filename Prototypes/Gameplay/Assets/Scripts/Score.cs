using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour {
    GameObject[] _zones;
    double _score1 = 0.0;
    double _score2 = 0.0;


    // Use this for initialization
    void Start () {
        _zones = GameObject.FindGameObjectsWithTag("Zone");
    }
	
	// Update is called once per frame
	void Update () {
	    foreach (GameObject z in _zones)
        {
            int team = z.GetComponent<ZoneBehaviour>().ownerTeam;
            if (team == 1)
            {
                _score1 += Time.deltaTime * 0.2;
            }
            else if (team == 2)
            {
                _score2 += Time.deltaTime * 0.2;
            }
        }

        GetComponent<Text>().text = "<color=#ff0000ff>" + ((int)_score1).ToString() + "</color> - <color=#0000ffff>" + ((int)_score2).ToString() + "</color>";

    }
}
