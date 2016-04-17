using UnityEngine;
using System.Collections;

public class WallMaker : MonoBehaviour {

	public Sprite loneSprite;
	public Sprite upLeftSprite;
	public Sprite endSprite;
	public Sprite horizontalSprite;
	public Sprite verticalSprite;
	public Sprite tSprite;
	public Sprite quadSprite;

	private const int WALL_Z = 0;

	private static Sprite[,,,] spriteConfig = new Sprite[2,2,2,2];
	private static int[,,,] rotationConfig = new int[2,2,2,2];

	// Save the sprite and rotation for a configuration of walls
	void initConfig(int top, int bottom, int left, int right, Sprite s, int r) {
		spriteConfig[top, bottom, left, right] = s;
		rotationConfig [top, bottom, left, right] = r;
	}	

	// Use this for initialization
	void Start () {
		// Initialize sprites for every combination of walls
		initConfig (0, 0, 0, 0, loneSprite, 0);
		initConfig (0, 0, 0, 1, endSprite, 0);
		initConfig (0, 0, 1, 0, endSprite, 180);
		initConfig (0, 0, 1, 1, horizontalSprite, 0);
		initConfig (0, 1, 0, 0, endSprite, 270);
		initConfig (0, 1, 0, 1, upLeftSprite, 0);
		initConfig (0, 1, 1, 0, upLeftSprite, 270);
		initConfig (0, 1, 1, 1, tSprite, 0);
		initConfig (1, 0, 0, 0, endSprite, 90);
		initConfig (1, 0, 0, 1, upLeftSprite, 90);
		initConfig (1, 0, 1, 0, upLeftSprite, 180);
		initConfig (1, 0, 1, 1, tSprite, 180);
		initConfig (1, 1, 0, 0, verticalSprite, 0);
		initConfig (1, 1, 0, 1, tSprite, 90);
		initConfig (1, 1, 1, 0, tSprite, 270);
		initConfig (1, 1, 1, 1, quadSprite, 0);

		// get the list of rooms
		Room[] rooms = FindObjectsOfType<Room> ();

		// Wall Generation
		TileItem ti = rooms[0].GetComponent<TileItem> ();
		int top = ti.tileY, right = ti.tileX, bottom = ti.tileY, left = ti.tileX;

		// Find the four edges of the building floor, by looking through each room
		foreach (Room room in rooms) {
			ti = room.GetComponent<TileItem> ();
			if (top < ti.tileY + ti.tileH + 2) {
				top = ti.tileY + ti.tileH + 2;
			}
			if (right < ti.tileX + ti.tileW + 1) {
				right = ti.tileX + ti.tileW + 1;
			}
			if (left > ti.tileX - 1) {
				left = ti.tileX - 1;
			}
			if (bottom > ti.tileY - 1) {
				bottom = ti.tileY - 1;
			}
		}
        
		int floorwidth = right - left;
		int floorheight = top - bottom;
		bool [,] wallFlags = new bool[floorwidth, floorheight]; // Where walls should go
		bool [,] wallpaperFlags = new bool[floorwidth, floorheight]; // Where wallpaper should go
		RoomTileSet[,] wallPaperSets = new RoomTileSet[floorwidth, floorheight]; // What wallpaper sprites should be used where

        // Add walls around each room
		foreach (Room room in rooms) {
            ti = room.GetComponent<TileItem>();
            for (int x = ti.tileX; x < ti.tileX + ti.tileW; x++) {
                wallFlags[x - left, ti.tileY + ti.tileH - bottom + 1] = true;
                wallFlags[x - left, ti.tileY + ti.tileH - bottom] = true;
                wallFlags[x - left, ti.tileY - bottom - 1] = true;

                wallpaperFlags[x - left, ti.tileY + ti.tileH - bottom] = true;
                wallPaperSets[x - left, ti.tileY + ti.tileH - bottom] = room.tileSet;
            }

            for (int y = ti.tileY - 1; y < ti.tileY + ti.tileH + 2; y++) {
                wallFlags[ti.tileX - left - 1, y - bottom] = true;
                wallFlags[ti.tileX - left + ti.tileW, y - bottom] = true;
            }
        }

        // Remove any walls that are overlapping a room's floor
        foreach (Room room in rooms) {
            ti = room.GetComponent<TileItem>();
            for (int x = ti.tileX; x < ti.tileX + ti.tileW; x++) {
                for (int y = ti.tileY; y < ti.tileY + ti.tileH; y++) {
                    wallFlags[x - left, y - bottom] = false;
                }
            }
        }

		// Debug.Log ("(top, bottom, left, right): (" + top + ", " + bottom + ", " + left + ", " + right + ")");
		for (int x = left; x < right; x++) {
			for (int y = bottom; y < top; y++) {
				int xIndex = x - left;
				int yIndex = y - bottom;
				if (wallFlags [xIndex, yIndex]) {
					if (wallpaperFlags [xIndex, yIndex]) {

						// Determine whether wallpaper continues left or right of you
						int xLeft = xIndex - 1;
						bool wallLeft = xLeft >= 0 & wallFlags [xLeft, yIndex] && wallpaperFlags [xLeft, yIndex];
						int xRight = xIndex + 1;
						bool wallRight = xRight < floorwidth && wallFlags [xRight, yIndex] && wallpaperFlags [xRight, yIndex];

						// Instantiate with the proper sprite (and manipulation)
						if (wallLeft && wallRight) {
							instantiateWall (x, y, wallPaperSets [xIndex, yIndex].wallOpen);
						} else if (wallLeft) {
							instantiateWall (x, y, wallPaperSets [xIndex, yIndex].wallLeftCorner, 0, true);
						} else if (wallRight) {
							instantiateWall (x, y, wallPaperSets [xIndex, yIndex].wallLeftCorner);
						} else {
							instantiateWall (x, y, wallPaperSets [xIndex, yIndex].wallTwoCorners);
						}
					} else {

						// Determine whether the walls continue in each direction (ternary convert bool to int for table lookup)
						int yAbove = yIndex + 1;
						int wallAbove = yAbove < floorheight && wallFlags [xIndex, yAbove] && !wallpaperFlags [xIndex, yAbove]
							? 1 : 0;
						int yBelow = yIndex - 1;
						int wallBelow = yBelow >= 0 && wallFlags [xIndex, yBelow] && !wallpaperFlags [xIndex, yBelow]
							? 1 : 0;
						int xLeft = xIndex - 1;
						int wallLeft = xLeft >= 0 && wallFlags [xLeft, yIndex] && !wallpaperFlags [xLeft, yIndex]
							? 1 : 0;
						int xRight = xIndex + 1;
						int wallRight = xRight < floorwidth && wallFlags [xRight, yIndex] && !wallpaperFlags [xRight, yIndex]
							? 1 : 0;

						// Instantiate the proper wall type and rotation based on table lookups
						// Debug.Log ("At (" + x + ", " + y + "): (" + wallAbove + ", " + wallBelow + ", " + wallLeft + ", " + wallRight + ")");
						instantiateWall (x, y, spriteConfig[wallAbove, wallBelow, wallLeft, wallRight],
							rotationConfig[wallAbove, wallBelow, wallLeft, wallRight]);
					}
				}
			}
		}
	}	

