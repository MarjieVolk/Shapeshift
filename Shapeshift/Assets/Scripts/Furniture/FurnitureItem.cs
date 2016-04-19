using UnityEngine;
using System.Collections;

[RequireComponent (typeof (TileItem))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CollisionEventCommunicator))]
public class FurnitureItem : MonoBehaviour {

    public RoomType[] roomTypes;

    // Use this for initialization
    protected void Start () {
    }

    // Update is called once per frame
    protected void Update () {
    }
}
