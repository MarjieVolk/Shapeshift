﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(CollisionEventCommunicator))]
public class ExitStairs : MonoBehaviour {

    public static ExitStairs INSTANCE;

    private CollisionEventCommunicator.CollisionHandler escapeHandler;

    // Use this for initialization
    void Start () {
        INSTANCE = this;
        GetComponent<SpriteRenderer>().enabled = false;
	}

    public void enableEscape(CollisionEventCommunicator.CollisionHandler escapeHandler) {
        if (this.escapeHandler != null) {
            GetComponent<CollisionEventCommunicator>().OnTriggerEnter -= this.escapeHandler;
        }

        this.escapeHandler = escapeHandler;
        GetComponent<CollisionEventCommunicator>().OnTriggerEnter += escapeHandler;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void disableEscape() {
        GetComponent<CollisionEventCommunicator>().OnTriggerEnter -= escapeHandler;
        escapeHandler = null;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
