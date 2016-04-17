using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
public class Guard : MonoBehaviour {

	private GuardWaypoint[] waypoints;
	private GuardWaypoint currentWaypoint;

	// Use this for initialization
	void Start () {
		waypoints = gameObject.GetComponentInParent<GuardDuty> ().GetWaypoints ();
		if (waypoints.Length > 0) {
			currentWaypoint = GetNextWaypoint (Int32.MinValue);
		}
		//gameObject.GetComponent<SpriteRenderer> ();
		//gameObject.transform.
		//gameObject.scene.GetRootGameObjects
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<TileItem>().SetGlobalPosition(transform.position + new Vector3(1, 1));
	}

	GuardWaypoint GetNextWaypoint(int currentOrdering) {
		int nextOrdering = Int32.MaxValue;
		GuardWaypoint nextWaypoint = null;

		foreach (GuardWaypoint waypoint in waypoints) {
			if (waypoint.Ordering > currentOrdering && waypoint.Ordering < nextOrdering) {
				nextOrdering = waypoint.Ordering;
				nextWaypoint = waypoint;
			}
		}

		if (nextWaypoint != null) {
			return nextWaypoint;
		} else {
			return GetNextWaypoint (Int32.MinValue);
		}
	}
}
