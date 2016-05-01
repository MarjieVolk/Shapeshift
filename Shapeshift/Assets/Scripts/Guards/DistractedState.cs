using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DistractedState : MonoBehaviour {
    /// <summary>
    /// The number of state activations between each distraction.
    /// </summary>
    public int DistractionPeriod;
    public State NextState;
    /// <summary>
    /// The number of seconds to be distracted for each time.
    /// </summary>
    public float DistractionDuration;

    private int activationsSinceDistraction = 0;
    private TileItem currentDistraction;
    private List<Tile> pathToDistraction;

	// Use this for initialization
	void Start () {
	
	}

    void OnEnable()
    {
        activationsSinceDistraction++;
        if(activationsSinceDistraction < DistractionPeriod)
        {
            // still determined enough to resist temptation!
            GetComponent<StateMachine>().CurrentState = NextState;
            return;
        }
        else
        {
            // oooooooh, shiny!
            // find the closest distraction
            pathToDistraction = Pathfinding.FindPath(new Tile(GetComponent<TileItem>()), findClosestDistraction(), false);

            // assume the distraction position
            // cease distraction and resume look & patrol
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private Tile findClosestDistraction()
    {
        Tile myLocation = new Tile(GetComponent<TileItem>());
        List<DistractionComponent> potentialDistractions = new List<DistractionComponent>(FindObjectsOfType<DistractionComponent>());
        Tile distractionLocation = new Tile(potentialDistractions[0].GetComponent<TileItem>());
        int minDistance = Pathfinding.FindPath(myLocation, distractionLocation, true).Count;
        foreach (DistractionComponent distraction in potentialDistractions)
        {
            Tile potentialDistractionLocation = new Tile(distraction.GetComponent<TileItem>());
            int potentialDistance = Pathfinding.FindPath(myLocation, potentialDistractionLocation, true).Count;
            if (potentialDistance < minDistance)
            {
                minDistance = potentialDistance;
                distractionLocation = potentialDistractionLocation;
            }
        }

        return distractionLocation;
    }
}
