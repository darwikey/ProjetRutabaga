using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class TeamManager : MonoBehaviour {
	
    public uint _numPlayers = 5;
    public Transform _spawner1;
    public Transform _spawner2;
    public float _spawnRadius = 5.0f;
	public float _visibilityDistance = 25.0f;

	List<Player> _team1 = new List<Player>();
	List<Player> _team2 = new List<Player>();
    List<Player> _players = new List<Player>();

    GameObject _mainPlayer;


    // Use this for initialization
    void Start () {
	    for (uint i = 0; i < _numPlayers; i++)
        {
			CreatePlayer(1, Player.Type.MARKSMAN);
            
			if (i < 2)
			{
				CreatePlayer(2, Player.Type.PROTECTOR);
			}
			else
			{
				CreatePlayer(2, Player.Type.MARKSMAN);
			}
        }
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Player player in _team1) 
		{
			// if dead
			if (player.health <= 0.0f)
			{
				SpawnPlayer(player);
				player.Start();
			}
		}
		foreach (Player player in _team2) 
		{
			// if dead
			if (player.health <= 0.0f)
			{
				SpawnPlayer(player);
				player.Start();
			}

			// enemies too far are not rendered
			bool isVisible = false;
			foreach (Player player1 in _team1) 
			{
				if (Vector3.Distance(player.transform.position, player1.transform.position) < _visibilityDistance)
				{
					isVisible = true;
					break;
				}
			}
			player.transform.FindChild("Bip001").gameObject.SetActive(isVisible);
			player.transform.FindChild("Icon").gameObject.SetActive(isVisible);
		}
    }


	void CreatePlayer(int team, Player.Type playerType)
    {
        GameObject player = Instantiate(Resources.Load("PlayerPrefab")) as GameObject;

		Player playerInstance = null;
		// type of player
		switch (playerType) 
		{
		case Player.Type.MARKSMAN:
			playerInstance = player.AddComponent<Marksman>();
			break;

		case Player.Type.PROTECTOR:
			playerInstance = player.AddComponent<Protector>();
			break;

		case Player.Type.ENGINEER:
			playerInstance = player.AddComponent<Engineer>();
			break;
		}

		// assign a team
		playerInstance.team = team;


		// team dependant
		if (team == 1)
		{
			if (_team1.Count() == 0)
			{
				_mainPlayer = player;
			}
			_team1.Add(playerInstance);
		}
		else
		{
			_team2.Add(playerInstance);
		}
        _players.Add(playerInstance);

		// find a position
		SpawnPlayer(playerInstance);
    }


	void SpawnPlayer(Player player)
	{
		Vector2 spawnPos = _spawnRadius * Random.insideUnitCircle;

		// team dependant
		if (player.team == 1)
		{
			player.transform.position = _spawner1.position + new Vector3(spawnPos.x, 0.0f, spawnPos.y);
		}
		else
		{
			player.transform.position = _spawner2.position + new Vector3(spawnPos.x, 0.0f, spawnPos.y);
		}

	}


	public GameObject mainPlayer
	{
		get
		{
			return _mainPlayer;
		}
	}

    public List<Player> playerList
    {
        get
        {
            return _players;
        }
    }
}
