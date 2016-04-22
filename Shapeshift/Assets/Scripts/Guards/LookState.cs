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
        //turn 2pi radians in LookTime seconds
        _directionHolder.Angle += Time.deltaTime / LookTime * Mathf.PI * 2;
        if (Time.time > lookTimer)
        {
            GetComponent<StateMachine>().CurrentState = GetComponent<MoveState>();
        }
    }
}
