using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerTransformer : MonoBehaviour {

    private GameObject currentTransformation;

    public delegate void PlayerTransformedHandler(GameObject target);
    public event PlayerTransformedHandler PlayerTransformed;

    public void TransformPlayer(GameObject target)
    {
        if(currentTransformation != null)
        {
            RevertPlayer();
        }

        if (PlayerTransformed != null) {
            PlayerTransformed(target);
        }
    }

    public void RevertPlayer()
    {
        if (PlayerTransformed != null)
        {
            PlayerTransformed(null);
        }
    }

    // Use this for initialization
    void Start () {
        PlayerTransformed += (target) =>
        {
            GetComponent<SpriteRenderer>().enabled = (target == null);
            GetComponent<Rigidbody2D>().isKinematic = (target != null);
        };
        PlayerTransformed += (target) =>
        {

            Destroy(currentTransformation);
            if (target != null)
            {
                Tile maybePos = FindNonCollidingPlacement(
                    transform.position,
                    target.GetComponent<TileItem>());
                if (maybePos == null) {
                    Debug.Log("Could not fit!");
                    RevertPlayer();
                } else {
                    currentTransformation = (GameObject) Instantiate(target, transform.position, Quaternion.identity);
                    currentTransformation.GetComponent<TileItem>().SetTilePosition(maybePos.X, maybePos.Y);
                    currentTransformation.transform.SetParent(transform);
                }
            }
        };
    }

    /** Nullable! */
    private Tile FindNonCollidingPlacement(Vector3 pos, TileItem targetTile) {
        List<Tile> tryPositions = new List<Tile>();
        // First try centered at current location.
        int firstX = TileItem.GlobalToTilePosition(pos.x);
        int firstY = TileItem.GlobalToTilePosition(pos.y);
        tryPositions.Add(new Tile(firstX - targetTile.tileW / 2, firstY - targetTile.tileH / 2));

        // Next try centered at second-closest location.
        int secondX, secondY;
        if (Math.Abs(TileItem.TileToGlobalPosition(firstX) - pos.x) > Math.Abs(TileItem.TileToGlobalPosition(firstY) - pos.y)) {
            // X dist moved greater, use other position as other x translation.
            secondX = TileItem.GlobalToTilePosition(pos.x - (TileItem.TileToGlobalPosition(firstX) - pos.x));
            secondY = firstY;
        } else {
            // Y dist moved greater, use other position as other y translation.
            secondX = firstX;
            secondY = TileItem.GlobalToTilePosition(pos.y - (TileItem.TileToGlobalPosition(firstY) - pos.y));
        }
        tryPositions.Add(new Tile(secondX - targetTile.tileW / 2, secondY - targetTile.tileH / 2));

        // Fuckit, add all other positions around player.
        for (int ix = firstX - targetTile.tileW/2 - 1; ix <  + targetTile.tileW/2 + 1; ix++) {
            for (int iy = firstY - targetTile.tileH/2 - 1; iy < firstY + targetTile.tileH/2 + 1; iy++) {
                tryPositions.Add(new Tile(ix, iy));
            }
        }

        // Try them out.
        foreach (Tile tryAt in tryPositions) {
            Debug.Log("Trying " + tryAt);
            if (!TileItem.DoesPlacementCollideWithThings(tryAt.X, tryAt.Y, targetTile.tileW, targetTile.tileH)) {
                return tryAt;
            }
        }

        return null;
    }

    public GameObject getTransformation() {
        return currentTransformation;
    }

    // Update is called once per frame
    void Update () {

    }
}
