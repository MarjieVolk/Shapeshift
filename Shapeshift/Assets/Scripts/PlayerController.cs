﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (TileItem))]
public class PlayerController : MonoBehaviour {
    public float Speed;

    private Vector3 startPosition;

    private bool movementEnabled = true;

    private Dictionary<KeyCode, Vector2> _keyConfiguration = new Dictionary<KeyCode, Vector2>();

    private Animator animator;

    private Vector2[] animationDirections;

	// Use this for initialization
	void Start ()
    {
        startPosition = transform.position;

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

        this.animator = GetComponent<Animator>();

        animationDirections = new Vector2[]
        {
            Vector2.down,
            Vector2.right,
            Vector2.up,
            Vector2.left
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

        if (!movementEnabled && translation.magnitude > 0) {
            GetComponent<PlayerTransformer>().TransformPlayer(null);
        }

        if (movementEnabled) {
            if (translation.magnitude > 0)
            {
                gameObject.GetComponent<TileItem>().SetGlobalPosition(transform.position + new Vector3(translation.x, translation.y));
                for (int i = 0; i < animationDirections.Length; i++)
                {
                    if (Vector2.Dot(translation.normalized, animationDirections[i]) > Mathf.Sqrt(2) / 2 + 0.01)
                    {
                        animator.SetInteger("Direction", i);
                    }
                }
            }
            else
            {
                animator.SetInteger("Direction", -1);
            }
        }
    }

    public void catchPlayer() {
        GameObject currentTransformation = GetComponent<PlayerTransformer>().getTransformation();
        if (currentTransformation == null) {
            // Transport player to beginning
            gameObject.GetComponent<TileItem>().SetGlobalPosition(startPosition);
        } else {
            // Lose access to item, gain catching cooldown
            FindObjectOfType<CardPanel>().remove(currentTransformation.GetComponent<PlayableFurnitureItem>());
            GetComponent<PlayerTransformer>().TransformPlayer(null);
        }
    }
}
