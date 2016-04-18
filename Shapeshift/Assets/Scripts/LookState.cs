using UnityEngine;
using System.Collections;

public class LookState : State {
    private DirectionComponent _directionHolder;

    public Direction FirstDirection = Direction.SOUTH;

    public int LookTime;

    private int lookTimer;

    void Start()
    {
        _directionHolder = GetComponent<DirectionComponent>();
    }

    // Use this for initialization
    void OnEnable () {
        GetComponent<DirectionComponent>().Direction = FirstDirection;
        lookTimer = LookTime;
	}
	
	// Update is called once per frame
	void Update () {
        if (lookTimer <= 0)
        {
            _directionHolder.Direction = Clockwise(_directionHolder.Direction);
            if (_directionHolder.Direction == FirstDirection)
            {
                // transition to move state here
				Debug.Log("Switching to Move State! :D\n");
                GetComponent<StateMachine>().CurrentState = GetComponent<MoveState>();
            }
            else
            {
                lookTimer = LookTime;
            }
        }
        else
        {
            lookTimer -= 1;
        }
    }

    private Direction Clockwise(Direction fromMe)
    {
        if (fromMe == Direction.NORTH)
        {
            return Direction.EAST;
        }
        else if (fromMe == Direction.EAST)
        {
            return Direction.SOUTH;
        }
        else if (fromMe == Direction.SOUTH)
        {
            return Direction.WEST;
        }
        else
        {
            return Direction.NORTH;
        }
    }
}
