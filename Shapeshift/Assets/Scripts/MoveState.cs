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

	// Put data for MOVE here.
	private bool moveInterruptedByPlayer = false;
	private List<Tile> currentPath;
	private int currentGoalInPath;  // Should always be greater than 1.

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
        Tile goalTile = currentPath[currentGoalInPath];
        float goalX = TileItem.TileToGlobalPosition(goalTile.X);
        float goalY = TileItem.TileToGlobalPosition(goalTile.Y);
        Vector3 goalPos = new Vector3(goalX, goalY);

        // The current goal has been reached.  Move on to the next tile.
        if (ManhattanDistance(goalPos, transform.position) <= 2 * Speed)
        {
            currentGoalInPath++;

            // Course correct.
            gameObject.GetComponent<TileItem>().SnapToGrid();

            // If you have reached the final goal, start looking around.
            // TODO switch states here
            if (currentGoalInPath == currentPath.Count)
            {
                GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
            }
            else
            {
                // Change directions if necessary.
                //TODO trigger animation switch
                GetComponent<DirectionComponent>().Direction = GetDirectionFromTiles(goalTile, currentPath[currentGoalInPath]);
            }
            return;
        }

        // Proceed to goal.
        Tile oldTile = currentPath[currentGoalInPath - 1];
        float oldX = TileItem.TileToGlobalPosition(oldTile.X);
        float oldY = TileItem.TileToGlobalPosition(oldTile.Y);
        Vector3 oldPos = new Vector3(oldX, oldY);

        // If the player is blocking the way, recalculate a route that goes around the player.
        if (Pathfinding.GetPlayerTile().Equals(goalTile))
        {
            // Corner case where player is blocking the waypoint.
            if (!(goalTile.Equals(currentPath[currentPath.Count - 1])))
            {
                moveInterruptedByPlayer = true;
            }
            // transition to look state
            GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
            return;
        }

        if (ManhattanDistance(transform.position, goalPos) > 1.1 * ManhattanDistance(oldPos, goalPos))
        {
            StartMove(false);
        }

        Vector3 increment = (goalPos - oldPos);
        increment.Normalize();
        increment *= Speed;
        GetComponent<TileItem>().SetGlobalPosition(transform.position + increment);
    }

	// Gets the guard actually moving.
	void StartMove(bool includePlayer) {
        TileItem tileItem = gameObject.GetComponent<TileItem>();

        // Establish current and goal tile.
        Tile startTile = new Tile(tileItem.tileX, tileItem.tileY);
        Tile goalTile = currentWaypoint.getTile();
        currentPath = Pathfinding.FindPath (startTile, goalTile, includePlayer);
		currentGoalInPath = 0;

		// Precautionary snap to grid.
		gameObject.GetComponent<TileItem> ().SnapToGrid ();

		// Move on if no path can be found.
		if (currentPath == null) {
			Debug.Log ("No path can be found; switching to LookState.\n");
            GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
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

	Direction GetDirectionFromTiles(Tile fromMe, Tile toMe) {
		int xDiff = toMe.X - fromMe.X;
		int yDiff = toMe.Y - fromMe.Y;
		if (xDiff == 1) {
			return Direction.EAST;
		} else if (xDiff == -1) {
			return Direction.WEST;
		} else if (yDiff == 1) {
			return Direction.NORTH;
		} else {
			return Direction.SOUTH;
		}
	}

	float ManhattanDistance(Vector3 a, Vector3 b) {
		return Math.Abs (a.x - b.x) + Math.Abs (a.y- b.y);
	}
}
