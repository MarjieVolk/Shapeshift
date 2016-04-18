using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CollisionEventCommunicator : MonoBehaviour {
    
    public event UnityAction OnCollisionEnter;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D() {
        OnCollisionEnter();
    }
}
