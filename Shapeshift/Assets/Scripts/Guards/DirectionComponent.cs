using UnityEngine;
using System.Collections;

public class DirectionComponent : MonoBehaviour {

    public Direction Direction
    {
        get
        {
            //scale to a 0-4 angle representation
            float quadrants = (Angle / (Mathf.PI / 2));

            //shift by 45 degrees clockwise and quantize
            switch ((int)((quadrants + .5f) % 4))
            {
                case 0:
                    return Direction.EAST;
                case 1:
                    return Direction.NORTH;
                case 2:
                    return Direction.WEST;
                case 3:
                    return Direction.SOUTH;
                default://impossible
                    return Direction.SOUTH;
            }
        }

        set
        {
            switch (value)
            {
                case Direction.EAST:
                    Angle = 0;
                    break;
                case Direction.NORTH:
                    Angle = Mathf.PI / 2;
                    break;
                case Direction.WEST:
                    Angle = Mathf.PI;
                    break;
                case Direction.SOUTH:
                    Angle = 3 * Mathf.PI / 2;
                    break;
            }
            //Angle -= Mathf.PI / 4;
        }
    }

    /// <summary>
    /// The angle, in radians, the guard is facing.
    /// West is 0, South is PI/2, etc.
    /// </summary>
    public float Angle { get; set; }
	
    // Use this for initialization
	void Start () {
        Direction = Direction.SOUTH;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
