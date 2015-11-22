using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class TeamManager : MonoBehaviour {
	
    public uint _numPlayers = 5;
    public Transform _spawner1;
    public Transform _spawner2;
    public float _spawnRadius = 5.0f;

	List<GameObject> _team1 = new List<GameObject>();
	List<GameObject> _team2 = new List<GameObject>();

	GameObject _mainPlayer;


    // Use this for initialization
    void Start () {
	    for (uint i = 0; i < _numPlayers; i++)
        {
            SpawnPlayer(1, Player.Type.MARKSMAN);
            
			SpawnPlayer(2, Player.Type.MARKSMAN);
        }
	}
	
	// Update is called once per frame
	void Update () {
	    
    }


	void SpawnPlayer(int team, Player.Type playerType)
    {
        GameObject player = Instantiate(Resources.Load("PlayerPrefab")) as GameObject;

        // Minimap icon
        GameObject minimapIcon = GameObject.CreatePrimitive(PrimitiveType.Quad);
        minimapIcon.GetComponent<MeshCollider>().enabled = false;
        minimapIcon.transform.SetParent(player.transform);
        minimapIcon.transform.localPosition = new Vector3(0.0f, 1.5f, 0.0f);
        minimapIcon.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        minimapIcon.transform.localScale = 9.0f * Vector3.one;
        minimapIcon.layer = 10;


        Vector2 spawnPos = _spawnRadius * Random.insideUnitCircle;

		// team dependant
        if (team == 1)
        {
            if (_team1.Count() == 0)
            {
				_mainPlayer = player;
            }
            _team1.Add(player);

            player.transform.position = _spawner1.position + new Vector3(spawnPos.x, 0.0f, spawnPos.y);

            minimapIcon.GetComponent<Renderer>().material = Resources.Load("team1Minimap") as Material;
        }
        else
        {
            _team2.Add(player);

            player.transform.position = _spawner2.position + new Vector3(spawnPos.x, 0.0f, spawnPos.y);

            minimapIcon.GetComponent<Renderer>().material = Resources.Load("team2Minimap") as Material;
        }

		// type of player
		switch (playerType) 
		{
		case Player.Type.MARKSMAN:
			player.AddComponent<Marksman>();
			break;

		case Player.Type.PROTECTOR:
			player.AddComponent<Protector>();
			break;

		case Player.Type.ENGINEER:
			player.AddComponent<Engineer>();
			break;
		}
    }


	public GameObject mainPlayer
	{
		get
		{
			return _mainPlayer;
		}
	}
}
