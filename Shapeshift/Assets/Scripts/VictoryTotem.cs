using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CollisionEventCommunicator))]
public class VictoryTotem : MonoBehaviour {

    public delegate void VictoryHandler();
    public event VictoryHandler OnVictory;
    public bool WinOnAcquire = true;
    public Collider2D EscapeZone;
    public Vector2 CarryOffset;

    private Vector2 oldPosition;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.GetComponent<PlayerController>() != null)
        {
            totemReached(coll.gameObject);
        }
    }

    private void totemReached(GameObject player)
    {
        if (WinOnAcquire)
        {
            ActivateVictory();
        }
        else
        {
            // disable collision on this
            GetComponent<Collider2D>().enabled = false;
            // attach something to the player
            transform.SetParent(player.transform);
            transform.localPosition = CarryOffset;
            // once the player returns to the escape zone, win!
            EscapeZone.GetComponent<CollisionEventCommunicator>().OnTriggerEnter += (go) =>
            {
                if (go == player)
                {
                    ActivateVictory();
                }
            };
            // TODO if the player dies, return to other place
        }
    }

    private void ActivateVictory()
    {
        Debug.Log("Victory!!");
        if(OnVictory != null)
        {
            OnVictory();
        }
    }
}
