using UnityEngine;
using System.Collections;

public class Tile {

	private int xPos;
	private int yPos;

	public int X {
		get { return xPos; }
	}

	public int Y {
		get { return yPos; }
	}

	public Tile(int xPos, int yPos) {
		this.xPos = xPos;
		this.yPos = yPos;
	}
}
