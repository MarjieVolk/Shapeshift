using UnityEngine;
using System.Collections;

public class GuardDuty : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerable getWaypoints()
	{
		return new Waypoints ();
	}

	public class Waypoints : IEnumerable
	{
		public IEnumerator GetEnumerator()
		{
			yield return 1;
		}
	}
}
