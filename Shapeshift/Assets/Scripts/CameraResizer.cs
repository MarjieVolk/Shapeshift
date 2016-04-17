using UnityEngine;
using System.Collections;

public class CameraResizer : MonoBehaviour {

    private const float SCALE = 1.0f;

	// Use this for initialization
	void Start () {
        Camera.main.orthographicSize = Screen.height / (100.0f * 2.0f * SCALE);
        Debug.Log("Screen height: " + Screen.height + "  Camera size: " + Camera.main.orthographicSize);
	}
}
