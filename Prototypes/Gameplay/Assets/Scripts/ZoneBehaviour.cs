using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ZoneBehaviour : MonoBehaviour {

    public MeshRenderer rend;
    public float captureDuration = 3.0f;
    public int dangerLevel;
    /*timer for the capture*/
    public float captureTimer;

    /*id of the owner team, 0: none, 1: red, 2: blue*/
    public int ownerTeam;

    /*list of player currently in the area*/
    List<GameObject> playersInArea = new List<GameObject>();


	// Use this for initialization
	void Start () {
        rend = GetComponent<MeshRenderer>();
        captureTimer = 0;
        ownerTeam = 0;
        dangerLevel = 0; //0
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        int teamInArea = getTeamInArea();
        if(teamInArea != 0 && teamInArea != ownerTeam)
        {
            if (teamInArea == -1)
                dangerLevel = 1;//teammate are defending the area
            else
                dangerLevel = 2;//no teamate defending the area
        }
        else
        {
            dangerLevel = 0;//no danger
        }

        manageTimer(teamInArea);
        setOwnerTeam(teamInArea);
        setAreaColor(ownerTeam);
    }

    /*when an object enter the area*/
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

    /*when an object leave the area*/
    void OnTriggerExit(Collider other)
    {
   
        if (other.gameObject.tag == "Player")
        {
            playersInArea.Remove(other.gameObject);
        }
    }

    /*manage timer according to the team in area*/
    void manageTimer(int teamInArea)
    {
        if(teamInArea > 0 && teamInArea != ownerTeam)
        {
            captureTimer += Time.deltaTime;
        }
        else
        {
            captureTimer = .0f;
        }
    }

    /*set the owner team when captureTimer > captureDuration*/
    void setOwnerTeam(int teamInArea)
    {
        if(captureTimer >= captureDuration)
        {
            ownerTeam = teamInArea;

        }
    }

    /* return the team present in area; 0: none; 1: red; 2: blue; -1: both*/
    public int getTeamInArea()
    {
        int teamId;
        int teamInArea = 0;

        foreach (GameObject player in playersInArea)
        {
            teamId = player.GetComponent<Player>().team;
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

    /* set the area color according to the owner team*/
    void setAreaColor(int ownerTeam)
    {
        switch (ownerTeam)
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
