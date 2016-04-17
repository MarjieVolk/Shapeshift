using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
[ExecuteInEditMode]
public class Room : MonoBehaviour {

    public const int ROOM_TILE_Z_INDEX = 999;

    public RoomTileSet tileSet;

    private TileItem tileItem;

    private GameObject floorTileParent;

    // Use this for initialization
    void Start () {
        tileItem = gameObject.GetComponent<TileItem>();

        floorTileParent = new GameObject();
        floorTileParent.name = "Floor Tile Parent";
        floorTileParent.transform.SetParent(this.transform);

        generateFloorSprites();
    }

    // Update is called once per frame
    void Update () {

    }

    public void generateFloorSprites() {
        clearFloorSprites();
        
        if (tileItem.tileW == 0) {
            // Being called in editor
            _generateFloorSprites(tileItem.startingTileWidth, tileItem.startingTileHeight);
        } else {
            _generateFloorSprites(tileItem.tileW, tileItem.tileH);
        }
    }

    private void _generateFloorSprites(int width, int height) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                GameObject floorTile = new GameObject();
                floorTile.name = "floorTile (" + i + ", " + j + ")";
                floorTile.transform.SetParent(floorTileParent.transform);
                floorTile.transform.position = new Vector3(
                    transform.position.x + (i * TileItem.TILE_SIZE),
                    transform.position.y + (j * TileItem.TILE_SIZE),
                    ROOM_TILE_Z_INDEX);
                SpriteRenderer sprite = floorTile.AddComponent<SpriteRenderer>();
                sprite.sprite = tileSet.floor;
            }
        }
    }

    public void clearFloorSprites() {
        if (floorTileParent != null) {
            while (floorTileParent.transform.childCount > 0) {
                DestroyImmediate(floorTileParent.transform.GetChild(0).gameObject);
            }
        }
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
