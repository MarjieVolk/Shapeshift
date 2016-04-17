using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
public class Room : MonoBehaviour {

    public const int ROOM_TILE_Z_INDEX = 999;

    public RoomTileSet tileSet;

    private TileItem tileItem;

    // Use this for initialization
    void Start () {
        tileItem = gameObject.GetComponent<TileItem>();

        for (int i = 0; i < tileItem.tileW; i++) {
            for (int j = 0; j < tileItem.tileH; j++) {
                GameObject floorTile = new GameObject();
                floorTile.transform.SetParent(this.transform);
                floorTile.transform.position = new Vector3(
                    transform.position.x + (i * TileItem.TILE_SIZE), 
                    transform.position.y + (j * TileItem.TILE_SIZE),
                    ROOM_TILE_Z_INDEX);
				SpriteRenderer sprite = floorTile.AddComponent<SpriteRenderer> ();
				sprite.sprite = tileSet.floor;
            }
        }
    }

    // Update is called once per frame
    void Update () {

    }

    public HashSet<FurnitureItem> getAllFurniture() {
        HashSet<FurnitureItem> furniture = new HashSet<FurnitureItem>();
        for (int x = tileItem.tileX; x < tileItem.tileX + tileItem.tileW; x++) {
            for (int y = tileItem.tileY; y < tileItem.tileY + tileItem.tileH; y++) {
                List<FurnitureItem> furnitureAt = TileItem.GetObjectsAtPosition<FurnitureItem>(x, y);
                furniture.UnionWith(furnitureAt);

                if (furnitureAt.Count > 1) {
                    Debug.LogError("Found " + furnitureAt.Count + " furniture items at (" + x + ", " + y + ")");
                }
            }
        }

        return furniture;
    }
}
