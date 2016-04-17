using UnityEngine;
using System.Collections;

[RequireComponent (typeof (TileItem))]
public class WallMaker : MonoBehaviour {

	public Sprite lonetop;
	public Sprite uplefttop;
	public Sprite endtop;
	public Sprite horizontaltop;
	public Sprite verticaltop;
	public Sprite wallpaper;

	private const int WALL_Z = 0;

	// Use this for initialization
	void Start () {
		// get the list of rooms
		Room[] rooms = FindObjectsOfType<Room> ();

		// Grab the first room
		foreach (Room room in rooms) {
			// Make walls all the way around the room
			// TODO add doors

			TileItem ti = room.GetComponent<TileItem>();
			Debug.Log ("Room width: " + ti.tileW);
			// Top left corner
			instantiateWall (ti.tileX - 1, ti.tileY + ti.tileH + 1, uplefttop);
			// Left Wall
			for (int y = ti.tileY ; y <= ti.tileY + ti.tileH; y++) {
				instantiateWall (ti.tileX - 1, y, verticaltop);
			}
			// Bottom left corner
			instantiateWall (ti.tileX - 1, ti.tileY - 1, uplefttop, 90);
			// Bottom wall
			for (int x = ti.tileX; x < ti.tileX + ti.tileW; x++) {
				instantiateWall (x, ti.tileY-1, horizontaltop);
			}
			// Bottom right corner
			instantiateWall (ti.tileX + ti.tileW, ti.tileY - 1, uplefttop, 180);
			// Right wall
			for (int y = ti.tileY ; y <= ti.tileY + ti.tileH; y++) {
				instantiateWall (ti.tileX + ti.tileW, y, verticaltop);
			}
			// Top right corner
			instantiateWall (ti.tileX + ti.tileW, ti.tileY + ti.tileH + 1, uplefttop, 270);
			// Top wallpaper
			for (int x = ti.tileX; x < ti.tileX + ti.tileW; x++) {
				instantiateWall (x, ti.tileY + ti.tileH, wallpaper);
			}
			// Top wall
			for (int x = ti.tileX; x < ti.tileX + ti.tileW; x++) {
				instantiateWall (x, ti.tileY + ti.tileH + 1, horizontaltop);
			}
		}
	}

	// Create a wall segment with x and y tile coordinates
	// Uses the given sprite rotated angle degrees counter-clockwise
	void instantiateWall(int x, int y, Sprite sprite) {
		instantiateWall (x, y, sprite, 0);
	}

	// Create a wall segment with x and y tile coordinates
	// Uses the given sprite rotated angle degrees counter-clockwise
	void instantiateWall(int x, int y, Sprite sprite, int angle) {
		GameObject g = new GameObject ();

		SpriteRenderer sr = g.AddComponent<SpriteRenderer> ();
        sr.sprite = sprite;
        g.AddComponent<BoxCollider2D>();

        g.transform.Rotate (0, 0, angle);

        g.AddComponent<Wall>();
        g.AddComponent<BlocksLineOfSight>();
        g.layer = LayerMask.NameToLayer("VisibilityBlocking");

		TileItem ti = g.AddComponent<TileItem> ();
		ti.setTilePosition (x, y);
		g.transform.Translate (0, 0, Room.ROOM_TILE_Z_INDEX); // TODO(aklen): Hack.
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}