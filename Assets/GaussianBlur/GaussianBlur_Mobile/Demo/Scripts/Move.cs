using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour 
{

    public bool moveOnX = false;
    public bool moveOnY = false;

    public float amp = 1f;
    private Vector3 originalPos; 

	// Use this for initialization
	void Start () 
    {
        originalPos = transform.position;	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        float X = (moveOnX) ? originalPos.x + Mathf.Sin(Time.time) * amp :originalPos.x;
        float Y = (moveOnY) ? originalPos.y + Mathf.Sin(Time.time) * amp: originalPos.y;


        transform.position = new Vector3(X ,Y,originalPos.z);
	}
}
