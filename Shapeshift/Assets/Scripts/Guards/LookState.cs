using UnityEngine;
using System.Collections;

public class LookState : State {
    private DirectionComponent _directionHolder;

    public Direction FirstDirection = Direction.SOUTH;

    public float LookTime;

    private float lookTimer;

    void Start()
    {
        _directionHolder = GetComponent<DirectionComponent>();
    }

    // Use this for initialization
    void OnEnable () {
        GetComponent<DirectionComponent>().Direction = FirstDirection;
        lookTimer = Time.time + LookTime;
	}
	
	// Update is called once per frame
    void Update()
    {
        if (lookTimer <= Time.time)
        {
            _directionHolder.Direction = Clockwise(_directionHolder.Direction);
            if (_directionHolder.Direction == FirstDirection)
            {
                // transition to move state here
                GetComponent<StateMachine>().CurrentState = GetComponent<MoveState>();
            }
            else
            {
                lookTimer = Time.time + LookTime;
            }
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
