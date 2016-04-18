using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
public class MoveState : State {

	public float Speed;
    public GuardDuty PatrolRoute;

	private GuardWaypoint[] waypoints;
	private GuardWaypoint currentWaypoint;

	// Whether the previous move was interrupted by the player.
	private bool moveInterruptedByPlayer = false;

	// Use this for initialization
	void Awake () {
		// A Guard must have GuardDuty as a parent.
		waypoints = PatrolRoute.GetWaypoints ();
	}

    void OnEnable ()
    {
        if (moveInterruptedByPlayer)
        {
            StartMove(true);
            moveInterruptedByPlayer = false;
            return;
        }

        // Look up the next waypoint.  Checks length to avoid an infinite loop.
        if (waypoints.Length > 0)
        {
            if (currentWaypoint == null)
            {
                currentWaypoint = GetNextWaypoint(Int32.MinValue);
            }
            else
            {
                currentWaypoint = GetNextWaypoint(currentWaypoint.Ordering);
            }
        }
        else
        {
			Debug.Log ("There are no waypoints. :(\n");
            GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
            return;
        }

        StartMove(false);
    }

	// Update is called once per frame
	void Update () {
		// No-op.
    }

	// Gets the guard actually moving.
	void StartMove(bool includePlayer) {
        TileItem tileItem = gameObject.GetComponent<TileItem>();

        // Establish current and goal tile.
        Tile currentTile = new Tile(tileItem.tileX, tileItem.tileY);
		List<Tile> path = Pathfinding.FindPath (currentTile, currentWaypoint.getTile(), includePlayer);

		// Move on if no path can be found.
		if (path == null) {
			Debug.Log ("No path can be found; switching to LookState.\n");
            GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
		}

		GuardController.Handler handleInterrupted = HandleInterrupted;
		GuardController.Handler handleCompletion = HandleCompletion;
		GetComponent<GuardController> ().Move (path, Speed, handleCompletion, handleInterrupted);
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

	public void HandleInterrupted() {
		GetComponent<GuardController> ().Stop ();

		// Check for corner case where player is blocking the waypoint.
		if (!(Pathfinding.GetPlayerTile().Equals(currentWaypoint.getTile())))
		{
			moveInterruptedByPlayer = true;
		}
		// Transition to look state.
		GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
	}

	public void HandleCompletion() {
		GetComponent<GuardController> ().Stop ();

		GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
	}
}
