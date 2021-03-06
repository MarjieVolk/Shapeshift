﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TileItem : MonoBehaviour
{
    public const float TILE_SIZE = 0.32f;

    private static Dictionary<long, List<GameObject>> _tileMap = new Dictionary<long, List<GameObject>>();

    public int tileX { get; private set; }
    public int tileY { get; private set; }

    public int startingTileWidth = 1;
    public int startingTileHeight = 1;

    public Vector3 gridOffset;

    private int _tileW;
    public int tileW {
        get { return _tileW; }
        set { _SetSize(value, _tileH); }
    }
    private int _tileH;
    public int tileH {
        get { return _tileH; }
        set { _SetSize (_tileW, value); }
    }

    public void Awake () {
        _tileW = startingTileWidth;
        _tileH = startingTileHeight;
        SnapToGrid ();
        AddToTileMap ();
    }

    void OnDestroy() {
        RemoveFromTileMap();
    }

    private static long ToKey(int x, int y) {
        return (((long)x) << 32) + y;
    }

    public static Tile CreateTileAt(Vector3 position) {
        Tile t = new Tile(GlobalToTilePosition(position.x), GlobalToTilePosition(position.y));
        return t;
    }

    public static int GlobalToTilePosition(float p) {
        return Mathf.RoundToInt (p / TILE_SIZE);
    }

    public static float TileToGlobalPosition(int p) {
        return p * TILE_SIZE;
    }

    public Vector3 getCenterPosition() {
        float xOffset = (TileToGlobalPosition(tileX + tileW) - TileToGlobalPosition(tileX)) / 2f;
        float yOffset = (TileToGlobalPosition(tileY + tileH) - TileToGlobalPosition(tileY)) / 2f;

        return new Vector3(TileToGlobalPosition(tileX) + xOffset, TileToGlobalPosition(tileY) + yOffset, transform.position.z);
    }

    /// <summary>
    /// Set tile position based on transform position, and snap to tile location
    /// </summary>
    public void SnapToGrid() {
        // Set tile position based off of starting transform.
        tileX = GlobalToTilePosition(transform.position.x - gridOffset.x);
        tileY = GlobalToTilePosition(transform.position.y - gridOffset.y);

        // Snap transform to grid.
        _SetPosition(tileX, tileY, new Vector3(tileX * TILE_SIZE, tileY * TILE_SIZE) + gridOffset);
    }

    /// <summary>
    /// Set transform position to newPos, and update tile location to the closest tile.
    /// </summary>
    /// <param name="newPos"></param>
    public void SetGlobalPosition(Vector3 newPos) {
        _SetPosition (GlobalToTilePosition (newPos.x), GlobalToTilePosition (newPos.y), newPos);
    }

    /// <summary>
    /// Set tile position and snap to tile location
    /// </summary>
    /// <param name="newTileX"></param>
    /// <param name="newTileY"></param>
    public void SetTilePosition(int newTileX, int newTileY) {
        _SetPosition (newTileX, newTileY, new Vector3(newTileX * TILE_SIZE, newTileY * TILE_SIZE) + gridOffset);
    }

    private void _SetPosition(int newTileX, int newTileY, Vector3 newPos) {
        bool tilePosMoved = newTileX != tileX || newTileY != tileY;
        if (tilePosMoved) {
            RemoveFromTileMap ();
        }

        tileX = newTileX;
        tileY = newTileY;

        transform.position = new Vector3(newPos.x, newPos.y, newPos.y);
        if (tilePosMoved) {
            AddToTileMap ();
        }
    }

    private void _SetSize(int newWidth, int newHeight) {
        bool tileResized = newWidth != _tileW || newHeight != _tileH;
        if (tileResized) {
            RemoveFromTileMap ();
        }
        _tileW = newWidth;
        _tileH = newHeight;
        if (tileResized) {
            AddToTileMap ();
        }
    }

    /** Don't call this unless destroying the item. */
    public void RemoveFromTileMap() {
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

    public static bool DoesPlacementCollideWithThings(int tileX, int tileY, int tileW, int tileH) {

      // Debug.Log("DoesPlacementCollideWithThings " + tileX + ", " + tileY + ", " + tileW + ", " + tileH);
      for (int ix = 0; ix < tileW; ix++) {
        for (int iy = 0; iy < tileH; iy++) {
          int x = tileX + ix;
          int y = tileY + iy;
          if (GetObjectsAtPosition<Wall>(x, y).Count > 0) {
            return true;
          } else if (GetObjectsAtPosition<FurnitureItem>(x, y).Count > 0) {
            return true;
          } else if (GetObjectsAtPosition<Guard>(x, y).Count > 0) {
            return true;
          }
        }
      }
      return false;
    }

    public static List<T> GetObjectsAtPosition<T>(int tileX, int tileY) where T: Component {
        List<T> matchingEntities = new List<T>();
        List<GameObject> allEntities;
        if (_tileMap.TryGetValue (ToKey (tileX, tileY), out allEntities)) {
            foreach (GameObject go in allEntities) {
                T nullableT = go.GetComponent<T> ();
                if (nullableT != null) {
                    matchingEntities.Add (nullableT);
                }
            }
        }
        return matchingEntities;
    }
}

