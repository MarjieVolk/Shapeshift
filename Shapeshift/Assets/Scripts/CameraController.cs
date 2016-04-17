using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    private const float SCALE = 1.0f;

    private TileItem playerTileItem;
    private Room previousRoom;
    private Room currentRoom;

	// Use this for initialization
	void Start () {
        playerTileItem = FindObjectOfType<PlayerController>().gameObject.GetComponent<TileItem>();

        Camera.main.orthographicSize = Screen.height / (100.0f * 2.0f * SCALE);
        Debug.Log("Screen height: " + Screen.height + "  Camera size: " + Camera.main.orthographicSize);

        previousRoom = getCurrentRoom();
        transform.position = previousRoom.transform.position;
    }

    void Update() {
        Room room = getCurrentRoom();
        
        if (room != previousRoom) {

        }

        previousRoom = room;
    }

    private Room getCurrentRoom() {
        List<Room> potentialRooms = TileItem.GetObjectsAtPosition<Room>(playerTileItem.tileX, playerTileItem.tileY);

        if (potentialRooms.Count != 1) {
            Debug.LogError("Found " + potentialRooms.Count + " rooms at (" + playerTileItem.tileX + ", " + playerTileItem.tileY + ")");
        }

        return potentialRooms[0];
    }
}
