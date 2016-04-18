using UnityEngine;
using System.Collections;

public class LookState : State {

    private Direction currentDirection;
    public Direction FirstDirection = Direction.SOUTH;

    public int LookTime;

    private int lookTimer;

    // Use this for initialization
    void OnEnable () {
        currentDirection = FirstDirection;
        lookTimer = LookTime;
	}
	
	// Update is called once per frame
	void Update () {
        if (lookTimer <= 0)
        {
            currentDirection = Clockwise(currentDirection);
            if (currentDirection == FirstDirection)
            {
                // transition to move state here
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
