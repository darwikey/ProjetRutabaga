using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class TeamManager : MonoBehaviour {

    List<GameObject> _team1 = new List<GameObject>();
    List<GameObject> _team2 = new List<GameObject>();

    public uint _numPlayers = 5;

    public GameObject _playerEntity;


    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    if (_team1.Count() < _numPlayers)
        {
            SpawnPlayer(1);
        }

        if (_team2.Count() < _numPlayers)
        {
            SpawnPlayer(2);
        }
    }


    void SpawnPlayer(int team)
    {
        Vector3 position = new Vector3(Random.Range(0.0f, 200.0f), 1.0f, Random.Range(0.0f, 200.0f));
        GameObject player = Instantiate(_playerEntity, position, Quaternion.identity) as GameObject;

        GameObject minimapIcon = GameObject.CreatePrimitive(PrimitiveType.Quad);
        minimapIcon.transform.SetParent(player.transform);
        minimapIcon.transform.localPosition = new Vector3(0.0f, 1.5f, 0.0f);
        minimapIcon.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        minimapIcon.transform.localScale = 9.0f * Vector3.one;
        minimapIcon.layer = 10;

        if (team == 1)
        {
            if (_team1.Count() == 0)
            {
                player.GetComponent<ThirdPersonController>().enabled = true;
            }
            _team1.Add(player);
        }
        else
        {
            _team2.Add(player);
        }
    }
}
