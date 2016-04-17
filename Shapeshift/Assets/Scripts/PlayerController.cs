using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
public class PlayerController : MonoBehaviour {
    public float Speed;

    private bool movementEnabled = true;

    private Dictionary<KeyCode, Vector2> _keyConfiguration = new Dictionary<KeyCode, Vector2>();

	// Use this for initialization
	void Start ()
    {
        _keyConfiguration[KeyCode.W] = Vector2.up;
        _keyConfiguration[KeyCode.A] = Vector2.left;
        _keyConfiguration[KeyCode.S] = Vector2.down;
        _keyConfiguration[KeyCode.D] = Vector2.right;

        _keyConfiguration[KeyCode.UpArrow] = Vector2.up;
        _keyConfiguration[KeyCode.LeftArrow] = Vector2.left;
        _keyConfiguration[KeyCode.DownArrow] = Vector2.down;
        _keyConfiguration[KeyCode.RightArrow] = Vector2.right;

        GetComponent<PlayerTransformer>().PlayerTransformed += (target) =>
        {
            movementEnabled = (target == null);
        };
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 translation = Vector2.zero;
        foreach (KeyValuePair<KeyCode, Vector2> key in _keyConfiguration)
        {
            if (Input.GetKey(key.Key))
            {
                translation += key.Value;
            }
        }

        translation.Normalize();
        translation *= Speed;
        if (movementEnabled)
        {
            gameObject.GetComponent<TileItem>().MovePosition(transform.position + new Vector3(translation.x, translation.y));
        }
	}
}
