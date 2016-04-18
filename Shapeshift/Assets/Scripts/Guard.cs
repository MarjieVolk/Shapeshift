using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
public class Guard : MonoBehaviour {

	public float Speed;
	public float ChaseSpeed;
	public int LookTime;
	public Direction FirstDirection = Direction.SOUTH;

	private GuardWaypoint[] waypoints;
	private GuardWaypoint currentWaypoint;
	private GuardAction currentAction;
	private Direction currentDirection;

	enum GuardAction {
		MOVE,
		LOOK,
		CHASE
	}

	// Put data for MOVE here.
	private bool moveInterruptedByPlayer = false;
	private List<Tile> currentPath;
	private int currentGoalInPath;  // Should always be greater than 1.

	// Put data for LOOK here.
	private int lookTimer;

	// Use this for initialization
	void Start () {
		// Initialize the direction first thing.
		currentDirection = FirstDirection;

		// A Guard must have GuardDuty as a parent.
		waypoints = gameObject.GetComponentInParent<GuardDuty> ().GetWaypoints ();

		// Guard looks around before anything else.
		InitializeLook();
	}

	// Update is called once per frame
	void Update () {
		if (currentAction == GuardAction.LOOK) {
			UpdateLook ();
		} else if (currentAction == GuardAction.MOVE) {
			UpdateMove ();
		}
	}

	// MOVE initialization.
	void InitializeMove() {
		if (moveInterruptedByPlayer) {
			StartMove (true);
			moveInterruptedByPlayer = false;
			return;
		}

		// Look up the next waypoint.  Checks length to avoid an infinite loop.
		if (waypoints.Length > 0) {
			if (currentWaypoint == null) {
				currentWaypoint = GetNextWaypoint (Int32.MinValue);
			} else {
				currentWaypoint = GetNextWaypoint (currentWaypoint.Ordering);
			}
		} else {
			InitializeLook ();
			return;
		}

		StartMove (false);
	}

	// Gets the guard actually moving.
	void StartMove(bool includePlayer) {
		currentAction = GuardAction.MOVE;

		currentPath = FindPath (includePlayer);
		currentGoalInPath = 1;

		// Precautionary snap to grid.
		gameObject.GetComponent<TileItem> ().SnapToGrid ();

		// Move on if no path can be found.
		if (currentPath == null) {
			InitializeLook ();
			return;
		}
	}

