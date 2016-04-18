using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// A class for managing state-machine-based AIs
/// </summary>
public class StateMachine : MonoBehaviour {

    /// <summary>
    /// The state the guard should start in, and default to if all others are disabled.
    /// </summary>
    public State initialState;

    private State _currentState;
    public State CurrentState
    {
        get { return _currentState; }
        set
        {
			if (_currentState != value) {
				if (_currentState != null) {
					_currentState.enabled = false;
				}
				_currentState = value;
				_currentState.enabled = true;
			}
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
        if (GetComponents<State>().All((state) => { return !state.enabled; })) {
            initialState.enabled = true;
        }
	}
}
