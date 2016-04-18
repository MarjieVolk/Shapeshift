using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
public class ChaseState : State {

	public float Speed;

	private Tile currentTarget;

	void OnEnable ()
	{
		// TODO: Guard chasing animation.
		// TODO: Chase music.
		StartChase();
	}

	void Update () {
		// No-op.
	}

	// This is called whenever a player is spotted.
	public void HandlePlayerSpotted(Vector3 playerPos) {
		// TODO: If player is furniture, judge player instead of chasing.
		currentTarget = new Tile(
			TileItem.GlobalToTilePosition(playerPos.x), TileItem.GlobalToTilePosition(playerPos.y));
		GetComponent<StateMachine>().CurrentState = GetComponent<ChaseState>();
	}

	void StartChase() {
		TileItem tileItem = gameObject.GetComponent<TileItem>();

		// Establish current and goal tile.
		Tile currentTile = new Tile(tileItem.tileX, tileItem.tileY);
		List<Tile> path = Pathfinding.FindPath (currentTile, currentTarget, false);

		// Move on if no path can be found.
		if (path == null || path.Count < 2) {
			Debug.Log ("No path can be found; switching to LookState.\n");
			GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
			return;
		}

		List<Tile> partialPath = new List<Tile> ();
		partialPath.Add (path [0]);
		partialPath.Add (path [1]);

		GuardController.Handler handleInterrupted = HandleInterrupted;
		GuardController.Handler handleCompletion = HandleCompletion;
		GetComponent<GuardController> ().Move (partialPath, Speed, handleCompletion, handleInterrupted);
	}

	public void HandleInterrupted() {
	}

	public void HandleCompletion() {
		GetComponent<GuardController> ().Stop ();

		Tile currentTile = new Tile(GetComponent<TileItem>().tileX, GetComponent<TileItem>().tileY);

		// Case where guard has reached last known place where player was spotted.
		if (currentTile.Equals (currentTarget)) {
			// Look around in confusion, then go back to patrolling.
			GetComponent<StateMachine> ().CurrentState = GetComponent<LookState> ();
		} else {
			StartChase ();
		}
	}

    public void OnTriggerEnter2D(Collider2D other) {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null) {
            player.catchPlayer();
        }
    }
}