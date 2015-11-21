using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZoneUI : MonoBehaviour {

    RectTransform rt;
    static float maxWidth = 0.8f;
    static float maxHeight = 0.05f;
    float width;



	// Use this for initialization
	void Start () {
        rt = GetComponent<RectTransform>();
        width = .8f;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        ZoneBehaviour zoneBehaviour = transform.parent.parent.GetComponent<ZoneBehaviour>();
        float timer = zoneBehaviour.captureTimer;
        float maxTimer = zoneBehaviour.captureDuration;
        int teamInArea = zoneBehaviour.getTeamInArea();

        if (timer == .0f)
            width = .0f;
        else
        {
            width = timer / maxTimer;
            width = 1 - width;
            width *= maxWidth;
        }

        if (width < .0f)
            width = .0f;
        if (width > maxWidth)
            width = maxWidth;

        rt.sizeDelta = new Vector2(width, maxHeight);

        setUIColor(teamInArea);

	}

    void setUIColor(int teamInArea)
    {
         

        switch (teamInArea)
        {
            case -1:
                GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                break;
            case 0:
                GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                break;
            case 1:
                GetComponent<Image>().color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                break;
            case 2:
                GetComponent<Image>().color = new Color(0.0f, 0.0f, 1.0f, 0.5f);
                break;
        }
    }


}
