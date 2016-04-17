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
	private Tile[] currentPath;
	private int currentGoalInPath;

	// Put data for LOOK here.
	private Direction currentDirection;
	private int lookTimer;

	// Use this for initialization
	void Start () {
		// A Guard must have GuardDuty as a parent.
		waypoints = gameObject.GetComponentInParent<GuardDuty> ().GetWaypoints ();

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

		// Look up the next waypoint.  Checks length to avoid an infinite loop.
		if (waypoints.Length > 0) {
			currentWaypoint = GetNextWaypoint (Int32.MinValue);
		} else {
			InitializeLook ();
		}
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

	/* PATHFINDING CODE BELOW HERE. */

	List<Tile> FindPath(bool includePlayer) {
		TileItem tileItem = gameObject.GetComponent<TileItem> ();

		// Establish current and goal tile.
		Tile startTile = new Tile (tileItem.tileX, tileItem.tileY);
		Tile goalTile = currentWaypoint.getTile();

		Dictionary<Tile, Tile> predecessors = new Dictionary<Tile, Tile> ();
		Dictionary<Tile, float> costs = new Dictionary<Tile, float> ();
		SortedList<Tile, float> priorityQueue = new SortedList<Tile, float> ();

		predecessors.Add (startTile, null);
		costs.Add (startTile, 0);
		priorityQueue.Add (startTile, 0);

		while (true) {
			// Pop lowest-cost node from priority queue.
			Tile currentTile = priorityQueue.Keys[0];
			priorityQueue.RemoveAt (0);

			List<Tile> neighbors = GetNeighbors (currentTile, includePlayer);
			foreach (Tile neighbor in neighbors) {
				float cost;
			}
			break;
		}

		return new List<Tile> ();
	}

	List<Tile> GetNeighbors(Tile fromMe, bool includePlayer) {
		List<Tile> neighbors = new List<Tile> (4);

		Tile right = new Tile (fromMe.X + 1, fromMe.Y);
		if (IsViable(right, includePlayer)) { neighbors.Add(right); }

		Tile up = new Tile (fromMe.X, fromMe.Y + 1);
		if (IsViable(up, includePlayer)) { neighbors.Add(up); }

		Tile left = new Tile (fromMe.X - 1, fromMe.Y);
		if (IsViable(left, includePlayer)) { neighbors.Add(left); }

		Tile down = new Tile (fromMe.X, fromMe.Y - 1);
		if (IsViable(down, includePlayer)) { neighbors.Add(down); }
		return neighbors;
	}

	bool IsViable(Tile tile, bool includePlayer) {
		return false;
	}

	float GetDistance(Tile tile1, Tile tile2) {
		float xDist = tile1.X - tile2.X;
		float yDist = tile1.Y - tile2.Y;
		return (float) Math.Sqrt((xDist * xDist) + (yDist * yDist));
	}
}
