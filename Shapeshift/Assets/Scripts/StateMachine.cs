using UnityEngine;
using System.Collections;

/// <summary>
/// A class for managing state-machine-based AIs
/// </summary>
public class StateMachine : MonoBehaviour {

    public State initialState;

    private State _currentState;
    public State CurrentState
    {
        get { return _currentState; }
        set
        {
            if (_currentState != null)
            {
                _currentState.enabled = false;
            }
            _currentState = value;
            _currentState.enabled = true;
        }
    }

	// Use this for initialization
	void Start () {
        foreach (State state in GetComponents<State>())
        {
            state.enabled = false;
        }
        CurrentState = initialState;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
