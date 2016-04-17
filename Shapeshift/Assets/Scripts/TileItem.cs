using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TileItem : MonoBehaviour
{
    public static int TILE_SIZE = 40;
	private static Dictionary<long, List<GameObject>> _tileMap;

	private int tileX { get; set; }
	private int tileY { get; set; }
	public int tileW { get; set; } // TODO(aklen): Auto-set these based off of type/furniture type, etc.
	public int tileH { get; set; }

    public void Start () {
        SnapToGrid ();
		AddToTileMap ();
    }

    public void SnapToGrid () {
        // Set tile position based off of starting transform.
        tileX = GlobalToTilePosition (transform.position.x);
		tileY = GlobalToTilePosition (transform.position.y);

        // Snap transform to grid.
        transform.position = new Vector3(tileX * TILE_SIZE, tileY * TILE_SIZE, transform.position.z);
    }
		
	private static long ToKey(int x, int y) {
		return (((long)x) << 32) + y;
	}

	public static int GlobalToTilePosition(float p) {
		return Mathf.RoundToInt (p / TILE_SIZE);
	}

	/**
	 * Call anytime you want to change position in order to update tile positions.
	 */
	public void MovePosition(Vector3 newPos) {
		int newTileX = GlobalToTilePosition (newPos.x);
		int newTileY = GlobalToTilePosition (newPos.y);
		bool tilePosMoved = newTileX != tileX || newTileY != tileY;
		if (tilePosMoved) {
			RemoveFromTileMap ();
		}
		tileX = newTileX;
		tileY = newTileY;
		transform.position = newPos;
		if (tilePosMoved) {
			AddToTileMap ();
		}
	}

	private void RemoveFromTileMap() {
		List<GameObject> ents;
		ForEachOfMyTiles ((int x, int y) => {
			if (_tileMap.TryGetValue (ToKey (x, y), out ents)) {
				ents.Remove (gameObject);
			}
		});
	}

	private void AddToTileMap() {
		List<GameObject> ents;
		ForEachOfMyTiles ((int x, int y) => {
			long key = ToKey (x, y);
			if (_tileMap.TryGetValue (key, out ents)) {
				ents.Add (gameObject);
			} else {
				ents = new List<GameObject>();
				ents.Add(gameObject);
				_tileMap.Add(key, ents);
			}
		});
	}

	private void ForEachOfMyTiles(Action<int, int> func) {
		for (int ix = 0; ix < tileW; ix++) {
			for (int iy = 0; iy < tileH; iy++) {
				func (tileX + ix, tileY + iy);
			}
		}
	}

	public static List<GameObject> GetObjectsAtPosition(int tileX, int tileY) {
		List<GameObject> entities;
		if (_tileMap.TryGetValue (ToKey (tileX, tileY), out entities)) {
			return entities;
		}
		return new List<GameObject>();
	}
}

