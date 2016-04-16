using UnityEngine;
using System.Collections;

public class TileItem : MonoBehaviour
{
    public static int TILE_SIZE = 40;

    public int tileX;
    public int tileY;

    public void Start () {
        snapToGrid ();
    }

    public void snapToGrid () {
        // Set tile position based off of starting transform.
        tileX = Mathf.RoundToInt (transform.position.x / TILE_SIZE);
        tileY = Mathf.RoundToInt (transform.position.y / TILE_SIZE);

        // Snap transform to grid.
        transform.position = new Vector3(tileX * TILE_SIZE, tileY * TILE_SIZE, transform.position.z);
    }
}

