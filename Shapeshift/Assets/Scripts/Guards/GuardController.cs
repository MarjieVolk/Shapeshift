using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
public class GuardController : MonoBehaviour {

	public delegate void Handler ();

	private bool isMoving;

	private List<Tile> currentPath;
	private int currentGoalInPath;
	private float currentSpeed;
	private Handler currentFinishHandler;
	private Handler currentInterruptedHandler;

	// Use this for initialization
	void Awake () {
		isMoving = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isMoving) {
			return;
		}

		Tile goalTile = currentPath[currentGoalInPath];
		float goalX = TileItem.TileToGlobalPosition(goalTile.X);
		float goalY = TileItem.TileToGlobalPosition(goalTile.Y);
		Vector3 goalPos = new Vector3(goalX, goalY);

		// The current goal has been reached.  Move on to the next tile.
		if (ManhattanDistance(goalPos, transform.position) <= 2 * speedForFrame())
		{
			currentGoalInPath++;

			// Course correct.
			gameObject.GetComponent<TileItem>().SnapToGrid();

			// If you have reached the final goal, call the finish handler.
			// TODO switch states here
			if (currentGoalInPath == currentPath.Count)
			{
				currentFinishHandler ();
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

		Vector3 increment = (goalPos - oldPos);
		increment.Normalize();
		increment *= speedForFrame();
		GetComponent<TileItem>().SetGlobalPosition(transform.position + increment);

		// Handle case where a player is blocking the way.
		if (Pathfinding.GetPlayerTile().Equals(goalTile))
		{
			currentInterruptedHandler();
		}

		// Emergency course correction.
		if (ManhattanDistance(transform.position, goalPos) > 1.1 * ManhattanDistance(oldPos, goalPos))
		{
            currentInterruptedHandler();
        }
	}

	public void Move(List<Tile> pathToFollow, float speed, Handler finishHandler, Handler interruptedHandler) {
        if (pathToFollow.Count < 2)
        {
            finishHandler();
            return;
        }
		currentPath = pathToFollow;
		currentGoalInPath = 1;

		currentSpeed = speed;
		currentFinishHandler = finishHandler;
		currentInterruptedHandler = interruptedHandler;

		// Precautionary snap to grid.
		gameObject.GetComponent<TileItem> ().SnapToGrid ();

		isMoving = true;
	}

	public void Stop() {
		// Course correct.
		gameObject.GetComponent<TileItem>().SnapToGrid();

		isMoving = false;
	}

	public bool GetIsMoving() {
		return isMoving;
	}

    float speedForFrame()
    {
        return Time.deltaTime * currentSpeed * TileItem.TILE_SIZE;
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
