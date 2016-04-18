using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardDuty : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GuardWaypoint[] GetWaypoints()
	{
		return gameObject.GetComponentsInChildren<GuardWaypoint> ();
	}
}
