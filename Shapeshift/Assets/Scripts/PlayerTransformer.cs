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
        int width = targetTile.startingTileWidth;
        int height = targetTile.startingTileHeight;

        List<Tile> tryPositions = new List<Tile>();
        int firstX = TileItem.GlobalToTilePosition(pos.x);
        int firstY = TileItem.GlobalToTilePosition(pos.y);

        for (int ix = firstX - width - 1; ix < firstX + 1; ix++) {
            for (int iy = firstY - height - 1; iy < firstY + 1; iy++) {
                tryPositions.Add(new Tile(ix, iy));
            }
        }

        Func<Tile, float> distToPlayerSquared = (Tile p) =>
            Mathf.Pow(TileItem.TileToGlobalPosition(p.X) + width/2f - pos.x, 2)
            + Mathf.Pow(TileItem.TileToGlobalPosition(p.Y) + height/2f - pos.y, 2);

        tryPositions.Sort( (p1, p2) => - distToPlayerSquared(p1).CompareTo(distToPlayerSquared(p2)) );


        // Try them out.
        foreach (Tile tryAt in tryPositions) {
            if (!TileItem.DoesPlacementCollideWithThings(tryAt.X, tryAt.Y, width, height)) {
                Debug.Log("Transforming at " + tryAt);
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
