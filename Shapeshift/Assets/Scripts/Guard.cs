using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
public class Guard : MonoBehaviour {

	public float Speed = 0.01f;
	public int LookTime = 200;
	public Direction FirstDirection = Direction.SOUTH;

	private GuardWaypoint[] waypoints;
	private GuardWaypoint currentWaypoint;
	private GuardAction currentAction;

	enum GuardAction {
		MOVE,
		LOOK,
		CATCH
	}

	// Put data for MOVE here.
	private int goalTileX;
	private int goalTileY;

	// Put data for LOOK here.
	private Direction currentDirection;
	private int lookTimer;

	// Use this for initialization
	void Start () {
		// A Guard must have GuardDuty as a parent.
		waypoints = gameObject.GetComponentInParent<GuardDuty> ().GetWaypoints ();
		if (waypoints.Length > 0) {
			currentWaypoint = GetNextWaypoint (Int32.MinValue);
		}

		// Guard looks around before anything else.
		InitializeLook();
	}

	// Update is called once per frame
	void Update () {
		if (currentAction == GuardAction.LOOK) {
			UpdateLook ();
		}
		GetComponent<TileItem>().SetGlobalPosition(new Vector3(transform.position.x + Speed, transform.position.y + Speed));
	}

	// MOVE initialization.
	void InitializeMove() {
		currentAction = GuardAction.MOVE;
	}

	// LOOK initialization.
	void InitializeLook() {
		currentAction = GuardAction.LOOK;
		lookTimer = LookTime;
		currentDirection = FirstDirection;
	}

	// Update in LOOK mode.
	void UpdateLook() {
		if (lookTimer <= 0) {
			currentDirection = Clockwise (currentDirection);
			if (currentDirection == FirstDirection) {
				InitializeMove ();
			} else {
				lookTimer = LookTime;
			}
		} else {
			lookTimer -= 1;
		}
			
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

	Direction Clockwise(Direction fromMe) {
		if (fromMe == Direction.NORTH) {
			return Direction.EAST;
		} else if (fromMe == Direction.EAST) {
			return Direction.SOUTH;
		} else if (fromMe == Direction.SOUTH) {
			return Direction.WEST;
		} else {
			return Direction.NORTH;
		}
	}
}
