using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    private const float SCALE = 1.0f;
    private const float CAMERA_SPEED = 0.4f;

    private TileItem player;

    private int prevScreenHeight;

	// Use this for initialization
	void Start () {
        player = FindObjectOfType<PlayerController>().gameObject.GetComponent<TileItem>();
        
        fitToScreen();
    }

    void Update() {
        transform.position = player.transform.position;

        if (prevScreenHeight != Screen.height) {
            fitToScreen();
        }
    }

    private void fitToScreen() {
        prevScreenHeight = Screen.height;
        Camera.main.orthographicSize = Screen.height / (100.0f * 2.0f * SCALE);
    }
}