	// Create a wall segment with x and y tile coordinates
	void instantiateWall(int x, int y, Sprite sprite) {
		instantiateWall (x, y, sprite, 0, false);
	}

	// Create a wall segment with x and y tile coordinates
	// Uses the given sprite rotated angle degrees counter-clockwise
	void instantiateWall(int x, int y, Sprite sprite, int angle) {
		instantiateWall (x, y, sprite, angle, false);
	}

	// Create a wall segment with x and y tile coordinates
	// Uses the given sprite rotated angle degrees counter-clockwise
	// Optionally flip along the x axis
	void instantiateWall(int x, int y, Sprite sprite, int angle, bool flipX) {
		// Debug.Log ("Adding a " + sprite.name + " wall at (" + x + ", " + y + "), rotated " + angle);
		GameObject g = new GameObject ();

		SpriteRenderer sr = g.AddComponent<SpriteRenderer> ();
        sr.sprite = sprite;
		sr.flipX = flipX;

        g.AddComponent<BoxCollider2D>();

        g.transform.Rotate (0, 0, angle);

        g.AddComponent<Wall>();
        g.AddComponent<BlocksLineOfSight>();
        g.layer = LayerMask.NameToLayer("VisibilityBlocking");

		TileItem ti = g.AddComponent<TileItem> ();
		ti.SetTilePosition (x, y);
		g.transform.Translate (0, 0, Room.ROOM_TILE_Z_INDEX); // TODO(aklen): Hack.
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
