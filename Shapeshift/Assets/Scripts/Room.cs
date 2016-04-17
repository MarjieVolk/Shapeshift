using UnityEngine;
using System.Collections;

[RequireComponent (typeof (TileItem))]
public class Room : MonoBehaviour {

    public const int ROOM_TILE_Z_INDEX = 999;

    public RoomTileSet tileSet;

    public int width;
    public int height;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                GameObject floorTile = new GameObject();
                floorTile.transform.SetParent(this.transform);
                floorTile.transform.position = new Vector3(
                    transform.position.x + (i * TileItem.TILE_SIZE), 
                    transform.position.y + (j * TileItem.TILE_SIZE),
                    ROOM_TILE_Z_INDEX);
                floorTile.AddComponent<SpriteRenderer>().sprite = tileSet.floor;
            }
        }
    }

    // Update is called once per frame
    void Update () {

    }
}
