using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InvestigatingFurnitureState : State {

    public float ApproachSpeed = 1;

    private FurnitureItem investigationTarget;

    public void HandleFurnitureInvestigation(FurnitureItem item)
    {
        Debug.Log("Handling investigation of " + item);
        investigationTarget = item;
        GuardController controller = GetComponent<GuardController>();
        // cancel current movement, if any
        if (controller.GetIsMoving())
        {
            controller.Stop();
        }

        Tile myTile = new Tile(GetComponent<TileItem>());
        Tile destinationTile = new Tile(investigationTarget.GetComponent<TileItem>());
        List<Tile> path = Pathfinding.FindPath(myTile, destinationTile, false);
        controller.Move(
            path.GetRange(0, path.Count - 1),
            ApproachSpeed,
            OnApproachFinish,
            OnApproachInterrupted);

        GetComponent<StateMachine>().CurrentState = GetComponent<InvestigatingFurnitureState>();
    }

    private void OnApproachFinish()
    {
        Debug.Log("Move finished");
        if (investigationTarget == null)
        {
            Debug.Log("Move finished AFTER it was interrupted.");
            return;
        }

        // TODO: check whether player is nearby before catching them

        // do different things if it's a player
        if (investigationTarget.gameObject.transform.parent != null
            && investigationTarget.gameObject.transform.parent.GetComponent<PlayerTransformer>() != null)
        {
            // handle player caught as furniture
            Debug.Log("Found you!");
            PlayerCaughtHandler player = investigationTarget.transform.parent.GetComponent<PlayerCaughtHandler>();
            if (player != null)
            {
                Debug.Log("catching you!");
                player.catchPlayer();
            }
        }
        else
        {
            // handle non-player furniture suspected
            // TODO
        }

        investigationTarget = null;
        GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
    }

    private void OnApproachInterrupted()
    {
        Debug.Log("Move interrupted!");
        if (investigationTarget == null)
        {
            Debug.Log("Move interrupted AFTER it finished.");
            return;
        }
        if ((investigationTarget.transform.position - transform.position).magnitude < 0.1f)
        {
            OnApproachFinish();
        }
        investigationTarget = null;
        GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
