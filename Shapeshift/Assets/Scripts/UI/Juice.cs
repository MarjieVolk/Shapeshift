using UnityEngine;
using System.Collections;

public class Juice : MonoBehaviour {
    public float InitialSpeed;
    public float Acceleration;

    public Vector3 Destination;

    private Vector2 _velocity;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().velocity = Random.insideUnitCircle * InitialSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        // accelerate towards the destination
        Vector2 towardsDestination = Destination - transform.position;
        towardsDestination.Normalize();
        towardsDestination *= Acceleration * Time.deltaTime;
        _velocity += towardsDestination;

        // move the juice
        GetComponent<Rigidbody2D>().velocity += towardsDestination;
	}
}
