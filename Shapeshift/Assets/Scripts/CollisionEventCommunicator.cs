using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CollisionEventCommunicator : MonoBehaviour {

    public delegate void CollisionHandler(GameObject obj);
    public event CollisionHandler OnTriggerEnter;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll) {
        if (OnTriggerEnter != null)
        {
            OnTriggerEnter(coll.gameObject);
        }
    }
}
