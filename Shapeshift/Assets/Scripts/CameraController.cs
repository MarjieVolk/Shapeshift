using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    private const float SCALE = 1.0f;

    private TileItem playerTileItem;

	// Use this for initialization
	void Start () {
        playerTileItem = FindObjectOfType<PlayerController>().gameObject.GetComponent<TileItem>();

        Camera.main.orthographicSize = Screen.height / (100.0f * 2.0f * SCALE);
        Debug.Log("Screen height: " + Screen.height + "  Camera size: " + Camera.main.orthographicSize);
	}

    void Update() {
        List<Room> potentialRooms = TileItem.GetObjectsAtPosition<Room>(playerTileItem.tileX, playerTileItem.tileY);

        if (potentialRooms.Count != 1) {
            Debug.LogError("Found " + potentialRooms.Count + " rooms at (" + playerTileItem.tileX + ", " + playerTileItem.tileY + ")");
        }

        Room room = potentialRooms[0];
        transform.position = room.transform.position;
    }
}
