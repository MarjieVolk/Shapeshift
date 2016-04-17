using UnityEngine;
using System.Collections;

[RequireComponent (typeof (TileItem))]
[RequireComponent(typeof(SpriteRenderer))]
public class FurnitureItem : MonoBehaviour {
    public Sprite image;
    public RoomType[] roomTypes;
    public int tileWidth = 1;
    public int tileHeight = 1;

    // Use this for initialization
    protected void Start () {
        gameObject.GetComponent<TileItem>().tileW = tileWidth;
        gameObject.GetComponent<TileItem>().tileH = tileHeight;
    }

    // Update is called once per frame
    protected void Update () {

    }
}
