﻿using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	public Juggler owner;
	public Vector3 destination;
	private bool dropped;
	// Use this for initialization
	void Start () {
		destination = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision) { 
		//Debug.Log("Touched ground");
		dropped = true;
	}

	void OnCollisionLeave(Collision collision) { 
		Debug.Log("Ball picked up");
		dropped = false;
	}

	public bool IsAvailable() {
		return owner == null;
	}
}
