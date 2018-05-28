using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour {

	Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();	
	}
	
	// Update is called once per frame
	public void SetText (string txt) {
		text.text = txt;	
	}
}
