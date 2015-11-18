using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ZoneBehaviour : MonoBehaviour {

    public bool isPlayerInArea = false;
    public MeshRenderer rend;
    GameObject[] players;

    List<GameObject> playersInArea = new List<GameObject>();

	// Use this for initialization
	void Start () {
        rend = GetComponent<MeshRenderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
    }
	
	// Update is called once per frame
	void FixedUpdate () {

  
        int teamInArea = getTeamInArea();
        setAreaColor(teamInArea);

    }

    /*quand un objet entre dans la zone*/
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            if (!playersInArea.Contains(other.gameObject))
            {
                playersInArea.Add(other.gameObject);
            }
        }
      
    }
    /*quand un objet sort de la zone*/
    void OnTriggerExit(Collider other)
    {
   
        if (other.gameObject.tag == "Player")
        {
            playersInArea.Remove(other.gameObject);
            isPlayerInArea = false;
        }
    }




    int getTeamInArea()
    {
        int teamId;
        int teamInArea = 0;
        foreach (GameObject player in playersInArea)
        {
            teamId = player.GetComponent<playerInfo>().teamId;
            if (teamInArea == 0)
            {
                teamInArea = teamId;
            }
            else if (teamInArea != teamId)
            {
                teamInArea = -1;
            }
        }
        return teamInArea;
    }

    void setAreaColor(int teamInArea)
    {
        switch (teamInArea)
        {
            case -1:
                rend.material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                break;
            case 0:
                rend.material.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                break;
            case 1:
                rend.material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                break;
            case 2:
                rend.material.color = new Color(0.0f, 0.0f, 1.0f, 0.5f);
                break;
        }
    }




}
