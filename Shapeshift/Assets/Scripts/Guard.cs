using UnityEngine;
using System.Collections;

[RequireComponent (typeof (TileItem))]
public class Guard : MonoBehaviour {

	//private int currentWaypoint;

	// Use this for initialization
	void Start () {
		//currentWaypoint = GetNextWaypoint ();
		//gameObject.GetComponent<SpriteRenderer> ();
		//gameObject.transform.
		//gameObject.scene.GetRootGameObjects
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<TileItem>().MovePosition(transform.position + new Vector3(1, 1));
	}

	int GetNextWaypoint() {
		//gameObject.
		return 1;
	}
}
