using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeathScreen : MonoBehaviour {

    Image _image;
    Text _text;
    float _timer;

	// Use this for initialization
	void Start () {
        _image = GetComponent<Image>();
        _text = transform.GetChild(0).GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {

        if (_image.enabled)
        {
            _timer += Time.deltaTime; 
            if (_timer > 2.0f)
            {
                _image.enabled = false;
                _text.enabled = false;
            }
        }
	}

    public void Show()
    {
        _image.enabled = true;
        _text.enabled = true;
        _timer = 0.0f;
    }
}
