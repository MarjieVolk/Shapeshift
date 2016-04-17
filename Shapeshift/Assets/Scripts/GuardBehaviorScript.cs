using UnityEngine;
using System.Collections;

[RequireComponent (typeof (TileItem))]
public class GuardBehaviorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//gameObject.GetComponent<SpriteRenderer> ();
		//gameObject.transform.
		//gameObject.scene.GetRootGameObjects
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<TileItem>().MovePosition(transform.position + new Vector3(1, 1));
	}
}