	// Update in MOVE mode.
	void UpdateMove() {
		Tile goalTile = currentPath [currentGoalInPath];
		float goalX = TileItem.TileToGlobalPosition (goalTile.X);
		float goalY = TileItem.TileToGlobalPosition (goalTile.Y);
		Vector3 goalPos = new Vector3 (goalX, goalY);

		// The current goal has been reached.  Move on to the next tile.
		if (ManhattanDistance(goalPos, transform.position) <= 2 * Speed) {
			currentGoalInPath++;

			// Course correct.
			gameObject.GetComponent<TileItem> ().SnapToGrid ();

			// If you have reached the final goal, start looking around.
			if (currentGoalInPath == currentPath.Count) {
				InitializeLook ();
			} else {
				// Change directions if necessary.
				currentDirection = GetDirectionFromTiles(goalTile, currentPath[currentGoalInPath]);
			}
			return;
		}

		// Proceed to goal.
		Tile oldTile = currentPath[currentGoalInPath - 1];
		float oldX = TileItem.TileToGlobalPosition (oldTile.X);
		float oldY = TileItem.TileToGlobalPosition (oldTile.Y);
		Vector3 oldPos = new Vector3 (oldX, oldY);

		// If the player is blocking the way, recalculate a route that goes around the player.
		if (GetPlayerTile().Equals(goalTile)) {
			// Corner case where player is blocking the waypoint.
			if (!(goalTile.Equals(currentPath[currentPath.Count - 1]))) {
				moveInterruptedByPlayer = true;
			}
			InitializeLook ();
			return;
		}

		if (ManhattanDistance (transform.position, goalPos) > 1.1 * ManhattanDistance (oldPos, goalPos)) {
			StartMove (false);
		}

		Vector3 increment = (goalPos - oldPos);
		increment.Normalize ();
		increment *= Speed;
		GetComponent<TileItem>().SetGlobalPosition(transform.position + increment);
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

	public Direction GetCurrentDirection() {
		return currentDirection;
	}

	float ManhattanDistance(Vector3 a, Vector3 b) {
		return Math.Abs (a.x - b.x) + Math.Abs (a.y- b.y);
	}

	/* PATHFINDING CODE BELOW HERE. */

	List<Tile> FindPath(bool includePlayer) {
		TileItem tileItem = gameObject.GetComponent<TileItem> ();

		// Establish current and goal tile.
		Tile startTile = new Tile (tileItem.tileX, tileItem.tileY);
		Tile goalTile = currentWaypoint.getTile();

		Dictionary<Tile, Tile> predecessors = new Dictionary<Tile, Tile> ();
		Dictionary<Tile, float> costs = new Dictionary<Tile, float> ();
		SortedList<ScoredTile, float> priorityQueue = new SortedList<ScoredTile, float> ();

		predecessors.Add (startTile, null);
		costs[startTile] = 0;
		priorityQueue.Add(new ScoredTile(startTile, 0), 0);

		int numIter = 0;;
		while (true) {
			// Pop lowest-cost node from priority queue.
			Tile currentTile = priorityQueue.Keys[0].EnclosedTile;
			priorityQueue.RemoveAt (0);

			// Add neighbors if a new lowest-cost path can be made through them.
			foreach (Tile neighbor in GetNeighbors (currentTile, goalTile, includePlayer)) {
				float newCost = costs [currentTile] + 1 + GetDistance (currentTile, neighbor);
				if (!(costs.ContainsKey (neighbor) && costs [neighbor] <= newCost)) {
					// Remove old cost in priorityQueue if necessary.
					if (costs.ContainsKey(neighbor)) {
						ScoredTile oldNeighbor = new ScoredTile (neighbor, costs [neighbor]);
						if (priorityQueue.ContainsKey(oldNeighbor)) {
							priorityQueue.Remove (oldNeighbor);
						}
					}
					costs [neighbor] = newCost;
					predecessors [neighbor] = currentTile;
					priorityQueue.Add(new ScoredTile(neighbor, newCost), newCost);
				}
			}

			// Ending condition.
			if (costs.ContainsKey (goalTile)) {
				float goalCost = costs [goalTile];
				foreach (float cost in priorityQueue.Values) {
					if (cost < goalCost) {
						continue;
					}
				}
				break;
			}

			numIter++;
			if (priorityQueue.Count == 0 || numIter > 1000) {
				// There is no path. :(
				return null;
			}
		}
		// Adds everything, including the start tile, to the path.
		List<Tile> tracedPath = new List<Tile> ();
		Tile traceTile = goalTile;
		while (traceTile != null) {
			tracedPath.Add (traceTile);
			traceTile = predecessors [traceTile];
		}
		tracedPath.Reverse ();
		return tracedPath;
	}

	List<Tile> GetNeighbors(Tile fromMe, Tile goalTile, bool includePlayer) {
		List<Tile> neighbors = new List<Tile> (4);

		Tile right = new Tile (fromMe.X + 1, fromMe.Y);
		if (IsViable(right, goalTile, includePlayer)) { neighbors.Add(right); }

		Tile up = new Tile (fromMe.X, fromMe.Y + 1);
		if (IsViable(up, goalTile, includePlayer)) { neighbors.Add(up); }

		Tile left = new Tile (fromMe.X - 1, fromMe.Y);
		if (IsViable(left, goalTile, includePlayer)) { neighbors.Add(left); }

		Tile down = new Tile (fromMe.X, fromMe.Y - 1);
		if (IsViable(down, goalTile, includePlayer)) { neighbors.Add(down); }
		return neighbors;
	}

	bool IsViable(Tile tile, Tile goalTile, bool includePlayer) {
		if (tile.Equals (goalTile)) {
			return true;
		} else if (TileItem.GetObjectsAtPosition<FurnitureItem> (tile.X, tile.Y).Count > 0) {
			return false;
		} else if (TileItem.GetObjectsAtPosition<Wall> (tile.X, tile.Y).Count > 0) {
			return false;
		} else if (includePlayer && GetPlayerTile().Equals(tile)) {
			return false;
		}
		return true;
	}

	Tile GetPlayerTile() {
		GameObject player;
		foreach (GameObject go in gameObject.scene.GetRootGameObjects()) {
			if (go.GetComponents<PlayerController> ().Length > 0) {
				player = go;
				return new Tile(
					TileItem.GlobalToTilePosition (player.transform.position.x), 
					TileItem.GlobalToTilePosition(player.transform.position.y));
			}
		}
		return null;
	}

	float GetDistance(Tile tile1, Tile tile2) {
		float xDist = tile1.X - tile2.X;
		float yDist = tile1.Y - tile2.Y;
		return (float) Math.Sqrt((xDist * xDist) + (yDist * yDist));
	}
}
