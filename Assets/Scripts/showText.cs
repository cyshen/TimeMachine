using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showText : MonoBehaviour {

    public string objectText; 

	// Use this for initialization
	void Start () {
        this.GetComponent<TextMesh>().text = objectText;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
