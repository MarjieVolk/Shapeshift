using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    private const float SCALE = 1.0f;
    private const float CAMERA_SPEED = 0.4f;

    private TileItem playerTileItem;

	// Use this for initialization
	void Start () {
        playerTileItem = FindObjectOfType<PlayerController>().gameObject.GetComponent<TileItem>();

        Camera.main.orthographicSize = Screen.height / (100.0f * 2.0f * SCALE);

        transform.position = getCurrentRoom().transform.position;
    }

    void Update() {
        Room room = getCurrentRoom();

        // Ignore hallways (aka tiny rooms less than 3 tall and less than 2 wide)
        if (room.GetComponent<TileItem> ().tileH < 3 && room.GetComponent<TileItem> ().tileW < 2) {
            return;
        }

        Vector3 diff = room.transform.position - transform.position;

        if (diff.magnitude < CAMERA_SPEED) {
            transform.position = room.transform.position;
        } else {
            transform.position += diff.normalized * CAMERA_SPEED;
        }
    }

    private Room getCurrentRoom() {
        List<Room> potentialRooms = TileItem.GetObjectsAtPosition<Room>(playerTileItem.tileX, playerTileItem.tileY);

        if (potentialRooms.Count != 1) {
            Debug.LogError("Found " + potentialRooms.Count + " rooms at (" + playerTileItem.tileX + ", " + playerTileItem.tileY + ")");
        }

        return potentialRooms[0];
    }
}
