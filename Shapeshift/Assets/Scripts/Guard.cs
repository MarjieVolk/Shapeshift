using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
public class Guard : MonoBehaviour {

	private GuardWaypoint[] waypoints;
	private GuardWaypoint currentWaypoint;
	private GuardAction currentAction;

	enum GuardAction {
		MOVE,
		LOOK,
		CATCH
	}

	// Use this for initialization
	void Start () {
		// A Guard must have GuardDuty as a parent.
		waypoints = gameObject.GetComponentInParent<GuardDuty> ().GetWaypoints ();
		if (waypoints.Length > 0) {
			currentWaypoint = GetNextWaypoint (Int32.MinValue);
		}
		currentAction = GuardAction.LOOK;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y + 0.00f);
		//GetComponent<TileItem>().SetGlobalPosition(new Vector3(transform.position.x + 1, transform.position.y + 1));
	}

	// Grabs the guard's next waypoint.
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
