using UnityEngine;
using System.Collections;

[RequireComponent (typeof (TileItem))]
public class GuardWaypoint : MonoBehaviour {

	// A lower ordering is earlier in a guard's route.
	// Should preferably be a positive integer, although making it negative won't break the game.
	public int Ordering;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Tile getTile() {
		TileItem tileItem = gameObject.GetComponent<TileItem> ();
		return new Tile (tileItem.tileX, tileItem.tileY);
	}
}
