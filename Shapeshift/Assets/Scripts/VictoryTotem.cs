using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CollisionEventCommunicator))]
public class VictoryTotem : MonoBehaviour {

    public static VictoryTotem INSTANCE;

    public delegate void VictoryHandler();
    public event VictoryHandler OnVictory;

    public event VictoryHandler OnVictoryTotemAcquired;

    public bool WinOnAcquire = true;
    public Collider2D EscapeZone;
    public Vector2 CarryOffset;

    private Vector2 oldPosition;

	// Use this for initialization
	void Awake () {
        INSTANCE = this;
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
        if (OnVictoryTotemAcquired != null) {
            OnVictoryTotemAcquired();
        }

        if (WinOnAcquire)
        {
            ActivateVictory();
        }
        else
        {
            // disable collision on this
            GetComponent<Collider2D>().enabled = false;
            // attach something to the player
            oldPosition = transform.position;
            transform.SetParent(player.transform);
            transform.localPosition = CarryOffset;
            // once the player returns to the escape zone, win!
            EscapeZone.GetComponent<CollisionEventCommunicator>().OnTriggerEnter += PlayerEscapeHandler;

            // TODO if the player dies, return to other place
            player.GetComponent<PlayerCaughtHandler>().PlayerCaughtAsHuman += resetTotem;
        }
    }

    private void PlayerEscapeHandler(GameObject go)
    {
        if (go.GetComponent<PlayerController>() != null)
        {
            ActivateVictory();
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

    private void resetTotem()
    {
        // undo the work of totemReached
        GetComponent<Collider2D>().enabled = true;

        transform.SetParent(null);
        transform.position = oldPosition;

        EscapeZone.GetComponent<CollisionEventCommunicator>().OnTriggerEnter -= PlayerEscapeHandler;

        FindObjectOfType<PlayerCaughtHandler>().PlayerCaughtAsHuman -= resetTotem;
    }
}
